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
            var response = await client.GetAsync("api/sessions/levels");
            if (!response.IsSuccessStatusCode)
                return View(new List<LevelSummaryViewModel>());
            var levels = await response.Content.ReadFromJsonAsync<List<LevelSummaryViewModel>>();

            var sorted = levels
                .OrderBy(l => l.LevelId.StartsWith("TrainingLevel_") ? 1 : 0)
                .ThenBy(l => l.LevelId.StartsWith("TrainingLevel_")
                    ? int.Parse(l.LevelId.Substring("TrainingLevel_".Length))
                    : 0)
                .ThenBy(l => l.LevelId)
                .ToList();
            return View(sorted);
        }
    }
}
