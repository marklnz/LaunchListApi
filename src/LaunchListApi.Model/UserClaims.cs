using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Model
{
    public class UserClaim
    { 
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ClaimType { get; set; } // This is a STRING because it's not limited to any particular enum
        public string ClaimValue { get; set; }
    }
}
