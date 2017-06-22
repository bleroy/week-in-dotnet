using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeekInDotnet.Services;
using WeekInDotnet.ViewModels;

namespace WeekInDotnet.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private CaptchaService _captcha;
        private LinksService _links;

        public HomeController(CaptchaService captcha, LinksService links)
        {
            _captcha = captcha;
            _links = links;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                BaseUrl = $"{Request.Scheme}://{Request.Host}",
                LinkServiceUrl = Url.Action(nameof(LinksController.Add), "Links"),

                CaptchaPublicKey = _captcha.Settings.PublicKey,
                Notification = TempData["notification"] as string,
                Error = TempData["error"] as string
            };
            return View(model);
        }

        [HttpPost]
        [Route("/add")]
        public async Task<IActionResult> AddUrl(string url, [ModelBinder(Name = "g-recaptcha-response")] string captcha)
        {
            if (!string.IsNullOrWhiteSpace(url) && !string.IsNullOrWhiteSpace(captcha))
            {
                if (await _captcha.Validate(captcha))
                {
                    await _links.Add(url);
                    TempData["notification"] = "Thanks for submitting a new link for The Week in .NET.";
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}