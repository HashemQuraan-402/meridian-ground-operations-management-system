using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace MeridianGroundOperationsManagementSystem.Models
{
    public class Passenger
    {
        public Passenger(
            int id,
            string name,
            int age,
            PassengerCategory category,
            bool isTransit,
            Flight? connectingFlight)
        {
            Id = id;
            Name = name;
            Age = age;
            Category = category;
            IsTransit = isTransit;
            ConnectingFlight = connectingFlight;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public PassengerCategory Category { get; set; }
        public bool IsTransit { get; set; }
        public Flight? ConnectingFlight { get; set; }
        public List<Baggage> Baggages { get; set; } = new List<Baggage>();

        public double GetCumulativeBaggageWeight(Flight flight)
        {
            return Baggages
                .Where(baggage => baggage.Flight.Id == flight.Id)
                .Sum(baggage => baggage.Weight);
        }

        public override string ToString()
        {
            string connection = ConnectingFlight is null ? "None" : ConnectingFlight.Name;
            return $"Passenger {Name} (ID: {Id}) | Age: {Age} | Category: " +
                   $"{SystemRules.GetPassengerCategoryName(Category)} | Connecting flight: {connection}";
        }
    }
}
