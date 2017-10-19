using System;
using System.Collections.Generic;

namespace BusService.Models
{
    public partial class BusStop
    {
        public BusStop()
        {
            RouteStop = new HashSet<RouteStop>();
            TripStop = new HashSet<TripStop>();
        }

        public int BusStopNumber { get; set; }
        public string Location { get; set; }
        public int LocationHash { get; set; }
        public bool GoingDowntown { get; set; }

        public ICollection<RouteStop> RouteStop { get; set; }
        public ICollection<TripStop> TripStop { get; set; }
    }
}
