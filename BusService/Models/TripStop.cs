using System;
using System.Collections.Generic;

namespace BusService.Models
{
    public partial class TripStop
    {
        public int TripStopId { get; set; }
        public int TripId { get; set; }
        public int BusStopNumber { get; set; }
        public TimeSpan TripStopTime { get; set; }
        public string Comments { get; set; }

        public BusStop BusStopNumberNavigation { get; set; }
        public Trip Trip { get; set; }
    }
}
