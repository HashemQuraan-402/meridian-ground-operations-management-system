using MeridianGroundOperationsManagementSystem.Models;
using MeridianGroundOperationsManagementSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeridianGroundOperationsManagementSystem.Services
{
    public static class BoardingManager
    {
        public static string CheckBoardingEligibility(Passenger passenger, Flight flight)
        {
            if (flight.Direction != FlightDirection.Departing)
            {
                throw new MeridianSystemException("Passengers can only board a departing flight.");
            }

            if (flight.Status == FlightStatus.Cancelled ||
                flight.Status == FlightStatus.Departed ||
                flight.Status == FlightStatus.Arrived)
            {
                throw new MeridianSystemException(
                    $"BOARDING DENIED: Flight {flight.Name} has status {flight.Status}.");
            }

            Booking? booking = flight.ConfirmedBookings.FirstOrDefault(
                confirmed => confirmed.Passenger.Id == passenger.Id &&
                             confirmed.Status == BookingStatus.Confirmed);

            if (booking is null)
            {
                throw new MeridianSystemException(
                    $"BOARDING DENIED: Passenger {passenger.Name} does not have a confirmed booking on flight {flight.Name}.");
            }

            if (booking.IsBoarded)
            {
                throw new MeridianSystemException(
                    $"BOARDING DENIED: Passenger {passenger.Name} has already boarded flight {flight.Name}.");
            }

            if (passenger.IsTransit)
            {
                if (passenger.ConnectingFlight is null)
                {
                    throw new MeridianSystemException(
                        "BOARDING DENIED: The passenger is marked as transit but has no connecting flight.");
                }

                if (passenger.ConnectingFlight.Status == FlightStatus.Cancelled)
                {
                    throw new MeridianSystemException("BOARDING DENIED: The passenger's connecting flight is cancelled.");
                }

                TimeSpan connectionTime = flight.DepartureTime - passenger.ConnectingFlight.ArrivalTime;
                if (connectionTime.TotalMinutes < 0)
                {
                    throw new MeridianSystemException(
                        "BOARDING DENIED: The connecting flight arrives after the next flight departs.");
                }

                if (connectionTime.TotalMinutes < SystemRules.MinimumConnectionMinutes)
                {
                    throw new MeridianSystemException(
                        $"BOARDING DENIED: Only {(int)connectionTime.TotalMinutes} minutes remain after the " +
                        $"connecting flight's arrival; the minimum connection time is " +
                        $"{SystemRules.MinimumConnectionMinutes} minutes.");
                }
            }

            return $"BOARDING ELIGIBLE: Passenger {passenger.Name} has a confirmed seat on flight {flight.Name}.";
        }

        public static Booking ProcessBoarding(Passenger passenger, Flight flight)
        {
            if (flight.Status != FlightStatus.Boarding)
            {
                throw new MeridianSystemException(
                    $"BOARDING DENIED: Flight {flight.Name} must have Boarding status. Current status: {flight.Status}.");
            }

            CheckBoardingEligibility(passenger, flight);

            Booking booking = flight.ConfirmedBookings.First(
                confirmed => confirmed.Passenger.Id == passenger.Id &&
                             confirmed.Status == BookingStatus.Confirmed);

            booking.IsBoarded = true;
            return booking;
        }
    }
}
