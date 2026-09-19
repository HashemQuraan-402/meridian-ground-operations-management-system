using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace MeridianGroundOperationsManagementSystem.Models
{
    public class Flight
    {
        public Flight(
            int id,
            string name,
            FlightType type,
            DateTime departureTime,
            DateTime arrivalTime,
            FlightDirection direction,
            int seatCapacity)
        {
            Id = id;
            Name = name;
            Type = type;
            DepartureTime = departureTime;
            ArrivalTime = arrivalTime;
            Direction = direction;
            SeatCapacity = seatCapacity;
            Status = FlightStatus.Scheduled;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public FlightType Type { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public FlightDirection Direction { get; set; }
        public Gate? Gate { get; set; }
        public int SeatCapacity { get; set; }
        public FlightStatus Status { get; set; }
        public List<Booking> ConfirmedBookings { get; set; } = new List<Booking>();
        public Queue<Booking> StandbyBookings { get; set; } = new Queue<Booking>();

        public DateTime GateOccupancyStart
        {
            get
            {
                if (Direction == FlightDirection.Departing)
                {
                    return DepartureTime.AddMinutes(-SystemRules.DepartingGateMinutesBefore);
                }

                return ArrivalTime.AddMinutes(-SystemRules.ArrivingGateMinutesBefore);
            }
        }

        public DateTime GateOccupancyEnd
        {
            get
            {
                if (Direction == FlightDirection.Departing)
                {
                    return DepartureTime.AddMinutes(SystemRules.DepartingGateMinutesAfter);
                }

                return ArrivalTime.AddMinutes(SystemRules.ArrivingGateMinutesAfter);
            }
        }

        public override string ToString()
        {
            string gateName = Gate is null ? "Not assigned" : Gate.Name;
            return $"Flight {Name} (ID: {Id}) | {Type} | {Direction} | " +
                   $"Departure: {DepartureTime:yyyy-MM-dd HH:mm} | Arrival: {ArrivalTime:yyyy-MM-dd HH:mm} | " +
                   $"Gate: {gateName} | Seats: {SeatCapacity} | Status: {Status}";
        }
    }
}
