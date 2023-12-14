using System.Text.Json.Serialization;

namespace AuthorsAndBooksAPI.Data
{
    public class AuthorDTO
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string COUNTRYOFORIGIN { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public int? TotBooks { get; set; } = null!;
        #endregion
    }
}
