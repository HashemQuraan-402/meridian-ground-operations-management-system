using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml.Linq;

namespace MeridianGroundOperationsManagementSystem.Models
{
    public class Baggage
    {
        public Baggage(int id, BaggageType type, double weight, Passenger passenger, Flight flight)
        {
            Id = id;
            Type = type;
            Weight = weight;
            Passenger = passenger;
            Flight = flight;
        }

        public int Id { get; set; }
        public BaggageType Type { get; set; }
        public double Weight { get; set; }
        public Passenger Passenger { get; set; }
        public Flight Flight { get; set; }

        public override string ToString()
        {
            return $"Baggage {Id} | {Type} | {Weight:0.##} kg | " +
                   $"Passenger: {Passenger.Name} | Flight: {Flight.Name}";
        }
    }
}
