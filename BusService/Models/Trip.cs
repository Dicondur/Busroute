using System;
using System.Collections.Generic;

namespace BusService.Models
{
    public partial class Trip
    {
        public Trip()
        {
            TripStop = new HashSet<TripStop>();
        }

        public int TripId { get; set; }
        public int RouteScheduleId { get; set; }
        public DateTime TripDate { get; set; }
        public int DriverId { get; set; }
        public int BusId { get; set; }
        public string Comments { get; set; }

        public Bus Bus { get; set; }
        public Driver Driver { get; set; }
        public RouteSchedule RouteSchedule { get; set; }
        public ICollection<TripStop> TripStop { get; set; }
    }
}
