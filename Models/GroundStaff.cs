using System;
using System.Collections.Generic;
using System.Text;

namespace MeridianGroundOperationsManagementSystem.Models
{
    public class GroundStaff
    {
        public GroundStaff(int id, string name)
        {
            Id = id;
            Name = name;
            Type = StaffType.Available;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public StaffType Type { get; set; }
        public List<StaffAssignment> Assignments { get; set; } = new List<StaffAssignment>();

        public double AccumulatedWorkingHours
        {
            get { return Assignments.Sum(assignment => assignment.DutyHours); }
        }

        public virtual string Role
        {
            get { return "Ground Staff"; }
        }

        public override string ToString()
        {
            return $"Staff {Name} (ID: {Id}) | Role: {Role} | " +
                   $"Duty hours: {AccumulatedWorkingHours:0.##}/{SystemRules.MaximumStaffDutyHours:0.##} | Status: {Type}";
        }
    }

    public class GateAgent : GroundStaff
    {
        public GateAgent(int id, string name) : base(id, name)
        {
        }

        public override string Role
        {
            get { return "Gate Agent"; }
        }
    }

    public class BaggageStaff : GroundStaff
    {
        public BaggageStaff(int id, string name) : base(id, name)
        {
        }

        public override string Role
        {
            get { return "Baggage Staff"; }
        }
    }
}
