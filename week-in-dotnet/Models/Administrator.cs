using System.ComponentModel.DataAnnotations;

namespace WeekInDotnet.Models
{
    public class Administrator
    {
        [Key]
        public string Email { get; set; }
        public string Name { get; set; }
        public string Roles { get; set; }
    }
}
