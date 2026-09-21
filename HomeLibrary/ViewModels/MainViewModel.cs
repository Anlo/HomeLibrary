using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeLibrary.Data;
using HomeLibrary.Models;

namespace HomeLibrary.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly BookRepository _repository = new();

        [ObservableProperty] private ObservableCollection<Book> books = new();
        [ObservableProperty] private Book? selectedBook;
        [ObservableProperty] private string searchText = string.Empty;

        public MainViewModel()
        {
            LoadBooks();
        }

        [RelayCommand]
        private void LoadBooks()
        {
            var list = _repository.GetAll();
            Books = new ObservableCollection<Book>(list);
        }

        [RelayCommand]
        private void Search()
        {
            var list = string.IsNullOrWhiteSpace(SearchText)
                ? _repository.GetAll()
                : _repository.Search(SearchText);
            Books = new ObservableCollection<Book>(list);
        }

        [RelayCommand]
        private void AddBook()
        {
            var editor = new Views.BookEditorWindow(new Book());
            if (editor.ShowDialog() == true)
                LoadBooks();
        }

        [RelayCommand]
        private void EditBook()
        {
            if (SelectedBook is null) return;
            var editor = new Views.BookEditorWindow(SelectedBook);
            if (editor.ShowDialog() == true)
                LoadBooks();
        }

        [RelayCommand]
        private void DeleteBook()
        {
            if (SelectedBook is null) return;
            var result = System.Windows.MessageBox.Show(
                $"Удалить книгу \"{SelectedBook.Title}\"?",
                "Подтверждение",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                _repository.Delete(SelectedBook.Id);
                LoadBooks();
            }
        }
    }
}