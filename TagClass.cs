//userclass is a class that contains all the information about a user that links with dbcontext


// Path: Tag.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Tag
    {
        public int id { get; set; }
        public string tag_name { get; set; }
    }

    public class TagDTO
    {
        public string tag_name { get; set; }
    }
}
