//userclass is a class that contains all the information about a user that links with dbcontext


// Path: Tag.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class ChatMessage
    {
        public int id { get; set; }
        public User sender { get; set; }
        public string content { get; set; }
        public DateTime timeStamp { get; set; }
    }

    public class MessageDTO
    {
        public string text { get; set; }
        public string sender { get; set; }
        public DateTime timestamp { get; set; }
    }
}
