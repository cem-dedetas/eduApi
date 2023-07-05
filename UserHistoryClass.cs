//userclass is a class that contains all the information about a user that links with dbcontext


// Path: UserClass.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class UserHistory
    {
        public int id { get; set; }
        public User user { get; set; }
        public LiveLectureClass liveLecture { get; set; }
        public DateTime joinDate { get; set; }
        public DateTime ? lastPingDate { get; set; }
    }

}
