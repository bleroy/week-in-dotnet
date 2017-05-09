using System.Threading.Tasks;
using WeekInDotnet.Data;
using WeekInDotnet.Models;

namespace WeekInDotnet.Services
{
    public class ApiKeyService
    {
        private LinksContext _linksContext;

        public ApiKeyService(LinksContext linksContext)
        {
            _linksContext = linksContext;
        }

        public virtual async Task<bool> Validate(string apiKey)
        {
            var key = await Find(apiKey);
            return key != null;
        }

        public virtual async Task<ApiKey> Find(string apiKey)
        {
            return await _linksContext.FindAsync<ApiKey>(apiKey);
        }
    }
}
