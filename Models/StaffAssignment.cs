using System;
using System.Collections.Generic;
using System.Text;

namespace MeridianGroundOperationsManagementSystem.Models
{
    public class StaffAssignment
    {
        public StaffAssignment(int id, double dutyHours, Flight? flight, Gate? gate)
        {
            Id = id;
            DutyHours = dutyHours;
            Flight = flight;
            Gate = gate;
        }

        public int Id { get; set; }
        public double DutyHours { get; set; }
        public Flight? Flight { get; set; }
        public Gate? Gate { get; set; }

        public string TargetDescription
        {
            get
            {
                if (Flight is not null)
                {
                    return $"Flight {Flight.Name}";
                }

                return $"Gate {Gate!.Name}";
            }
        }

        public override string ToString()
        {
            return $"Assignment {Id} | {TargetDescription} | {DutyHours:0.##} hours";
        }
    }
}
