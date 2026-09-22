using System.Windows;
using HomeLibrary.Models;
using HomeLibrary.ViewModels;
using Microsoft.Web.WebView2.Core;
using System.Text.Json;

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

            // Подписываемся на событие завершения навигации
            var tcs = new TaskCompletionSource<bool>();
            HtmlEditor.NavigationCompleted += (s, ev) =>
            {
                tcs.TrySetResult(ev.IsSuccess);
            };


            // HTML с Quill.js (бесплатный редактор)
            var html = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <link href='https://cdn.quilljs.com/1.3.6/quill.snow.css' rel='stylesheet'>
    <script src='https://cdn.quilljs.com/1.3.6/quill.js'></script>
    <style>
        body { margin: 0; padding: 10px; font-family: Arial, sans-serif; }
        #editor { height: 400px; font-size: 14px; }
        .ql-container { font-family: Arial, sans-serif; }
    </style>
</head>
<body>
    <div id='editor'></div>
    <script>
        var quill = new Quill('#editor', {
            theme: 'snow',
            placeholder: 'Введите оглавление книги...',
            modules: {
                toolbar: [
                    [{ 'header': [1, 2, 3, false] }],
                    ['bold', 'italic', 'underline'],
                    [{ 'list': 'ordered'}, { 'list': 'bullet' }],
                    ['link', 'image'],
                    ['clean']
                ]
            }
        });

        // Функция для получения HTML содержимого
        window.getEditorHtml = function() {
            return quill.root.innerHTML;
        };

        // Функция для установки HTML содержимого
        window.setEditorHtml = function(html) {
            quill.root.innerHTML = html || '';
        };
    </script>
</body>
</html>";

            HtmlEditor.NavigateToString(html);

            // Ждём загрузки HTML
            await tcs.Task;

            // Загружаем существующее оглавление, если оно есть
            var vm = (BookEditorViewModel)DataContext;
            var existingContent = ExtractContent(vm.Book.TableOfContentsXml);
            if (!string.IsNullOrWhiteSpace(existingContent))
            {
                // Экранируем содержимое для JavaScript
                var escapedContent = existingContent
                    .Replace("\\", "\\\\")
                    .Replace("'", "\\'")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r");

                await HtmlEditor.ExecuteScriptAsync(
                    $"window.setEditorHtml('{escapedContent}');");
            }
        }

        // Публичный метод для получения XML из редактора
        public async Task<string> GetTableOfContentsXmlAsync()
        {
            try
            {
                // Выполняем JavaScript и получаем HTML содержимое
                var result = await HtmlEditor.ExecuteScriptAsync("window.getEditorHtml();");

                // ExecuteScriptAsync возвращает JSON-строку в кавычках, нужно убрать их
                var htmlContent = JsonSerializer.Deserialize<string>(result) ?? string.Empty;

                if (string.IsNullOrWhiteSpace(htmlContent))
                    return string.Empty;

                // Оборачиваем в XML
                return $"<TableOfContents><Content><![CDATA[{htmlContent}]]></Content></TableOfContents>";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения содержимого: {ex.Message}");
                return string.Empty;
            }
        }

        private static string ExtractContent(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return string.Empty;
            try
            {
                var doc = System.Xml.Linq.XDocument.Parse(xml);
                return doc.Root?.Element("Content")?.Value ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}