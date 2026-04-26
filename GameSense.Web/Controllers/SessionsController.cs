using GameSense.Web.Models;
using Microsoft.AspNetCore.Mvc;


namespace GameSense.Web.Controllers
{
    public class SessionsController : Controller
    {
        private readonly IHttpClientFactory _client;

        public SessionsController(IHttpClientFactory client)
        {
            _client = client;
        }

        public async Task<IActionResult> Index()
        {
            var client = _client.CreateClient("GameSenseAPI");
            var response = await client.GetAsync("api/sessions");
            if (!response.IsSuccessStatusCode)
                return View(new List<SessionViewModel>());

            var sessions = await response.Content.ReadFromJsonAsync<List<SessionViewModel>>(); //deserealized list of JSON objects
            return View(sessions);
        }
    }
}
