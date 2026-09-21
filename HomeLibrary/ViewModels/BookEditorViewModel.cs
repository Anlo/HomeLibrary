using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeLibrary.Data;
using HomeLibrary.Models;

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
        private void Save(System.Windows.Window window)
        {
            if (string.IsNullOrWhiteSpace(Book.Title) ||
                string.IsNullOrWhiteSpace(Book.Author))
            {
                System.Windows.MessageBox.Show("Название и автор обязательны!",
                    "Ошибка", System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            if (_isNew)
                _repository.Insert(Book);
            else
                _repository.Update(Book);

            window.DialogResult = true;
            window.Close();
        }

        [RelayCommand]
        private void Cancel(System.Windows.Window window)
        {
            window.DialogResult = false;
            window.Close();
        }
    }
}