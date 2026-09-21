using CommunityToolkit.Mvvm.ComponentModel;
using System.Security.Policy;

namespace HomeLibrary.Models
{
    public partial class Book : ObservableObject
    {
        [ObservableProperty] private int id;
        [ObservableProperty] private string title = string.Empty;
        [ObservableProperty] private string author = string.Empty;
        [ObservableProperty] private int publishYear;
        [ObservableProperty] private int pageCount;
        [ObservableProperty] private string tableOfContentsXml = string.Empty; // XML-строка
        [ObservableProperty] private DateTime createdAt;
        [ObservableProperty] private DateTime updatedAt;

        public override string ToString() => $"{Author} — {Title} ({PublishYear})";
    }
}