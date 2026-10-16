using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendMusic.Domain
{
    public class User
    {
        public int uId { get; set; }
        public string role { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public bool isOk { get; set; }
    }
}
