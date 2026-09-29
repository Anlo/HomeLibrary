using System;
using HomeLibrary.Data;
using HomeLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeLibrary.Pages.Book
{
    public class EditPageModel : PageModel
    {
        private readonly BookRepository _repository = new();

        [BindProperty] public HomeLibrary.Models.Book Book { get; set; } = new HomeLibrary.Models.Book();

        public void OnGet(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                var book = _repository.GetById(id.Value);
                if (book is null)
                {
                    TempData["Message"] = "Книга не найдена.";
                    RedirectToPage("/Index");
                    return;
                }
                Book = book;
            }
            else
            {
                Book = new HomeLibrary.Models.Book
                {
                    TableOfContentsXml = string.Empty
                };
            }
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Book.Title) || string.IsNullOrWhiteSpace(Book.Author))
            {
                ModelState.AddModelError("Book", "Название и автор обязательны.");
                return Page();
            }

            try
            {
                if (Book.Id == 0)
                    Book.Id = _repository.Insert(Book);
                else
                    _repository.Update(Book);

                TempData["Message"] = $"Книга «{Book.Title}» сохранена.";
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
        }
    }
}
