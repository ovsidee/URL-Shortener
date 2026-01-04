using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Models.DTO; 
using URLShortener.Services; 

namespace URLShortener.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class UrlApiController : ControllerBase
    {
        private readonly IUrlApiService _urlService;
        private readonly UserManager<IdentityUser> _userManager; // ADDED: Needed for Role checks
        
        public UrlApiController(IUrlApiService urlService, UserManager<IdentityUser> userManager)
        {
            _urlService = urlService;
            _userManager = userManager;
        }

        // tells Angular who is logged in and if they are Admin
        [HttpGet("me")]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            
            // Anonymous (Not logged in)
            if (user == null) 
            {
                return Ok(new { IsAuthenticated = false, Username = "", IsAdmin = false });
            }

            // Logged In
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            
            return Ok(new { 
                IsAuthenticated = true, 
                Username = user.UserName, 
                IsAdmin = isAdmin 
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetUrls(CancellationToken cancellationToken)
        {
            var urls = await _urlService.GetAllUrlsAsync(cancellationToken);
            return Ok(urls);
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddUrl([FromBody] UrlDto urlDto, CancellationToken cancellationToken) 
        {
            if (urlDto == null || string.IsNullOrWhiteSpace(urlDto.Url))
                return BadRequest("URL cannot be empty.");

            try
            {
                var result = await _urlService.AddUrlAsync(urlDto.Url, User, cancellationToken);
                return Ok(result);
            }
            catch (InvalidOperationException ex) // handles "URL already exists"
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while creating the URL.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUrl(int id, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _urlService.DeleteUrlAsync(id, User, cancellationToken);
                if (!success) return NotFound();
                
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}