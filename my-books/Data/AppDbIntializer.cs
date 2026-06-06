using my_books.Data.Model;

namespace my_books.Data
{
    public class AppDbIntializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                if (!context.Books.Any())
                {
                    context.Books.AddRange(new Book()
                    {
                        Title = "The Lord of the Rings",
                        Description = "An epic high-fantasy novel written by English author and scholar J. R. R. Tolkien.",
                        IsRead = true,
                        DateRead = DateTime.Now.AddDays(-10),
                        Genre = "Fantasy",
                        Author = "J.R.R. Tolkien",
                        CoverUrl = "https://upload.wikimedia.org/wikipedia/en/8/8e/The_Lord_of_the_Rings_cover.gif",
                        DateAdded = DateTime.Now
                    },
                    new Book()
                    {
                        Title = "The Hobbit",
                        Description = "A children's fantasy novel by English author J. R. R. Tolkien.",
                        IsRead = false,
                        Author = "J.R.R. Tolkien",
                        Genre = "Fantasy",
                        CoverUrl = "https://upload.wikimedia.org/wikipedia/en/4/4a/TheHobbit_FirstEdition.jpg",
                        DateAdded = DateTime.Now
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
