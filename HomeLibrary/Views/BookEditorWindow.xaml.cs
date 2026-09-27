using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
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
            // Даем WPF выполнить первичную разметку, чтобы WebView2 имел реальный размер.
            await Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => { }));

            await HtmlEditor.EnsureCoreWebView2Async(await CoreWebView2Environment.CreateAsync());

            var html = LoadEditorHtml();
            HtmlEditor.NavigateToString(html);

            // Ждём навигацию к содержимому (не к initial about:blank — иначе гонка и вис).
            var navTcs = new TaskCompletionSource<bool>();
            HtmlEditor.CoreWebView2.NavigationCompleted += (s, ev) => navTcs.TrySetResult(true);
            await navTcs.Task;

            var vm = (BookEditorViewModel)DataContext;
            var storedXml = vm.Book.TableOfContentsXml ?? string.Empty;

            var htmlFromXml = await HtmlEditor.ExecuteScriptAsync(
                $"window.xmlToHtml({JsonSerializer.Serialize(storedXml)});");
            var editableHtml = JsonSerializer.Deserialize<string>(htmlFromXml) ?? string.Empty;

            await HtmlEditor.ExecuteScriptAsync(
                $"window.setEditorHtml({JsonSerializer.Serialize(editableHtml)});");
        }

        private static string LoadEditorHtml()
        {
            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("HomeLibrary.Helpers.XmlEditor.html");
            if (stream is null)
                throw new InvalidOperationException("Embedded resource 'HomeLibrary.Helpers.XmlEditor.html' not found.");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        // Публичный метод для получения XML из редактора
        public async Task<string> GetTableOfContentsXmlAsync()
        {
            try
            {
                var result = await HtmlEditor.ExecuteScriptAsync("window.getTocXml();");
                var xml = JsonSerializer.Deserialize<string>(result) ?? string.Empty;
                return xml;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения содержимого: {ex.Message}");
                return string.Empty;
            }
        }
    }
}