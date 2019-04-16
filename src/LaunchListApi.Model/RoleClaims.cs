using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Model
{
    public class RoleClaim
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string ClaimType { get; set; } // This is a STRING because it's not limited to any particular enum
        public string ClaimValue { get; set; }
    }
}
