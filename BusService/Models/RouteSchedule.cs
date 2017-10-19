using System;
using System.Collections.Generic;

namespace BusService.Models
{
    public partial class RouteSchedule
    {
        public RouteSchedule()
        {
            Trip = new HashSet<Trip>();
        }

        public int RouteScheduleId { get; set; }
        public string BusRouteCode { get; set; }
        public TimeSpan StartTime { get; set; }
        public bool IsWeekDay { get; set; }
        public string Comments { get; set; }

        public BusRoute BusRouteCodeNavigation { get; set; }
        public ICollection<Trip> Trip { get; set; }
    }
}
