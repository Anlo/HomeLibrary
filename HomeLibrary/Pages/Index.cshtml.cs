using System.Collections.Generic;
using HomeLibrary.Data;
using HomeLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeLibrary.Pages
{
    public class IndexPageModel : PageModel
    {
        private readonly BookRepository _repository = new();

        public List<HomeLibrary.Models.Book> Books { get; set; } = new List<HomeLibrary.Models.Book>();
        public string SearchText { get; set; } = string.Empty;
        public int TotalCount => Books.Count;

        public void OnGet(string? searchText)
        {
            System.IO.File.AppendAllText(@"C:\Temp\kilo\debug.txt", $"OnGet at {DateTime.UtcNow:O}\n");
            SearchText = searchText ?? string.Empty;
            Books = string.IsNullOrWhiteSpace(SearchText)
                ? _repository.GetAll()
                : _repository.Search(SearchText);
        }

        public IActionResult OnPostDelete(int id)
        {
            try
            {
                System.IO.File.AppendAllText(@"C:\Temp\kilo\delete_debug.txt",
                    $"OnPostDelete id={id} at {DateTime.UtcNow:O}\n");
                _repository.Delete(id);
                TempData["Message"] = "Книга удалена.";
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(@"C:\Temp\kilo\delete_debug.txt",
                    $"OnPostDelete EXCEPTION id={id}: {ex.Message}\n");
                TempData["Message"] = $"Ошибка удаления: {ex.Message}";
            }
            return RedirectToPage();
        }
    }
}
