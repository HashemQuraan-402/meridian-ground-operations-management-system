using MeridianGroundOperationsManagementSystem.Models;
using MeridianGroundOperationsManagementSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeridianGroundOperationsManagementSystem.Services
{
    public static class BaggageManager
    {
        public static Baggage AddBaggage(
            int id,
            BaggageType type,
            double baggageWeight,
            Passenger passenger,
            Flight flight)
        {
            if (id <= 0)
            {
                throw new MeridianSystemException("Baggage ID must be a positive number.");
            }

            if (Program.Baggages.Any(baggage => baggage.Id == id))
            {
                throw new MeridianSystemException($"Baggage ID {id} is already registered.");
            }

            if (flight.Direction != FlightDirection.Departing)
            {
                throw new MeridianSystemException("Baggage can only be loaded onto a departing flight.");
            }

            if (flight.Status == FlightStatus.Cancelled ||
                flight.Status == FlightStatus.Departed ||
                flight.Status == FlightStatus.Arrived)
            {
                throw new MeridianSystemException(
                    $"Baggage cannot be loaded because flight {flight.Name} has status {flight.Status}.");
            }

            Booking? confirmedBooking = flight.ConfirmedBookings.FirstOrDefault(
                booking => booking.Passenger.Id == passenger.Id &&
                           booking.Status == BookingStatus.Confirmed);

            if (confirmedBooking is null)
            {
                throw new MeridianSystemException(
                    $"Passenger {passenger.Name} does not have a confirmed booking on flight {flight.Name}.");
            }

            if (confirmedBooking.IsBoarded)
            {
                throw new MeridianSystemException(
                    $"Baggage cannot be added because passenger {passenger.Name} has already boarded flight {flight.Name}.");
            }

            if (baggageWeight <= 0)
            {
                throw new MeridianSystemException("Baggage weight must be a positive value.");
            }

            double singleBagLimit = GetSingleBagLimit(type);
            if (baggageWeight > singleBagLimit)
            {
                throw new MeridianSystemException(
                    $"A {type} bag cannot weigh more than {singleBagLimit:0.##} kg.");
            }

            double currentTotal = passenger.GetCumulativeBaggageWeight(flight);
            double newTotal = currentTotal + baggageWeight;
            double allowance = SystemRules.GetBaggageAllowance(passenger.Category);

            if (newTotal > allowance)
            {
                string category = SystemRules.GetPassengerCategoryName(passenger.Category);
                throw new MeridianSystemException(
                    $"BAGGAGE REJECTED: This bag would bring the passenger's total checked baggage " +
                    $"to {newTotal:0.##} kg, exceeding the {allowance:0.##} kg allowance for {category} passengers.");
            }

            Baggage baggage = new Baggage(id, type, baggageWeight, passenger, flight);
            passenger.Baggages.Add(baggage);
            return baggage;
        }

        private static double GetSingleBagLimit(BaggageType type)
        {
            if (type == BaggageType.Small)
            {
                return 10;
            }

            if (type == BaggageType.Medium)
            {
                return 20;
            }

            return 30;
        }
    }
}
