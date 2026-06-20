using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using my_books.Controllers;
using my_books.Data;
using my_books.Data.Model;
using my_books.Data.Services;
using my_books.Data.ViewModel;

namespace my_books_tests
{
    public class PublisherControllerTests
    {
        private static DbContextOptions<AppDbContext> dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "BooksDbControllerTest")
            .Options;

#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
        AppDbContext context;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method

        PublisherService publisherService;
        PublishersController publishersController;

        [OneTimeSetUp]
        public void Setup()
        {
            context = new AppDbContext(dbContextOptions);
            context.Database.EnsureCreated();

            SeedDatabase();
            publisherService = new PublisherService(context);
            publishersController = new PublishersController(publisherService, new NullLogger<PublishersController>());
        }

        [Test, Order(1)]
        public void HTTPGET_GetAllPublishers_WithSortBySearchStrngPageNumber_ReturnOk_Test()
        {
            IActionResult actionResult = publishersController.GetAllPublishers("Name", "P", 1);
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var actionResultData = (actionResult as OkObjectResult).Value as List<Publisher>;
            Assert.That(actionResultData.First().Name, Is.EqualTo("Publisher 1"));
            Assert.That(actionResultData.First().Id, Is.EqualTo(1));
            Assert.That(actionResultData.Count, Is.EqualTo(5));

            IActionResult actionResultSecond = publishersController.GetAllPublishers("Name", "P", 2);
            Assert.That(actionResultSecond, Is.InstanceOf<OkObjectResult>());
            var actionResultDataSecond = (actionResultSecond as OkObjectResult).Value as List<Publisher>;
            Assert.That(actionResultDataSecond.First().Name, Is.EqualTo("Publisher 6"));
            Assert.That(actionResultDataSecond.First().Id, Is.EqualTo(6));
            Assert.That(actionResultDataSecond.Count, Is.EqualTo(4));
        }

        [Test, Order(2)]
        public void HTTPGET_GetPublishersById_ReturnOk_Test()
        {
            IActionResult actionResult = publishersController.GetPublisherById(2);
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var actionResultData = (actionResult as OkObjectResult).Value as Publisher;
            Assert.That(actionResultData.Name, Is.EqualTo("Publisher 2"));
            Assert.That(actionResultData.Id, Is.EqualTo(2));
        }

        [Test, Order(3)]
        public void HTTPGET_GetPublishersById_ReturnNotFound_Test()
        {
            IActionResult actionResult = publishersController.GetPublisherById(999);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test, Order(4)]
        public void HTTPPOST_AddPublisher_ReturnCreated_Test()
        {
            var newPublisher = new PublisherVM { Name = "New Publisher" };
            IActionResult actionResult = publishersController.AddPublisher(newPublisher);
            Assert.That(actionResult, Is.TypeOf<CreatedResult>());
        }

        [Test, Order(5)]
        public void HTTPPOST_AddPublisher_ReturnBadRequest_Test()
        {
            var newPublisher = new PublisherVM { Name = "12 Publisher" };
            IActionResult actionResult = publishersController.AddPublisher(newPublisher);
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test, Order(6)]
        public void HTTPDELETE_DeletePublisher_ReturnOk_Test()
        {
            IActionResult actionResult = publishersController.DeletePublisher(2);
            Assert.That(actionResult, Is.TypeOf<OkResult>());

        }

        [Test, Order(7)]
        public void HTTPDELETE_DeletePublisher_ReturnBadRequest_Test()
        {
            IActionResult actionResult = publishersController.DeletePublisher(999);
            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
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
            context.SaveChanges();
        }
    }
}
