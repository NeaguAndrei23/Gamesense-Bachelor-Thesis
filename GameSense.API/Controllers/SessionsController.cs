using GameSense.API.Data;
using GameSense.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameSense.API.Controllers
{
    //handle HTTP requests
    [ApiController]
    [Route("api/[controller]")]
    public class SessionsController : ControllerBase
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
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var session = _context.Sessions.FirstOrDefault(s => s.Id == id);
            if (session == null)
            {
                return NotFound();
            }
            return Ok(session);
            
        }

        [HttpGet("level/{levelId}")]
        public IActionResult Get(string levelId)
        {
            var sessions = _context.Sessions.Where(s => s.LevelId == levelId).ToList();
            if (!sessions.Any())
            {
                return NotFound();
            }
            return Ok(sessions);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Session session)
        {

            _context.Sessions.Add(session);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetAll), new { id = session.Id }, session);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var session = _context.Sessions.FirstOrDefault(s => s.Id==id);
            if (session == null)
            {
                return NotFound();
            }
            _context.Sessions.Remove(session);
            _context.SaveChanges();
            return NoContent();
        }

    }
}
