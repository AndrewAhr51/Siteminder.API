using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.ResourceParameters
{
    public class TankResourceParameters
    {
        const int maxPageSize = 20;
        public string Type { get; set; }
        public Guid TermimalId { get; set; }
        public Guid DispenserId { get; set; }
        public Guid FuelTypeId { get; set; }
        public string SearchQuery { get; set; }
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string OrderBy { get; set; } = "Name";

        public string Fields { get; set; }
    }
}
