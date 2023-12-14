using System.Diagnostics.Metrics;
using System.Security;
using AuthorsAndBooksAPI.Data.Models;
using AuthorsAndBooksAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;


namespace AuthorsAndBooksAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize(Roles = "Administrator")]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public SeedController(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _env = env;
            _configuration = configuration;
        }


        [HttpGet]
        public async Task<ActionResult> Import()
        {
            // prevents non-development environments from running this method
            if (!_env.IsDevelopment())
                throw new SecurityException("Not allowed");
            var path = Path.Combine(
                _env.ContentRootPath,
                "Data/Source/AuthorsAndBooks.xlsx");
            using var stream = System.IO.File.OpenRead(path);
            using var excelPackage = new ExcelPackage(stream);
            // get the first worksheet 
            var worksheet = excelPackage.Workbook.Worksheets[0];
            // define how many rows we want to process 
            var nEndRow = worksheet.Dimension.End.Row;
            // initialize the record counters 
            var numberOfAuthorsAdded = 0;
            var numberOfBooksAdded = 0;
            // create a lookup dictionary 
            // containing all the countries already existing 
            // into the Database (it will be empty on first run).
            var AuthorsByName = _context.Authors
                .AsNoTracking()
                .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            // iterates through all rows, skipping the first one 
            for (int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = worksheet.Cells[
                    nRow, 1, nRow, worksheet.Dimension.End.Column];
                var authorName = row[nRow, 5].GetValue<string>();
                var countryOfOrigin = row[nRow, 6].GetValue<string>();
                var gender = row[nRow, 7].GetValue<string>();
                // skip this country if it already exists in the database
                if (AuthorsByName.ContainsKey(authorName))
                    continue;
                // create the Country entity and fill it with xlsx data 
                var author = new Author
                {
                    Name = authorName,
                    COUNTRYOFORIGIN = countryOfOrigin,
                    Gender = gender
                };
                // add the new country to the DB context 
                await _context.Authors.AddAsync(author);
                // store the country in our lookup to retrieve its Id later on
                AuthorsByName.Add(authorName, author);
                // increment the counter 
                numberOfAuthorsAdded++;
            }
            // save all the countries into the Database 
            if (numberOfAuthorsAdded > 0)
                await _context.SaveChangesAsync();
            // create a lookup dictionary
            // containing all the cities already existing 
            // into the Database (it will be empty on first run). 
            var books = _context.Books
                .AsNoTracking()
                .ToDictionary(x => (
                    Title: x.Title,
                    Genre: x.Genre,
                    MAINCHARACTER: x.MAINCHARACTER,
                    AuthorId: x.AuthorId));
            // iterates through all rows, skipping the first one 
            for (int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = worksheet.Cells[
                    nRow, 1, nRow, worksheet.Dimension.End.Column];
                var title = row[nRow, 1].GetValue<string>();
                var title_copy = row[nRow, 2].GetValue<string>();
                var genre = row[nRow, 3].GetValue<String>();
                var mainCharacter = row[nRow, 4].GetValue<String>();
                var authorName = row[nRow, 5].GetValue<string>();
                // retrieve country Id by countryName
                var authorId = AuthorsByName[authorName].Id;
                // skip this city if it already exists in the database
                if (books.ContainsKey((
                    Title: title,
                    Genre: genre,
                    MAINCHARACTER: mainCharacter,
                    AuthorId: authorId)))
                    continue;
                // create the City entity and fill it with xlsx data 
                var book = new Book
                {
                    Title = title,
                    Genre = genre,
                    MAINCHARACTER = mainCharacter,
                    AuthorId = authorId
                };
                // add the new city to the DB context 
                _context.Books.Add(book);
                // increment the counter 
                numberOfBooksAdded++;
            }
            // save all the cities into the Database 
            if (numberOfBooksAdded > 0)
                await _context.SaveChangesAsync();
            return new JsonResult(new
            {
                Books = numberOfBooksAdded,
                Authors = numberOfAuthorsAdded
            });
        }








        [HttpGet]
        public async Task<ActionResult> CreateDefaultUsers()
        {
            // setup the default role names
            string role_RegisteredUser = "RegisteredUser";
            string role_Administrator = "Administrator";
            // create the default roles (if they don't exist yet)
            if (await _roleManager.FindByNameAsync(role_RegisteredUser) ==
             null)
                await _roleManager.CreateAsync(new
                 IdentityRole(role_RegisteredUser));
            if (await _roleManager.FindByNameAsync(role_Administrator) ==
             null)
                await _roleManager.CreateAsync(new
                 IdentityRole(role_Administrator));
            // create a list to track the newly added users
            var addedUserList = new List<ApplicationUser>();
            // check if the admin user already exists
            var email_Admin = "admin@email.com";
            if (await _userManager.FindByNameAsync(email_Admin) == null)
            {
                // create a new admin ApplicationUser account
                var user_Admin = new ApplicationUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_Admin,
                    Email = email_Admin,
                };
                // insert the admin user into the DB
                await _userManager.CreateAsync(user_Admin, _configuration["DefaultPasswords:Administrator"]);
                // assign the "RegisteredUser" and "Administrator" roles
                await _userManager.AddToRoleAsync(user_Admin,
                 role_RegisteredUser);
                await _userManager.AddToRoleAsync(user_Admin,
                 role_Administrator);
                // confirm the e-mail and remove lockout
                user_Admin.EmailConfirmed = true;
                user_Admin.LockoutEnabled = false;
                // add the admin user to the added users list
                addedUserList.Add(user_Admin);
            }
            // check if the standard user already exists
            var email_User = "user@email.com";
            if (await _userManager.FindByNameAsync(email_User) == null)
            {
                // create a new standard ApplicationUser account
                var user_User = new ApplicationUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_User,
                    Email = email_User
                };
                // insert the standard user into the DB
                await _userManager.CreateAsync(user_User, _configuration["DefaultPasswords:RegisteredUser"]);
                // assign the "RegisteredUser" role
                await _userManager.AddToRoleAsync(user_User,
                 role_RegisteredUser);
                // confirm the e-mail and remove lockout
                user_User.EmailConfirmed = true;
                user_User.LockoutEnabled = false;
                // add the standard user to the added users list
                addedUserList.Add(user_User);
            }
            // if we added at least one user, persist the changes into the DB
            if (addedUserList.Count > 0)
                await _context.SaveChangesAsync();
            return new JsonResult(new
            {
                Count = addedUserList.Count,
                Users = addedUserList
            });
        }
    }
    }

