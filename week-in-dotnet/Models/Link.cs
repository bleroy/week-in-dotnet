using System;
using System.ComponentModel.DataAnnotations;

namespace WeekInDotnet.Models
{
    public class Link
    {
        [Key]
        public string Url { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public DateTime? DatePublished { get; set; }
        public string SubmittedBy { get; set; }
    }
}
