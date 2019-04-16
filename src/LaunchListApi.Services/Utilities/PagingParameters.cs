using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Services.Utilities
{
    public class PagingParameters
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SortColumn { get; set; }
        public bool Ascending { get; set; }
        public string Filter { get; set; }

        public PagingParameters() : this(new Dictionary<string, string>())
        { }

        public PagingParameters(Dictionary<string, string> parameters)
        {
            // Parse the page number and set to one if it's not valid or null
            int _parseTarget;
            if (parameters.ContainsKey("page") && int.TryParse(parameters["page"] ?? "1", out _parseTarget))
                Page = _parseTarget;
            else
                Page = 1;

            // Parse the page number and set to zero if it's not valid or null
            if (parameters.ContainsKey("pageSize") && int.TryParse(parameters["pageSize"] ?? "0", out _parseTarget))
                PageSize = _parseTarget;
            else
                PageSize = 0;

            SortColumn = parameters.ContainsKey("sortColumn") ? parameters["sortColumn"] : string.Empty;

            // Parse the sort direction and set to ascending if it's not valid or null
            bool sortAscending;
            if (parameters.ContainsKey("ascending") && Boolean.TryParse(parameters["ascending"], out sortAscending))
                Ascending = sortAscending;
            else
                Ascending = true;

            Filter = parameters.ContainsKey("filter") ? parameters["filter"] : null;
        }
    }
}
