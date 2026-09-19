using MeridianGroundOperationsManagementSystem.Exceptions;
using MeridianGroundOperationsManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeridianGroundOperationsManagementSystem.Services
{
    public static class BookingManager
    {
        public static Booking BookPassengerSeat(int bookingId, Passenger passenger, Flight flight)
        {
            if (flight.Direction != FlightDirection.Departing)
            {
                throw new MeridianSystemException("Passengers can only be booked onto departing flights.");
            }

            if (flight.Status == FlightStatus.Cancelled ||
                flight.Status == FlightStatus.Departed ||
                flight.Status == FlightStatus.Arrived)
            {
                throw new MeridianSystemException(
                    $"A booking cannot be created because flight {flight.Name} has status {flight.Status}.");
            }

            bool alreadyBooked = Program.Bookings.Any(
                booking => booking.Passenger.Id == passenger.Id &&
                           booking.Flight.Id == flight.Id &&
                           booking.Status != BookingStatus.Cancelled);

            if (alreadyBooked)
            {
                throw new MeridianSystemException(
                    $"Passenger {passenger.Name} already has an active booking on flight {flight.Name}.");
            }

            if (flight.ConfirmedBookings.Count < flight.SeatCapacity)
            {
                int seatNumber = GetNextAvailableSeatNumber(flight);
                Booking confirmedBooking = new Booking(
                    bookingId,
                    passenger,
                    flight,
                    BookingStatus.Confirmed,
                    seatNumber,
                    DateTime.Now);

                flight.ConfirmedBookings.Add(confirmedBooking);
                return confirmedBooking;
            }

            if (flight.StandbyBookings.Count >= SystemRules.StandbyLimit)
            {
                throw new MeridianSystemException(
                    $"Booking rejected: flight {flight.Name} is full and its standby list has reached " +
                    $"the limit of {SystemRules.StandbyLimit} passengers.");
            }

            Booking standbyBooking = new Booking(
                bookingId,
                passenger,
                flight,
                BookingStatus.Standby,
                null,
                DateTime.Now);

            flight.StandbyBookings.Enqueue(standbyBooking);
            return standbyBooking;
        }

        public static Booking? CancelConfirmedBooking(Passenger passenger, Flight flight)
        {
            Booking? booking = flight.ConfirmedBookings.FirstOrDefault(
                confirmed => confirmed.Passenger.Id == passenger.Id &&
                             confirmed.Status == BookingStatus.Confirmed);

            if (booking is null)
            {
                throw new MeridianSystemException(
                    $"Passenger {passenger.Name} does not have a confirmed booking on flight {flight.Name}.");
            }

            if (booking.IsBoarded)
            {
                throw new MeridianSystemException("A booking cannot be cancelled after the passenger has boarded.");
            }

            int freedSeatNumber = booking.SeatNumber ?? 0;
            booking.Status = BookingStatus.Cancelled;
            booking.SeatNumber = null;
            flight.ConfirmedBookings.Remove(booking);

            if (flight.StandbyBookings.Count == 0)
            {
                return null;
            }

            Booking promotedBooking = flight.StandbyBookings.Dequeue();
            promotedBooking.Status = BookingStatus.Confirmed;
            promotedBooking.SeatNumber = freedSeatNumber;
            flight.ConfirmedBookings.Add(promotedBooking);
            return promotedBooking;
        }

        private static int GetNextAvailableSeatNumber(Flight flight)
        {
            for (int seatNumber = 1; seatNumber <= flight.SeatCapacity; seatNumber++)
            {
                bool isUsed = flight.ConfirmedBookings.Any(
                    booking => booking.SeatNumber == seatNumber &&
                               booking.Status == BookingStatus.Confirmed);

                if (!isUsed)
                {
                    return seatNumber;
                }
            }

            throw new MeridianSystemException("No confirmed seat is available on this flight.");
        }
    }
}
