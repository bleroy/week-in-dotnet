using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeekInDotnet.Data;
using WeekInDotnet.Models;

namespace WeekInDotnet.Services
{
    public class LinksService
    {
        private LinksContext _linksContext;
        private LinksSettings _settings;

        public LinksService(LinksContext linksContext, IOptions<LinksSettings> settings)
        {
            _linksContext = linksContext;
            _settings = settings.Value;
        }

        public virtual async Task<IEnumerable<string>> GetCategories() =>
            await Task.FromResult(_settings.Categories.AsEnumerable());

        public virtual async Task<IDictionary<string, IList<Link>>> GetUnpublishedLinks() =>
            await _linksContext.Links
                .Where(link => link.DatePublished == null)
                .GroupBy(link => link.Category)
                .ToDictionaryAsync(
                    group => group.Key,
                    group => (IList<Link>)group.ToList()
                );

        public virtual async Task Add(string url)
        {
            var existing = await _linksContext.FindAsync<Link>(url);
            if (existing == null)
            {
                await _linksContext.AddAsync(new Link { Url = url });
                await _linksContext.SaveChangesAsync();
            }
        }

        public virtual async Task Add(string url, string title, string author)
        {
            var existing = await _linksContext.FindAsync<Link>(url);
            if (existing != null)
            {
                existing.Title = title;
                existing.Author = author;
            }
            else
            {
                await _linksContext.AddAsync(new Link
                {
                    Url = url,
                    Title = title,
                    Author = author
                });
            }
            await _linksContext.SaveChangesAsync();
        }
    }
}
