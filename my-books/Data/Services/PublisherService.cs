using my_books.Data.Model;
using my_books.Data.ViewModel;

namespace my_books.Data.Services
{
    public class PublisherService
    {
        private AppDbContext _context;  
        public PublisherService(AppDbContext context)
        {
            _context = context;
        }

        public void AddPublisher(PublisherVM publisher)
        {
            var _publisher = new Publisher()
            {
                Name = publisher.Name
            };
            _context.Publishers.Add(_publisher);
            _context.SaveChanges();
        }
    }
}
