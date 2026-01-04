using URLShortener.Models;

namespace URLShortener.Services
{
    public interface IAboutService
    {
        Task<AboutContent> GetAboutContentAsync(CancellationToken cancellationToken);
        Task UpdateAboutContentAsync(string description, CancellationToken cancellationToken);
    }
}