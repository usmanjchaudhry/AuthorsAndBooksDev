using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace AuthorsAndBooksAPI.Data.Models
{

    [Table("Books")]
    [Index(nameof(Title))]
    [Index(nameof(Genre))]
    [Index(nameof(MAINCHARACTER))]

    public class Book
    {
        #region Properties
        /// <summary>
        /// The unique id and primary key for this Book
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Book Title (in UTF8 format)
        /// </summary>
        public string Title { get; set; } = null!;
        /// <summary>
        ///Book Genre
        /// </summary>
        /// </summary>
        public string Genre { get; set; } = null!;
        /// <summary>
        /// Book made to film?
        /// </summary>
        public string MAINCHARACTER { get; set; } = null!;
        /// <summary>
        /// Author Id (foreign key)
        /// </summary>
        /// [ForeignKey(nameof(Country))]
        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
        #endregion
        #region Navigation Properties
        /// <summary>
        /// The author related to this book.
        /// </summary>
        public Author? Author { get; set; } = null!;
        #endregion
    }
}
