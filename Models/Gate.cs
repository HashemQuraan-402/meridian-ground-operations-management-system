using System;
using System.Collections.Generic;
using System.Text;

namespace MeridianGroundOperationsManagementSystem.Models
{
    public class Gate
    {
        public Gate(int id, string name, bool supportsInternational, DateTime availableFrom, DateTime availableUntil)
        {
            Id = id;
            Name = name;
            SupportsInternational = supportsInternational;
            AvailableFrom = availableFrom;
            AvailableUntil = availableUntil;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool SupportsInternational { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableUntil { get; set; }
        public List<Flight> Flights { get; set; } = new List<Flight>();

        public override string ToString()
        {
            return $"Gate {Name} (ID: {Id}) | Supports international: {SupportsInternational} | " +
                   $"Available: {AvailableFrom:yyyy-MM-dd HH:mm} to {AvailableUntil:yyyy-MM-dd HH:mm}";
        }
    }
}
