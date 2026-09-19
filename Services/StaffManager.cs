using MeridianGroundOperationsManagementSystem.Exceptions;
using MeridianGroundOperationsManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeridianGroundOperationsManagementSystem.Services
{
    public static class StaffManager
    {
        public static GroundStaff RegisterStaff(int id, string name, GroundStaffRole role)
        {
            if (id <= 0)
            {
                throw new MeridianSystemException("Staff ID must be a positive number.");
            }

            if (Program.GroundStaffMembers.Any(staff => staff.Id == id))
            {
                throw new MeridianSystemException($"Staff ID {id} is already registered.");
            }

            name = name.Trim();
            if (name.Length < 2 || name.Length > 100)
            {
                throw new MeridianSystemException("Staff name must contain between 2 and 100 characters.");
            }

            if (role == GroundStaffRole.GateAgent)
            {
                return new GateAgent(id, name);
            }

            return new BaggageStaff(id, name);
        }

        public static StaffAssignment AssignToFlight(
            int assignmentId,
            GroundStaff staff,
            Flight flight,
            double dutyHours)
        {
            if (flight.Status == FlightStatus.Cancelled ||
                flight.Status == FlightStatus.Departed ||
                flight.Status == FlightStatus.Arrived)
            {
                throw new MeridianSystemException(
                    $"Staff cannot be assigned because flight {flight.Name} has status {flight.Status}.");
            }

            if (staff.Assignments.Any(assignment => assignment.Flight?.Id == flight.Id))
            {
                throw new MeridianSystemException(
                    $"Staff member {staff.Name} is already assigned to flight {flight.Name}.");
            }

            ValidateDutyHours(staff, dutyHours);
            StaffAssignment assignment = new StaffAssignment(assignmentId, dutyHours, flight, null);
            AddAssignment(staff, assignment);
            return assignment;
        }

        public static StaffAssignment AssignToGate(
            int assignmentId,
            GroundStaff staff,
            Gate gate,
            double dutyHours)
        {
            if (staff.Assignments.Any(assignment => assignment.Gate?.Id == gate.Id))
            {
                throw new MeridianSystemException(
                    $"Staff member {staff.Name} is already assigned to gate {gate.Name}.");
            }

            ValidateDutyHours(staff, dutyHours);
            StaffAssignment assignment = new StaffAssignment(assignmentId, dutyHours, null, gate);
            AddAssignment(staff, assignment);
            return assignment;
        }

        private static void ValidateDutyHours(GroundStaff staff, double dutyHours)
        {
            if (dutyHours <= 0)
            {
                throw new MeridianSystemException("Assignment duty hours must be a positive value.");
            }

            double newTotal = staff.AccumulatedWorkingHours + dutyHours;
            if (newTotal > SystemRules.MaximumStaffDutyHours)
            {
                throw new MeridianSystemException(
                    $"Staff assignment rejected: it would bring {staff.Name}'s cumulative duty time to " +
                    $"{newTotal:0.##} hours, exceeding the maximum of " +
                    $"{SystemRules.MaximumStaffDutyHours:0.##} hours per shift.");
            }
        }

        private static void AddAssignment(GroundStaff staff, StaffAssignment assignment)
        {
            staff.Assignments.Add(assignment);
            staff.Type = staff.AccumulatedWorkingHours >= SystemRules.MaximumStaffDutyHours
                ? StaffType.Off
                : StaffType.Assigned;
        }
    }
}
