using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WeekInDotnet.Services;
using System.Threading.Tasks;
using WeekInDotnet.ViewModels;
using Microsoft.AspNetCore.Cors;
using System.Security.Cryptography;
using System;

namespace WeekInDotnet.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
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

        // GET api/links/categories
        [HttpGet("categories")]
        public async Task<IEnumerable<string>> GetCategories() =>
            await _linksService.GetCategories();

        // POST api/links/add
        [HttpPost("add")]
        public async Task<IActionResult> Add(AddLinkUpdateModel addLinkUpdateModel)
        {
            if (!await _apiKeyService.Validate(addLinkUpdateModel.ApiKey))
            {
                TempData["error"] = "No valid API key was submitted with this link.";
            }
            await _linksService.Add(addLinkUpdateModel.Url, addLinkUpdateModel.Title, addLinkUpdateModel.Author);
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
