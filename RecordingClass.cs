using System;
namespace WebApplication1.Models
{
    public class Recording
    {
        public int id { get; set; }
        public DateTime start { get; set; }
        public DateTime? end { get; set; }
        public string? sid { get; set; }
        public string? ruid { get; set; }
        public string? resourceId { get; set; }
        public string ?  url { get; set; }
    }
}

