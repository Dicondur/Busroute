using System;
using System.Collections.Generic;

namespace BusService.Models
{
    public partial class BusRoute
    {
        public BusRoute()
        {
            RouteSchedule = new HashSet<RouteSchedule>();
            RouteStop = new HashSet<RouteStop>();
        }

        public string BusRouteCode { get; set; }
        public string RouteName { get; set; }

        public ICollection<RouteSchedule> RouteSchedule { get; set; }
        public ICollection<RouteStop> RouteStop { get; set; }
    }
}
