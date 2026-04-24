using GameSense.API.Data;
using GameSense.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameSense.API.Controllers
{
    //handle HTTP requests
    [ApiController]
    [Route("api/[controller]")]
    public class SessionsController: ControllerBase
    {
        private readonly GameSenseDbContext _context; //dependency injections (final)
        public SessionsController(GameSenseDbContext gameSenseDbContext) //registered in Program.cs, searched in the service container
        {
            _context = gameSenseDbContext;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var sessions = _context.Sessions.ToList(); //query sessions from the database
            return Ok(sessions);

        }
        [HttpPost]
        public IActionResult Create([FromBody] Session session)
        {

            _context.Sessions.Add(session);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetAll),new {id = session.Id}, session);
        }
    }
}
