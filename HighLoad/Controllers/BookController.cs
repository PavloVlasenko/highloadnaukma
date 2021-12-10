using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HighLoad.ApiModels;
using HighLoad.Entities;
using HighLoad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HighLoad.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> _logger;
        private readonly IMemoryService _redisService;
        private readonly DbContext _dbContext;
        private readonly DbSender _serviceBusSender;


        public BookController(ILogger<BookController> logger, IMemoryService redisService, DbContext dbContext, DbSender serviceBusSender)
        {
            _logger = logger;
            _redisService = redisService;
            _dbContext = dbContext;
            _serviceBusSender = serviceBusSender;
        }

        [HttpGet]
        public async Task<ActionResult<BookView>> Get(string id)
        {
            var res = await _redisService.GetAsync<Book>(id) ?? await _dbContext.Books.FindAsync(Guid.Parse(id));
            if (res == null) return NotFound();
            var view = new BookView()
            {
                Author = res.Author,
                Date = res.Date,
                Id = res.Id.ToString(),
                Name = res.Name
            };
            return Ok(view);
        }

        [HttpPost]
        public async Task<string> Create([FromBody] BookCreateData bookCreateData)
        {
            var entity = new Book()
            {
                Author = bookCreateData.Author,
                Date = bookCreateData.Date,
                Name = bookCreateData.Name,
                Id = Guid.NewGuid()
            };
            await _serviceBusSender.SendMessage(entity, HttpContext.RequestAborted);
            return entity.Id.ToString();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] BookView bookCreateData)
        {
            var entity = new Book()
            {
                Author = bookCreateData.Author,
                Date = bookCreateData.Date,
                Name = bookCreateData.Name,
                Id = Guid.Parse(bookCreateData.Id)
            };
            await _serviceBusSender.SendMessage(entity, HttpContext.RequestAborted);
            return Ok();
        }
    }
}