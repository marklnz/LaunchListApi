using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Model
{
    public enum EntityStatus
    {
        New = 1,
        Active = 2,
        Suspended = 3,
        Cancelled = 4 

        // TODO: Determine if there are any to add to this list - or take away (i.e."New". "Deceased")
    }
}
