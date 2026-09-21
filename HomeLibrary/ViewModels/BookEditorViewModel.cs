using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeLibrary.Data;
using HomeLibrary.Models;
using System.Windows;

namespace HomeLibrary.ViewModels
{
    public partial class BookEditorViewModel : ObservableObject
    {
        private readonly BookRepository _repository = new();
        private readonly bool _isNew;

        [ObservableProperty] private Book book;

        public BookEditorViewModel(Book book)
        {
            Book = book;
            _isNew = book.Id == 0;
        }

        [RelayCommand]
        private async void Save(Window window)
        {
            if (string.IsNullOrWhiteSpace(Book.Title) ||
                string.IsNullOrWhiteSpace(Book.Author))
            {
                MessageBox.Show("Название и автор обязательны!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем актуальное содержимое редактора перед сохранением
            if (window is Views.BookEditorWindow editorWindow)
            {
                var xmlContent = await editorWindow.GetTableOfContentsXmlAsync();
                Book.TableOfContentsXml = xmlContent;

                System.Diagnostics.Debug.WriteLine($"Сохраняем XML: {xmlContent}");
            }

            try
            {
                if (_isNew)
                    _repository.Insert(Book);
                else
                    _repository.Update(Book);

                window.DialogResult = true;
                window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            window.DialogResult = false;
            window.Close();
        }
    }
}