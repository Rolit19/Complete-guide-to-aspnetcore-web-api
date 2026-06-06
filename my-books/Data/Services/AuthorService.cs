using my_books.Data.Model;
using my_books.Data.ViewModel;

namespace my_books.Data.Services
{
    public class AuthorService
    {
        private AppDbContext _context;  
        public AuthorService(AppDbContext context)
        {
            _context = context;
        }

        public void AddAuthor(AuthorVM author)
        {
            var _author = new Author()
            {
                FullName = author.FullName
            };
            _context.Authors.Add(_author);
            _context.SaveChanges();
        }
    }
}
