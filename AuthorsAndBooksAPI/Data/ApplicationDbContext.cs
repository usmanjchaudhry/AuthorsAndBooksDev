using AuthorsAndBooksAPI.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthorsAndBooksAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext() : base()
        {
        }

        public ApplicationDbContext(DbContextOptions options)
             : base(options)
        {
        }


        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();

    }
}