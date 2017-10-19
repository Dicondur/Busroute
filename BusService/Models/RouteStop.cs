using System;
using System.Collections.Generic;

namespace BusService.Models
{
    public partial class RouteStop
    {
        public int RouteStopId { get; set; }
        public string BusRouteCode { get; set; }
        public int? BusStopNumber { get; set; }
        public int? OffsetMinutes { get; set; }

        public BusRoute BusRouteCodeNavigation { get; set; }
        public BusStop BusStopNumberNavigation { get; set; }
    }
}
