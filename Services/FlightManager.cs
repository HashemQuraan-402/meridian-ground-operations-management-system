using MeridianGroundOperationsManagementSystem.Models;
using MeridianGroundOperationsManagementSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.CodeAnalysis;

namespace MeridianGroundOperationsManagementSystem.Services
{
    public static class FlightManager
    {
        public static Flight RegisterFlight(
            int id,
            string name,
            FlightType type,
            DateTime departureTime,
            DateTime arrivalTime,
            FlightDirection direction,
            int seatCapacity)
        {
            if (id <= 0)
            {
                throw new MeridianSystemException("Flight ID must be a positive number.");
            }

            name = name.Trim();
            if (name.Length < 2 || name.Length > 100)
            {
                throw new MeridianSystemException("Flight name must contain between 2 and 100 characters.");
            }

            if (departureTime < DateTime.Now)
            {
                throw new MeridianSystemException("A new flight cannot be registered with a departure time in the past.");
            }

            if (arrivalTime <= departureTime)
            {
                throw new MeridianSystemException("Arrival time must be later than departure time.");
            }

            if (arrivalTime - departureTime < TimeSpan.FromMinutes(15))
            {
                throw new MeridianSystemException("A flight must last at least 15 minutes.");
            }

            if (seatCapacity < 1 || seatCapacity > 1000)
            {
                throw new MeridianSystemException("Seat capacity must be between 1 and 1000.");
            }

            return new Flight(id, name, type, departureTime, arrivalTime, direction, seatCapacity);
        }

        public static void AssignFlightToGate(Flight flight, Gate gate)
        {
            if (flight.Status == FlightStatus.Cancelled ||
                flight.Status == FlightStatus.Departed ||
                flight.Status == FlightStatus.Arrived)
            {
                throw new MeridianSystemException(
                    $"Gate assignment is not allowed because flight {flight.Name} has status {flight.Status}.");
            }

            if (flight.Type == FlightType.International && !gate.SupportsInternational)
            {
                throw new MeridianSystemException(
                    $"Gate {gate.Name} does not support international flights.");
            }

            if (flight.GateOccupancyStart < gate.AvailableFrom ||
                flight.GateOccupancyEnd > gate.AvailableUntil)
            {
                throw new MeridianSystemException(
                    $"Gate {gate.Name} is available from {gate.AvailableFrom:yyyy-MM-dd HH:mm} " +
                    $"to {gate.AvailableUntil:yyyy-MM-dd HH:mm}, but flight {flight.Name} needs it from " +
                    $"{flight.GateOccupancyStart:yyyy-MM-dd HH:mm} to {flight.GateOccupancyEnd:yyyy-MM-dd HH:mm}.");
            }

            foreach (Flight assignedFlight in gate.Flights)
            {
                if (assignedFlight.Id == flight.Id)
                {
                    continue;
                }

                bool overlaps = flight.GateOccupancyStart < assignedFlight.GateOccupancyEnd &&
                                assignedFlight.GateOccupancyStart < flight.GateOccupancyEnd;

                if (overlaps)
                {
                    throw new MeridianSystemException(
                        $"Gate {gate.Name} is already occupied by flight {assignedFlight.Name} from " +
                        $"{assignedFlight.GateOccupancyStart:yyyy-MM-dd HH:mm} to " +
                        $"{assignedFlight.GateOccupancyEnd:yyyy-MM-dd HH:mm}.");
                }
            }

            if (flight.Gate is not null)
            {
                flight.Gate.Flights.Remove(flight);
            }

            flight.Gate = gate;
            if (!gate.Flights.Contains(flight))
            {
                gate.Flights.Add(flight);
            }
        }

        public static void UpdateFlightStatus(Flight flight, FlightStatus newStatus)
        {
            if (flight.Status == newStatus)
            {
                throw new MeridianSystemException($"Flight {flight.Name} already has status {newStatus}.");
            }

            if (flight.Status == FlightStatus.Departed ||
                flight.Status == FlightStatus.Arrived ||
                flight.Status == FlightStatus.Cancelled)
            {
                throw new MeridianSystemException(
                    $"The status of a completed or cancelled flight cannot be changed. Current status: {flight.Status}.");
            }

            if (newStatus == FlightStatus.Boarding)
            {
                if (flight.Direction != FlightDirection.Departing)
                {
                    throw new MeridianSystemException("Only a departing flight can be placed in Boarding status.");
                }

                if (flight.Gate is null)
                {
                    throw new MeridianSystemException("Boarding cannot begin until the flight has an assigned gate.");
                }
            }

            if (newStatus == FlightStatus.Departed && flight.Direction != FlightDirection.Departing)
            {
                throw new MeridianSystemException("Only a departing flight can be marked as Departed.");
            }

            if (newStatus == FlightStatus.Arrived && flight.Direction != FlightDirection.Arriving)
            {
                throw new MeridianSystemException("Only an arriving flight can be marked as Arrived.");
            }

            if (newStatus == FlightStatus.Cancelled && flight.Gate is not null)
            {
                flight.Gate.Flights.Remove(flight);
                flight.Gate = null;
            }

            flight.Status = newStatus;
        }
    }
}
