using System;
using System.Collections.Generic;
using System.Text;

namespace MeridianGroundOperationsManagementSystem.Models
{
    public static class SystemRules
    {
        public const int MinimumConnectionMinutes = 45;
        public const int StandbyLimit = 5;
        public const double MaximumStaffDutyHours = 8;

        public const double StandardBaggageAllowance = 30;
        public const double VIPBaggageAllowance = 40;
        public const double ReducedMobilityBaggageAllowance = 35;

        public const int DepartingGateMinutesBefore = 30;
        public const int DepartingGateMinutesAfter = 15;
        public const int ArrivingGateMinutesBefore = 15;
        public const int ArrivingGateMinutesAfter = 30;

        public static double GetBaggageAllowance(PassengerCategory category)
        {
            if (category == PassengerCategory.VIP)
            {
                return VIPBaggageAllowance;
            }

            if (category == PassengerCategory.ReducedMobility)
            {
                return ReducedMobilityBaggageAllowance;
            }

            return StandardBaggageAllowance;
        }

        public static string GetPassengerCategoryName(PassengerCategory category)
        {
            return category == PassengerCategory.ReducedMobility
                ? "Reduced Mobility"
                : category.ToString();
        }
    }
}
