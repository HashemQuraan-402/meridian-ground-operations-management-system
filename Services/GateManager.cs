using MeridianGroundOperationsManagementSystem.Models;
using MeridianGroundOperationsManagementSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeridianGroundOperationsManagementSystem.Services
{
    public static class GateManager
    {
        public static Gate RegisterGate(
            int id,
            string name,
            bool supportsInternational,
            DateTime availableFrom,
            DateTime availableUntil)
        {
            if (id <= 0)
            {
                throw new MeridianSystemException("Gate ID must be a positive number.");
            }

            name = name.Trim();
            if (name.Length < 1 || name.Length > 20)
            {
                throw new MeridianSystemException("Gate name must contain between 1 and 20 characters.");
            }

            if (availableUntil <= availableFrom)
            {
                throw new MeridianSystemException("Gate availability end time must be later than its start time.");
            }

            return new Gate(id, name, supportsInternational, availableFrom, availableUntil);
        }
    }
}
