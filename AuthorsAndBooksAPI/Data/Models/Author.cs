using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorsAndBooksAPI.Data.Models
{

    [Table("Authors")]
    [Index(nameof(Name))]
    [Index(nameof(COUNTRYOFORIGIN))]
    [Index(nameof(Gender))]
    public class Author

    {


        #region Properties
        /// <summary>
        /// The unique id and primary key for this Country
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Country name (in UTF8 format)
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        ///  Author Country of Origin (in ISO 3166-1 ALPHA-2 format)
        /// </summary>
        public string COUNTRYOFORIGIN { get; set; } = null!;
        /// <summary>
        ///Author Gender (in ISO 3166-1 ALPHA-3 format)
        /// </summary>
        public string Gender { get; set; } = null!;
        #endregion

        #region Navigation Properties
        /// <summary>
        /// A collection of all the BOoks related to this Author.
        /// </summary>
        public ICollection<Book>? Books { get; set; } = null!;
        #endregion
    }
}
