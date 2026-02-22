namespace BookCollection.Model
{
    public class Book
    {
        // Properties
        // Id set in Repository -> Create method 
        public int Id { get; set; }
        public string Title {  get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int PublicationYear { get; set; }

        // Constructor
        public Book(string title, string author, string genre, int year)
        {
            Title = title ?? string.Empty;
            Author = author ?? string.Empty;
            Genre = genre ?? string.Empty;
            PublicationYear = year;
        }
    }
}
