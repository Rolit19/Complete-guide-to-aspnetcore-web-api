using Microsoft.EntityFrameworkCore;
using my_books.Data;
using my_books.Data.Model;
using my_books.Data.Services;
using my_books.Data.ViewModel;
using my_books.Exceptions;

namespace my_books_tests
{
    public class PublishersServiceTest
    {
        private static DbContextOptions<AppDbContext> dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "BooksDbTest")
            .Options;

#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
        AppDbContext context;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method

        PublisherService publisherService;

        [OneTimeSetUp]
        public void Setup()
        {
            context = new AppDbContext(dbContextOptions);
            context.Database.EnsureCreated();

            SeedDatabase();
            publisherService = new PublisherService(context);
        }

        [Test, Order(1)]
        public void GetAllPublishers_WithNoSortBy_WithNoSearchStrng_WithNoPageNumber()
        {
            var result = publisherService.GetAllPublishers("", "", null);
            Assert.That(result.Count, Is.EqualTo(5));
        }

        [Test, Order(2)]

        public void GetAllPublishers_WithNoSortBy_WithNoSearchStrng_WithPageNumber()
        {
            var result = publisherService.GetAllPublishers("", "", 2);
            Assert.That(result.Count, Is.EqualTo(4));
        }

        [Test, Order(3)]
        public void GetAllPublishers_WithNoSortBy_WithSearchStrng_WithNoPageNumber()
        {
            var result = publisherService.GetAllPublishers("", "3", null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.FirstOrDefault().Name, Is.EqualTo("Publisher 3"));
        }

        [Test, Order(4)]
        public void GetAllPublishers_WithSortBy_WithNoSearchStrng_WithNoPageNumber()
        {
            var result = publisherService.GetAllPublishers("Name", "", null);
            Assert.That(result.Count, Is.EqualTo(5));
            Assert.That(result.FirstOrDefault().Name, Is.EqualTo("Publisher 1"));
        }

        [Test, Order(5)]
        public void GetPublisherById_WithResponse_Test()
        {
            var result = publisherService.GetPublisherById(6);
            Assert.That(result.Name, Is.EqualTo("Publisher 6"));
        }

        [Test, Order(6)]
        public void GetPublisherById_WithoutResponse_Test()
        {
            var result = publisherService.GetPublisherById(99);
            Assert.That(result, Is.Null);
        }

        [Test, Order(7)]
        public void Addpublisher_WithException_Test()
        {
            var newPublisher = new PublisherVM()
            {
                Name = "12 with Exception"
            };
            Assert.That(()=>publisherService.AddPublisher(newPublisher), 
                Throws.Exception.TypeOf<PublisherNameException>().With.Message.EqualTo("Name of the publisher cannot start with a number."));
        }

        [Test, Order(8)]
        public void Addpublisher_WithoutException_Test()
        {
            var newPublisher = new PublisherVM()
            {
                Name = "Publisher 10"
            };
            var result = publisherService.AddPublisher(newPublisher);
            Assert.That(result, Is.Not.Null);
        }

        [Test, Order(9)]
        public void GetPublisherData_Test()
        {
            var result = publisherService.GetPublisherData(1);
            Assert.That(result.Name, Is.EqualTo("Publisher 1"));
            Assert.That(result.BookAuthors, Is.Not.Empty);
            Assert.That(result.BookAuthors.Count(), Is.GreaterThan(0));
            Assert.That(result.BookAuthors.OrderBy(a => a.BookName).FirstOrDefault().BookName, Is.EqualTo("The Hobbit"));
        }

        [Test, Order(10)]
        public void DeletePublisherById_WithResponse_Test()
        {
            publisherService.DeletePublisherById(1);
            Assert.That(context.Publishers.Count(), Is.EqualTo(8));
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            context.Database.EnsureDeleted();
        }

        private void SeedDatabase()
        {
            var publishers = new List<Publisher>
            {
                new Publisher { Id=1, Name = "Publisher 1" },
                new Publisher { Id=2, Name = "Publisher 2" },
                new Publisher { Id=3, Name = "Publisher 3" },
                new Publisher { Id=4, Name = "Publisher 4" },
                new Publisher { Id=5, Name = "Publisher 5" },
                new Publisher { Id=6, Name = "Publisher 6" },
                new Publisher { Id=7, Name = "Publisher 7" },
                new Publisher { Id=8, Name = "Publisher 8" },
                new Publisher { Id=9, Name = "Publisher 9" }
            };
            context.Publishers.AddRange(publishers);

            var authors = new List<Author>
            {
                new Author { Id=1, FullName = "Author 1" },
                new Author { Id=2, FullName = "Author 2" }
            };
            context.Authors.AddRange(authors);

            var books = new List<Book>
            {
                new Book()
                    {
                        Id=1,
                        Title = "The Lord of the Rings",
                        Description = "An epic high-fantasy novel written by English author and scholar J. R. R. Tolkien.",
                        IsRead = true,
                        DateRead = DateTime.Now.AddDays(-10),
                        Genre = "Fantasy",
                        CoverUrl = "https://upload.wikimedia.org/wikipedia/en/8/8e/The_Lord_of_the_Rings_cover.gif",
                        DateAdded = DateTime.Now,
                        PublisherId = 1
                    },
                new Book()
                    {
                        Id=2,
                        Title = "The Hobbit",
                        Description = "A children's fantasy novel by English author J. R. R. Tolkien.",
                        IsRead = false,
                        Genre = "Fantasy",
                        CoverUrl = "https://upload.wikimedia.org/wikipedia/en/4/4a/TheHobbit_FirstEdition.jpg",
                        DateAdded = DateTime.Now,
                        PublisherId = 1
                    }
            };
            context.Books.AddRange(books);

            var book_authors = new List<Book_Author>
            {
                new Book_Author { Id=1, BookId =1, AuthorId = 1 },
                new Book_Author { Id=2, BookId =1, AuthorId = 2 },
                new Book_Author { Id=3, BookId =2, AuthorId = 2 }
            };

            context.Book_Authors.AddRange(book_authors);
            context.SaveChanges();
        }
    }
}
