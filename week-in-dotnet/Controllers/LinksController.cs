using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WeekInDotnet.Models;
using WeekInDotnet.Services;
using System.Threading.Tasks;

namespace WeekInDotnet.Controllers
{
    [Route("api/[controller]")]
    public class LinksController : Controller
    {
        private LinksService _linksService;

        public LinksController(LinksService linksService)
        {
            _linksService = linksService;
        }

        // GET api/links
        [HttpGet]
        public async Task<IDictionary<string, IList<Link>>> Get() =>
            await _linksService.GetUnpublishedLinks();

        // GET api/links/categories
        [HttpGet("categories")]
        public async Task<IEnumerable<string>> GetCategories() =>
            await _linksService.GetCategories();
    }
}
