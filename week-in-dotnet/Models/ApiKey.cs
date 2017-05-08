using System.ComponentModel.DataAnnotations;

namespace WeekInDotnet.Models
{
    public class ApiKey
    {
        [Key]
        public string Key { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
    }
}
