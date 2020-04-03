using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.ResourceParameters
{
    public class TerminalResourceParameters
    {
        const int maxPageSize = 20;
        public Guid SiteId { get; set; }

        public string TerminalName { get; set; }

        public string SearchQuery { get; set; }

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string OrderBy { get; set; } = "TerminalName";

        public string Fields { get; set; }
    }
}
