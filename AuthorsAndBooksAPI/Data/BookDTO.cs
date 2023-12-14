namespace AuthorsAndBooksAPI.Data
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Genre { get; set; }
        public string MAINCHARACTER { get; set; }
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; } = null!;

    }
}
