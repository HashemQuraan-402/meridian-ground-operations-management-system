using System;
using System.Collections.Generic;
using System.Text;

namespace MeridianGroundOperationsManagementSystem.Models
{
    public enum FlightDirection
    {
        Departing,
        Arriving
    }

    public enum FlightType
    {
        Domestic,
        International
    }

    public enum FlightStatus
    {
        Scheduled,
        Delayed,
        Boarding,
        Departed,
        Arrived,
        Cancelled
    }

    public enum BaggageType
    {
        Small,
        Medium,
        Large
    }

    public enum PassengerCategory
    {
        Standard,
        VIP,
        ReducedMobility
    }

    public enum StaffType
    {
        Available,
        Assigned,
        Off
    }

    public enum GroundStaffRole
    {
        GateAgent,
        BaggageStaff
    }

    public enum BookingStatus
    {
        Confirmed,
        Standby,
        Cancelled
    }
}
