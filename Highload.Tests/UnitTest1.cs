using System;
using System.Linq;
using System.Threading;
using HighLoad;
using HighLoad.ApiModels;
using HighLoad.Controllers;
using HighLoad.Entities;
using HighLoad.EventHandlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using DbContext = HighLoad.DbContext;

namespace Highload.Tests
{
    public class Tests
    {
        private IServiceProvider Services = null!;
        [SetUp]
        public void Setup()
        {

            var services = Program.CreateHostBuilder<Highload.Tests.Tests>(Array.Empty<string>()).Build().Services;
            var context = services.GetService<DbContext>();
            context!.Database.EnsureCreated();
            context!.Database.ExecuteSqlRaw("truncate table [dbo].[Books]");
            Services = services;
        }

        [Test]
        public void BookDbEventHandler_NewBook_ShouldBeSavedInDb()
        {
            var handler = Services.GetService<BookDbEventHandler>();
            var message = GetBook();
            handler!.Handle(message, CancellationToken.None).GetAwaiter().GetResult();
            var context = Services.GetService<DbContext>();
            var actual = context!.Books.Single();
            Assert.AreEqual(message, actual);
        }

        [Test]
        public void BookController_Get_BookExists_ShouldReturnBook()
        {
            var context = Services.GetService<DbContext>();
            var book = GetBook();
            context!.Books.Add(book);

            var controller = Services.GetService<BookController>();
            var actualResult = controller!.Get(book.Id.ToString()).GetAwaiter().GetResult().Result as OkObjectResult;
            var actual = actualResult!.Value as BookView;
            
            Assert.AreEqual(book.Author, actual!.Author);
            Assert.AreEqual(book.Date, actual.Date);
            Assert.AreEqual(book.Id.ToString(), actual.Id);
            Assert.AreEqual(book.Name, actual.Name);
        }

        private Book GetBook() => new()
        {
            Author = "bla",
            Date = DateTime.Now,
            Id = Guid.NewGuid(),
            Name = "name"
        };

    }
}