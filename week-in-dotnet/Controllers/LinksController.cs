using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WeekInDotnet.Services;
using System.Threading.Tasks;
using WeekInDotnet.ViewModels;
using System.Security.Cryptography;
using System;
using System.Text;

namespace WeekInDotnet.Controllers
{
    [Route("api/[controller]")]
    public class LinksController : Controller
    {
        private LinksService _linksService;
        private ApiKeyService _apiKeyService;

        private static RandomNumberGenerator _random = RandomNumberGenerator.Create();

        public LinksController(LinksService linksService, ApiKeyService apiKeyService)
        {
            _linksService = linksService;
            _apiKeyService = apiKeyService;
        }

        // GET api/links
        [HttpGet]
        public async Task<string> Get()
        {
            var output = new StringBuilder();
            var links = await _linksService.GetUnpublishedLinks();
            foreach(var category in await _linksService.GetCategories())
            {
                if (!links.ContainsKey(category)) continue;
                output.AppendLine();
                output.AppendLine($"## {category}");
                output.AppendLine();
                foreach(var link in links[category])
                {
                    output.AppendLine($"* [{link.Title}]({link.Url}) by {link.Author}.");
                }
            }
            return output.ToString();
        }

        // GET api/links/categories
        [HttpGet("categories")]
        public async Task<IEnumerable<string>> GetCategories() =>
            await _linksService.GetCategories();

        // POST api/links/add
        [HttpPost("add")]
        public async Task<IActionResult> Add(AddLinkUpdateModel addLinkUpdateModel)
        {
            var submitter = await _apiKeyService.Find(addLinkUpdateModel.ApiKey);
            if (submitter == null)
            {
                TempData["error"] = "No valid API key was submitted with this link.";
            }
            await _linksService.Add(addLinkUpdateModel.Url, addLinkUpdateModel.Title, addLinkUpdateModel.Author, addLinkUpdateModel.Category, submitter.OwnerName);
            TempData["notification"] = "Thanks for submitting a new link for The Week in .NET.";
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        // GET api/links/newkey
        [HttpGet("newkey")]
        public string NewKey()
        {
            var key = new byte[64];
            _random.GetBytes(key);
            var encoded = Convert.ToBase64String(key);
            return encoded.Substring(0, encoded.Length - 2);
        }
    }
}
