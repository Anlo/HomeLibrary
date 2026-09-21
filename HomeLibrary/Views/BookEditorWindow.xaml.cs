using System.Windows;
using HomeLibrary.Models;
using HomeLibrary.ViewModels;
using Microsoft.Web.WebView2.Core;


namespace HomeLibrary.Views
{
    public partial class BookEditorWindow : Window
    {
        public BookEditorWindow(Book book)
        {
            InitializeComponent();
            DataContext = new BookEditorViewModel(book);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Инициализация WebView2
            await HtmlEditor.EnsureCoreWebView2Async(
                await CoreWebView2Environment.CreateAsync());

            // HTML с TinyMCE (загружается с CDN)
            var html = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <script src='https://cdn.tiny.cloud/1/no-api-key/tinymce/6/tinymce.min.js' referrerpolicy='origin'></script>
</head>
<body>
    <textarea id='editor'></textarea>
    <script>
        tinymce.init({
            selector: '#editor',
            height: '100%',
            menubar: false,
            plugins: 'lists link image table code',
            toolbar: 'undo redo | bold italic underline | bullist numlist | link image | code',
            setup: function(ed) {
                ed.on('init', function() {
                    // Получаем начальный XML от C#
                    window.chrome.webview.postMessage({ type: 'ready' });
                });
                ed.on('change', function() {
                    window.chrome.webview.postMessage({
                        type: 'content',
                        html: ed.getContent()
                    });
                });
            }
        });

        // Слушаем сообщения от C#
        window.chrome.webview.addEventListener('message', function(event) {
            if (event.data.type === 'setContent') {
                tinymce.get('editor').setContent(event.data.html || '');
            }
        });
    </script>
</body>
</html>";

            HtmlEditor.NavigateToString(html);

            // Обработка сообщений от HTML
            HtmlEditor.WebMessageReceived += (s, args) =>
            {
                var json = args.TryGetWebMessageAsString();
                // Простейший парсинг (для продакшена используйте System.Text.Json)
                if (json.Contains("\"type\":\"ready\""))
                {
                    var vm = (BookEditorViewModel)DataContext;
                    var xml = vm.Book.TableOfContentsXml;
                    // Извлекаем содержимое между тегами <content>...</content>, если есть
                    var content = ExtractContent(xml);
                    HtmlEditor.PostWebMessageAsJson(
                        $"{{\"type\":\"setContent\",\"html\":{System.Text.Json.JsonSerializer.Serialize(content)}}}");
                }
                else if (json.Contains("\"type\":\"content\""))
                {
                    // Извлекаем html из JSON
                    var start = json.IndexOf("\"html\":") + 8;
                    var end = json.LastIndexOf("\"");
                    var htmlContent = json.Substring(start, end - start);
                    // Декодируем escape-последовательности
                    htmlContent = System.Text.Json.JsonSerializer.Deserialize<string>(
                        "\"" + htmlContent + "\"") ?? string.Empty;

                    var vm = (BookEditorViewModel)DataContext;
                    // Оборачиваем в XML
                    vm.Book.TableOfContentsXml =
                        $"<TableOfContents><Content><![CDATA[{htmlContent}]]></Content></TableOfContents>";
                }
            };
        }

        private static string ExtractContent(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return string.Empty;
            try
            {
                var doc = System.Xml.Linq.XDocument.Parse(xml);
                var content = doc.Root?.Element("Content")?.Value;
                return content ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}