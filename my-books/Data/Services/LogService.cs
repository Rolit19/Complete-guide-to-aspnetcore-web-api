using my_books.Data.Model;

namespace my_books.Data.Services
{
    public class LogService
    {
        private AppDbContext _context;
        public LogService(AppDbContext context)
        {
            _context = context;
        }

        public List<Log> GetAllLogsFromDb()
        {
            return _context.Logs.ToList();
        }
    }
}
