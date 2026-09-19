using MeridianGroundOperationsManagementSystem.Models;
using MeridianGroundOperationsManagementSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeridianGroundOperationsManagementSystem.Services
{
    public static class PassengerManager
    {
        public static Passenger CreatePassenger(
            int id,
            string name,
            int age,
            PassengerCategory category,
            bool isTransit,
            Flight? connectingFlight)
        {
            if (id <= 0)
            {
                throw new MeridianSystemException("Passenger ID must be a positive number.");
            }

            if (Program.Passengers.Any(passenger => passenger.Id == id))
            {
                throw new MeridianSystemException($"Passenger ID {id} is already registered.");
            }

            name = name.Trim();
            if (name.Length < 2 || name.Length > 100)
            {
                throw new MeridianSystemException("Passenger name must contain between 2 and 100 characters.");
            }

            if (age < 1 || age > 150)
            {
                throw new MeridianSystemException("Passenger age must be between 1 and 150.");
            }

            if (isTransit && connectingFlight is null)
            {
                throw new MeridianSystemException("A transit passenger must be linked to an arriving connecting flight.");
            }

            if (!isTransit && connectingFlight is not null)
            {
                throw new MeridianSystemException("A non-transit passenger cannot have a connecting flight.");
            }

            if (connectingFlight is not null)
            {
                if (connectingFlight.Direction != FlightDirection.Arriving)
                {
                    throw new MeridianSystemException("The connecting flight must be an arriving flight.");
                }

                if (connectingFlight.Status == FlightStatus.Cancelled)
                {
                    throw new MeridianSystemException("The connecting flight is cancelled and cannot be linked.");
                }
            }

            return new Passenger(id, name, age, category, isTransit, connectingFlight);
        }
    }
}
