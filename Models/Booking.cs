using System;
using System.Collections.Generic;
using System.Text;

namespace MeridianGroundOperationsManagementSystem.Models
{
    public class Booking
    {
        public Booking(
            int id,
            Passenger passenger,
            Flight flight,
            BookingStatus status,
            int? seatNumber,
            DateTime bookingTime)
        {
            Id = id;
            Passenger = passenger;
            Flight = flight;
            Status = status;
            SeatNumber = seatNumber;
            BookingTime = bookingTime;
        }

        public int Id { get; set; }
        public int? SeatNumber { get; set; }
        public Passenger Passenger { get; set; }
        public Flight Flight { get; set; }
        public BookingStatus Status { get; set; }
        public bool IsBoarded { get; set; }
        public DateTime BookingTime { get; set; }

        public override string ToString()
        {
            string seat = SeatNumber.HasValue ? SeatNumber.Value.ToString() : "Not assigned";
            return $"Booking {Id} | Passenger: {Passenger.Name} | Flight: {Flight.Name} | " +
                   $"Status: {Status} | Seat: {seat} | Boarded: {IsBoarded}";
        }
    }
}
