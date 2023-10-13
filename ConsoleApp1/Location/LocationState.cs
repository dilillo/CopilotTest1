using ConsoleApp1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Location
{
    public class LocationState
    {
        public LocationProfile Profile { get; set; }

        public LocationAddress Address { get; set; }

        public List<DayOfOperation> Hours { get; set; }
    }
}