using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WeekInDotnet.Models;

namespace WeekInDotnet.Services
{
    public class CaptchaService
    {
        public CaptchaSettings Settings {get;set;}

        public CaptchaService(IOptions<CaptchaSettings> settings)
        {
            Settings = settings.Value;
        }

        public virtual async Task<bool> Validate(string token)
        {
            var client = new HttpClient();
            var response = await client.PostAsync(
                "https://www.google.com/recaptcha/api/siteverify",
                new FormUrlEncodedContent(new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("secret", Settings.PrivateKey),
                    new KeyValuePair<string, string>("response", token)
                }));
            if (!response.IsSuccessStatusCode) return false;
            return JToken
                .Parse(await response.Content.ReadAsStringAsync())
                .Value<bool>("success");
        }
    }
}
