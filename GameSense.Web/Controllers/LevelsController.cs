using GameSense.Web.Models;
using Microsoft.AspNetCore.Mvc;
namespace GameSense.Web.Controllers
{
    public class LevelsController : Controller
    {
        private readonly IHttpClientFactory _client;
        public LevelsController(IHttpClientFactory client)
        {
            _client = client;
        }
        // GET Levels/Details/{levelId} — fetches session and zone data for a specific level
        public async Task<IActionResult> Details(string levelId)
        {
            var client = _client.CreateClient("GameSenseAPI");

            // fetch all sessions for this level — critical, return early with empty list if it fails
            var sessionResponse = await client.GetAsync($"api/sessions/level/{levelId}");
            if (!sessionResponse.IsSuccessStatusCode)
                return View(new List<SessionViewModel>());

            var sessions = await sessionResponse.Content.ReadFromJsonAsync<List<SessionViewModel>>();

            // fetch zone death stats — not critical, use empty list if it fails so sessions still display
            var zoneResponse = await client.GetAsync($"api/deathevents/zones/{levelId}");
            var zoneStats = zoneResponse.IsSuccessStatusCode
                ? await zoneResponse.Content.ReadFromJsonAsync<List<ZoneStatViewModel>>()
                : new List<ZoneStatViewModel>();

            ViewBag.ZoneStats = zoneStats;
            ViewBag.LevelId = levelId;
            return View(sessions);
        }
    }
}