using GameSense.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameSense.Web.Controllers
{
    public class LevelConfigController : Controller
    {
        private readonly IHttpClientFactory _client;

        public LevelConfigController(IHttpClientFactory client)
        {
            _client = client;
        }

        // GET LevelConfig/Configure/{levelId} — show config form
        public async Task<IActionResult> Configure(string levelId)
        {
            var client = _client.CreateClient("GameSenseAPI");

            // fetch existing config if any
            var response = await client.GetAsync($"api/levelconfig/{levelId}");
            var config = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<LevelConfigViewModel>()
                : null;

            ViewBag.LevelId = levelId;
            ViewBag.Config = config;
            return View(config);
        }

        // POST LevelConfig/Configure/{levelId} — submit config form
        [HttpPost]
        public async Task<IActionResult> Configure(string levelId, string gameName, string expectedDifficulty)
        {
            var client = _client.CreateClient("GameSenseAPI");

            // check if config already exists
            var checkResponse = await client.GetAsync($"api/levelconfig/{levelId}");

            if (checkResponse.IsSuccessStatusCode)
            {
                // update existing config
                var updatePayload = new { expectedDifficulty, gameName };
                var updateContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(updatePayload),
                    System.Text.Encoding.UTF8,
                    "application/json");
                await client.PutAsync($"api/levelconfig/{levelId}", updateContent);
            }
            else
            {
                // create new config
                var createPayload = new { levelId, gameName, expectedDifficulty };
                var createContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(createPayload),
                    System.Text.Encoding.UTF8,
                    "application/json");
                await client.PostAsync("api/levelconfig", createContent);
            }

            // redirect back to level detail page
            return RedirectToAction("Index", "Sessions");
        }
    }
}