//userclass is a class that contains all the information about a user that links with dbcontext


// Path: UserClass.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string id_number { get; set; }
        public byte[] password_hash { get; set; }
        public byte[] password_salt { get; set; }
        public string email { get; set; }
        public string ? token { get; set; }
    }

    public class UserSignUpDTO
    {
        public string username { get; set; }
        public string password { get; set; }
        public string id_number { get; set; }
        public string email { get; set; }
    }

    public class UserSignInDTO
    {
        public string? id_number { get; set; }
        public string? email { get; set; }
        public string password { get; set; }
    }
}
