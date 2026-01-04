using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Models;
using URLShortener.Services; 

namespace URLShortener.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUrlApiService _urlService;
        private readonly IAboutService _aboutService; 

        public HomeController(IUrlApiService urlService, IAboutService aboutService)
        {
            _urlService = urlService;
            _aboutService = aboutService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // about page
        public async Task<IActionResult> Privacy(CancellationToken cancellationToken)
        {
            var content = await _aboutService.GetAboutContentAsync(cancellationToken);
            return View(content);
        }

        // update about page
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAbout(string description, CancellationToken cancellationToken)
        {
            await _aboutService.UpdateAboutContentAsync(description, cancellationToken);
            return RedirectToAction("Privacy");
        }

        [HttpGet("/{shortCode}")]
        public async Task<IActionResult> RedirectToUrl(string shortCode)
        {
            if (string.IsNullOrEmpty(shortCode)) return RedirectToAction("Index");

            var shortUrl = await _urlService
                .GetByPathAsync(shortCode, CancellationToken.None);

            if (shortUrl == null) return NotFound();

            return Redirect(shortUrl.OriginalUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}