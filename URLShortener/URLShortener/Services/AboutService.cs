using Microsoft.EntityFrameworkCore;
using URLShortener.Data;
using URLShortener.Models;

namespace URLShortener.Services
{
    public class AboutService : IAboutService
    {
        private readonly AppDbContext _context;

        public AboutService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AboutContent> GetAboutContentAsync(CancellationToken cancellationToken)
        {
            var content = await _context
                .AboutContents
                .FirstOrDefaultAsync(cancellationToken);

            // if not exists, create with default desc
            if (content == null)
            {
                content = new AboutContent
                {
                    Description = "URL Shortener uses a random alphanumeric generation algorithm. It creates a unique 6-character code from a set of 62 characters (A-Z, a-z, 0-9), that gives over 56 billion possible combinations."
                };
                _context.AboutContents.Add(content);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return content;
        }

        public async Task UpdateAboutContentAsync(string description, CancellationToken cancellationToken)
        {
            var content = await _context
                .AboutContents
                .FirstOrDefaultAsync(cancellationToken);

            if (content != null)
            {
                content.Description = description;
                _context.AboutContents.Update(content);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}