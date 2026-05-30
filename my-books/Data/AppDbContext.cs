using Microsoft.EntityFrameworkCore;
using my_books.Data.Model;

namespace my_books.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Book> Books { get; set; }
    }
}
