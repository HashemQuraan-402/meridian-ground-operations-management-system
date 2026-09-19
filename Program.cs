using MeridianGroundOperationsManagementSystem.Exceptions;
using MeridianGroundOperationsManagementSystem.Models;
using MeridianGroundOperationsManagementSystem.Services;
using System;
using System.Globalization;

namespace MeridianGroundOperationsManagementSystem
{
    class Program
    {
        public static List<Flight> Flights { get; } = new List<Flight>();
        public static List<Gate> Gates { get; } = new List<Gate>();
        public static List<Passenger> Passengers { get; } = new List<Passenger>();
        public static List<Baggage> Baggages { get; } = new List<Baggage>();
        public static List<Booking> Bookings { get; } = new List<Booking>();
        public static List<GroundStaff> GroundStaffMembers { get; } = new List<GroundStaff>();

        private static int nextBookingId = 1;
        private static int nextAssignmentId = 1;

        public static void Main(string[] args)
        {
            SeedSampleData();

            while (true)
            {
                try
                {
                    PrintMainMenu();
                    int option = ReadMenuOption("Select an option: ", 0, 9);

                    if (option == 0)
                    {
                        Console.WriteLine("Exiting the program. Goodbye!");
                        break;
                    }

                    DecideOperation(option);
                }
                catch (MeridianSystemException exception)
                {
                    Console.WriteLine($"Operation failed: {exception.Message}");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Unexpected error: {exception.Message}");
                }

                Console.WriteLine("----------------------------------------------------------");
            }
        }

        private static void SeedSampleData()
        {
            DateTime now = DateTime.Now;

            Gate gate = GateManager.RegisterGate(
                1,
                "A1",
                supportsInternational: true,
                availableFrom: now.AddHours(-1),
                availableUntil: now.AddHours(8));
            Gates.Add(gate);

            Flight flight = FlightManager.RegisterFlight(
                1,
                "MGT101",
                FlightType.Domestic,
                departureTime: now.AddHours(2),
                arrivalTime: now.AddHours(3),
                direction: FlightDirection.Departing,
                seatCapacity: 3);
            Flights.Add(flight);
            FlightManager.AssignFlightToGate(flight, gate);

            Passenger passenger = PassengerManager.CreatePassenger(
                100,
                "Jordan Lee",
                age: 29,
                category: PassengerCategory.Standard,
                isTransit: false,
                connectingFlight: null);
            Passengers.Add(passenger);

            Booking booking = BookingManager.BookPassengerSeat(
                nextBookingId++,
                passenger,
                flight);
            Bookings.Add(booking);

            GroundStaff staff = StaffManager.RegisterStaff(
                10,
                "Sam Carter",
                GroundStaffRole.GateAgent);
            GroundStaffMembers.Add(staff);
            StaffManager.AssignToGate(nextAssignmentId++, staff, gate, dutyHours: 4);
        }

        public static void PrintMainMenu()
        {
            Console.WriteLine("=== Meridian Terminal - Ground Operations System ===");
            Console.WriteLine("1. Register Flight");
            Console.WriteLine("2. Register Gate");
            Console.WriteLine("3. Assign Gate");
            Console.WriteLine("4. Update Flight Status");
            Console.WriteLine("5. Register Passenger");
            Console.WriteLine("6. Manage Boarding");
            Console.WriteLine("7. Manage Baggage");
            Console.WriteLine("8. Manage Bookings & Standby");
            Console.WriteLine("9. Manage Staff");
            Console.WriteLine("0. Exit");
        }

        public static void DecideOperation(int option)
        {
            switch (option)
            {
                case 1:
                    RegisterFlight();
                    break;
                case 2:
                    RegisterGate();
                    break;
                case 3:
                    AssignGate();
                    break;
                case 4:
                    UpdateFlightStatus();
                    break;
                case 5:
                    RegisterPassenger();
                    break;
                case 6:
                    ManageBoarding();
                    break;
                case 7:
                    ManageBaggage();
                    break;
                case 8:
                    ManageBookingsAndStandby();
                    break;
                case 9:
                    ManageStaff();
                    break;
                default:
                    throw new MeridianSystemException("Invalid menu option.");
            }
        }

        public static void RegisterFlight()
        {
            Console.WriteLine("=== Register Flight ===");

            int id = ReadInt("Enter Flight ID: ", "Invalid input. Please enter a whole number.");
            CheckFlightIdUniqueness(id);

            string name = ReadString("Enter Flight Name/Number: ", "Flight name cannot be empty.");
            CheckFlightNameUniqueness(name);

            FlightType type = ReadEnum<FlightType>(
                "Enter Flight Type (Domestic/International): ",
                "Invalid flight type. Enter Domestic or International.");

            DateTime departureTime = ReadDateTime(
                "Enter Departure Time (yyyy-MM-dd HH:mm): ",
                "Invalid date. Use the format yyyy-MM-dd HH:mm.");

            DateTime arrivalTime = ReadDateTime(
                "Enter Arrival Time (yyyy-MM-dd HH:mm): ",
                "Invalid date. Use the format yyyy-MM-dd HH:mm.");

            FlightDirection direction = ReadEnum<FlightDirection>(
                "Enter Flight Direction (Departing/Arriving): ",
                "Invalid direction. Enter Departing or Arriving.");

            int seatCapacity = ReadInt(
                "Enter Seat Capacity: ",
                "Invalid input. Please enter a whole number.");

            Flight flight = FlightManager.RegisterFlight(
                id,
                name,
                type,
                departureTime,
                arrivalTime,
                direction,
                seatCapacity);

            Flights.Add(flight);
            Console.WriteLine("Flight registered successfully.");
            Console.WriteLine(flight);
        }

        public static void RegisterGate()
        {
            Console.WriteLine("=== Register Gate ===");

            int id = ReadInt("Enter Gate ID: ", "Invalid input. Please enter a whole number.");
            CheckGateIdUniqueness(id);

            string name = ReadString("Enter Gate Name: ", "Gate name cannot be empty.");
            CheckGateNameUniqueness(name);

            bool supportsInternational = ReadBool(
                "Does the gate support international flights? (true/false): ",
                "Invalid input. Enter true or false.");

            DateTime availableFrom = ReadDateTime(
                "Enter Gate Available From (yyyy-MM-dd HH:mm): ",
                "Invalid date. Use the format yyyy-MM-dd HH:mm.");

            DateTime availableUntil = ReadDateTime(
                "Enter Gate Available Until (yyyy-MM-dd HH:mm): ",
                "Invalid date. Use the format yyyy-MM-dd HH:mm.");

            Gate gate = GateManager.RegisterGate(
                id,
                name,
                supportsInternational,
                availableFrom,
                availableUntil);

            Gates.Add(gate);
            Console.WriteLine("Gate registered successfully.");
            Console.WriteLine(gate);
        }

        public static void AssignGate()
        {
            Console.WriteLine("=== Assign Flight to Gate ===");
            Flight flight = GetFlightByName(ReadString("Enter Flight Name/Number: ", "Flight name cannot be empty."));
            Gate gate = GetGateByName(ReadString("Enter Gate Name: ", "Gate name cannot be empty."));

            FlightManager.AssignFlightToGate(flight, gate);
            Console.WriteLine(
                $"Flight {flight.Name} was assigned to gate {gate.Name} from " +
                $"{flight.GateOccupancyStart:yyyy-MM-dd HH:mm} to {flight.GateOccupancyEnd:yyyy-MM-dd HH:mm}.");
        }

        public static void UpdateFlightStatus()
        {
            Console.WriteLine("=== Update Flight Status ===");
            Flight flight = GetFlightByName(ReadString("Enter Flight Name/Number: ", "Flight name cannot be empty."));
            FlightStatus newStatus = ReadEnum<FlightStatus>(
                "Enter Status (Scheduled/Delayed/Boarding/Departed/Arrived/Cancelled): ",
                "Invalid flight status.");

            FlightManager.UpdateFlightStatus(flight, newStatus);
            Console.WriteLine($"Flight {flight.Name} status was updated to {flight.Status}.");
        }

        public static void RegisterPassenger()
        {
            Console.WriteLine("=== Register Passenger ===");

            int id = ReadInt("Enter Passenger ID: ", "Invalid input. Please enter a whole number.");
            string name = ReadString("Enter Passenger Name: ", "Passenger name cannot be empty.");
            int age = ReadInt("Enter Passenger Age: ", "Invalid input. Please enter a whole number.");
            PassengerCategory category = ReadEnum<PassengerCategory>(
                "Enter Category (Standard/VIP/ReducedMobility): ",
                "Invalid category. Enter Standard, VIP, or ReducedMobility.");
            bool isTransit = ReadBool(
                "Is this a connecting passenger? (true/false): ",
                "Invalid input. Enter true or false.");

            Flight? connectingFlight = null;
            if (isTransit)
            {
                string connectingFlightName = ReadString(
                    "Enter Earlier Connecting Flight Name/Number: ",
                    "Connecting flight name cannot be empty.");
                connectingFlight = GetFlightByName(connectingFlightName);
            }

            Passenger passenger = PassengerManager.CreatePassenger(
                id,
                name,
                age,
                category,
                isTransit,
                connectingFlight);

            Passengers.Add(passenger);
            Console.WriteLine("Passenger registered successfully. Registration does not create a booking.");
            Console.WriteLine(passenger);
        }

        public static void ManageBoarding()
        {
            while (true)
            {
                Console.WriteLine("=== Passenger & Boarding ===");
                Console.WriteLine("1. Check Boarding Eligibility");
                Console.WriteLine("2. Process Boarding");
                Console.WriteLine("0. Back to Main Menu");

                int option = ReadMenuOption("Select an option: ", 0, 2);
                if (option == 0)
                {
                    return;
                }

                if (option == 1)
                {
                    CheckBoardingEligibility();
                }
                else
                {
                    ProcessBoarding();
                }

                Console.WriteLine("----------------------------------------------------------");
            }
        }

        public static void CheckBoardingEligibility()
        {
            Console.WriteLine("=== Check Boarding Eligibility ===");
            Passenger passenger = GetPassengerById(
                ReadInt("Enter Passenger ID: ", "Invalid input. Please enter a whole number."));
            Flight flight = GetFlightByName(
                ReadString("Enter Next Flight Name/Number: ", "Flight name cannot be empty."));

            Console.WriteLine(BoardingManager.CheckBoardingEligibility(passenger, flight));
        }

        public static void ProcessBoarding()
        {
            Console.WriteLine("=== Process Boarding ===");
            Passenger passenger = GetPassengerById(
                ReadInt("Enter Passenger ID: ", "Invalid input. Please enter a whole number."));
            Flight flight = GetFlightByName(
                ReadString("Enter Flight Name/Number: ", "Flight name cannot be empty."));

            Booking booking = BoardingManager.ProcessBoarding(passenger, flight);
            Console.WriteLine(
                $"BOARDING COMPLETED: {passenger.Name} boarded flight {flight.Name}, seat {booking.SeatNumber}.");
        }

        public static void ManageBaggage()
        {
            while (true)
            {
                Console.WriteLine("=== Baggage ===");
                Console.WriteLine("1. Register Baggage");
                Console.WriteLine("2. View Cumulative Baggage Weight");
                Console.WriteLine("0. Back to Main Menu");

                int option = ReadMenuOption("Select an option: ", 0, 2);
                if (option == 0)
                {
                    return;
                }

                if (option == 1)
                {
                    RegisterBaggage();
                }
                else
                {
                    ViewCumulativeBaggageWeight();
                }

                Console.WriteLine("----------------------------------------------------------");
            }
        }

        public static void RegisterBaggage()
        {
            Console.WriteLine("=== Register Baggage ===");
            int baggageId = ReadInt("Enter Baggage ID: ", "Invalid input. Please enter a whole number.");
            Passenger passenger = GetPassengerById(
                ReadInt("Enter Passenger ID: ", "Invalid input. Please enter a whole number."));
            Flight flight = GetFlightByName(
                ReadString("Enter Flight Name/Number: ", "Flight name cannot be empty."));
            BaggageType baggageType = ReadEnum<BaggageType>(
                "Enter Baggage Type (Small/Medium/Large): ",
                "Invalid baggage type.");
            double baggageWeight = ReadDouble(
                "Enter Baggage Weight (kg): ",
                "Invalid input. Please enter a number.");

            Baggage baggage = BaggageManager.AddBaggage(
                baggageId,
                baggageType,
                baggageWeight,
                passenger,
                flight);

            Baggages.Add(baggage);
            Console.WriteLine("Baggage registered successfully.");
            Console.WriteLine(baggage);
            PrintBaggageTotal(passenger, flight);
        }

        public static void ViewCumulativeBaggageWeight()
        {
            Console.WriteLine("=== View Cumulative Baggage Weight ===");
            Passenger passenger = GetPassengerById(
                ReadInt("Enter Passenger ID: ", "Invalid input. Please enter a whole number."));
            Flight flight = GetFlightByName(
                ReadString("Enter Flight Name/Number: ", "Flight name cannot be empty."));

            PrintBaggageTotal(passenger, flight);
        }

        private static void PrintBaggageTotal(Passenger passenger, Flight flight)
        {
            double total = passenger.GetCumulativeBaggageWeight(flight);
            double allowance = SystemRules.GetBaggageAllowance(passenger.Category);
            Console.WriteLine(
                $"Passenger {passenger.Name} has {total:0.##} kg checked on flight {flight.Name}. " +
                $"Category allowance: {allowance:0.##} kg.");
        }

        public static void ManageBookingsAndStandby()
        {
            while (true)
            {
                Console.WriteLine("=== Manage Bookings & Standby ===");
                Console.WriteLine("1. Book/Confirm Passenger Seat");
                Console.WriteLine("2. Cancel Confirmed Booking");
                Console.WriteLine("3. View Flight Standby List");
                Console.WriteLine("0. Back to Main Menu");

                int option = ReadMenuOption("Select an option: ", 0, 3);
                if (option == 0)
                {
                    return;
                }

                if (option == 1)
                {
                    BookPassengerSeat();
                }
                else if (option == 2)
                {
                    CancelConfirmedBooking();
                }
                else
                {
                    ViewFlightStandbyList();
                }

                Console.WriteLine("----------------------------------------------------------");
            }
        }

        public static void BookPassengerSeat()
        {
            Console.WriteLine("=== Book/Confirm Passenger Seat ===");
            Passenger passenger = GetPassengerById(
                ReadInt("Enter Passenger ID: ", "Invalid input. Please enter a whole number."));
            Flight flight = GetFlightByName(
                ReadString("Enter Flight Name/Number: ", "Flight name cannot be empty."));

            Booking booking = BookingManager.BookPassengerSeat(nextBookingId, passenger, flight);
            Bookings.Add(booking);
            nextBookingId++;

            if (booking.Status == BookingStatus.Confirmed)
            {
                Console.WriteLine($"BOOKING CONFIRMED: Seat {booking.SeatNumber} was assigned.");
            }
            else
            {
                Console.WriteLine(
                    $"FLIGHT FULL: Passenger was added to standby position {flight.StandbyBookings.Count}.");
            }

            Console.WriteLine(booking);
        }

        public static void CancelConfirmedBooking()
        {
            Console.WriteLine("=== Cancel Confirmed Booking ===");
            Passenger passenger = GetPassengerById(
                ReadInt("Enter Passenger ID: ", "Invalid input. Please enter a whole number."));
            Flight flight = GetFlightByName(
                ReadString("Enter Flight Name/Number: ", "Flight name cannot be empty."));

            Booking? promoted = BookingManager.CancelConfirmedBooking(passenger, flight);
            Console.WriteLine($"Confirmed booking for {passenger.Name} was cancelled.");

            if (promoted is null)
            {
                Console.WriteLine("No standby passenger was waiting for the freed seat.");
            }
            else
            {
                Console.WriteLine(
                    $"STANDBY PROMOTED: {promoted.Passenger.Name} now has confirmed seat {promoted.SeatNumber}.");
            }
        }

        public static void ViewFlightStandbyList()
        {
            Console.WriteLine("=== View Flight Standby List ===");
            Flight flight = GetFlightByName(
                ReadString("Enter Flight Name/Number: ", "Flight name cannot be empty."));

            if (flight.StandbyBookings.Count == 0)
            {
                Console.WriteLine($"Flight {flight.Name} has no standby passengers.");
                return;
            }

            Console.WriteLine($"Standby list for flight {flight.Name}:");
            int position = 1;
            foreach (Booking booking in flight.StandbyBookings)
            {
                Console.WriteLine($"{position}. {booking.Passenger.Name} (Passenger ID: {booking.Passenger.Id})");
                position++;
            }
        }

        public static void ManageStaff()
        {
            while (true)
            {
                Console.WriteLine("=== Staff Management ===");
                Console.WriteLine("1. Register Ground Staff");
                Console.WriteLine("2. Assign Staff to Flight");
                Console.WriteLine("3. Assign Staff to Gate");
                Console.WriteLine("4. View Staff Cumulative Duty Hours");
                Console.WriteLine("0. Back to Main Menu");

                int option = ReadMenuOption("Select an option: ", 0, 4);
                if (option == 0)
                {
                    return;
                }

                switch (option)
                {
                    case 1:
                        RegisterGroundStaff();
                        break;
                    case 2:
                        AssignStaffToFlight();
                        break;
                    case 3:
                        AssignStaffToGate();
                        break;
                    case 4:
                        ViewStaffDutyHours();
                        break;
                }

                Console.WriteLine("----------------------------------------------------------");
            }
        }

        public static void RegisterGroundStaff()
        {
            Console.WriteLine("=== Register Ground Staff ===");
            int id = ReadInt("Enter Staff ID: ", "Invalid input. Please enter a whole number.");
            string name = ReadString("Enter Staff Name: ", "Staff name cannot be empty.");
            GroundStaffRole role = ReadEnum<GroundStaffRole>(
                "Enter Role (GateAgent/BaggageStaff): ",
                "Invalid role. Enter GateAgent or BaggageStaff.");

            GroundStaff staff = StaffManager.RegisterStaff(id, name, role);
            GroundStaffMembers.Add(staff);
            Console.WriteLine("Staff member registered successfully.");
            Console.WriteLine(staff);
        }

        public static void AssignStaffToFlight()
        {
            Console.WriteLine("=== Assign Staff to Flight ===");
            GroundStaff staff = GetStaffById(
                ReadInt("Enter Staff ID: ", "Invalid input. Please enter a whole number."));
            Flight flight = GetFlightByName(
                ReadString("Enter Flight Name/Number: ", "Flight name cannot be empty."));
            double hours = ReadDouble(
                "Enter Assignment Duty Hours: ",
                "Invalid input. Please enter a number.");

            StaffAssignment assignment = StaffManager.AssignToFlight(
                nextAssignmentId,
                staff,
                flight,
                hours);
            nextAssignmentId++;

            Console.WriteLine("Staff assignment created successfully.");
            Console.WriteLine(assignment);
            Console.WriteLine(staff);
        }

        public static void AssignStaffToGate()
        {
            Console.WriteLine("=== Assign Staff to Gate ===");
            GroundStaff staff = GetStaffById(
                ReadInt("Enter Staff ID: ", "Invalid input. Please enter a whole number."));
            Gate gate = GetGateByName(
                ReadString("Enter Gate Name: ", "Gate name cannot be empty."));
            double hours = ReadDouble(
                "Enter Assignment Duty Hours: ",
                "Invalid input. Please enter a number.");

            StaffAssignment assignment = StaffManager.AssignToGate(
                nextAssignmentId,
                staff,
                gate,
                hours);
            nextAssignmentId++;

            Console.WriteLine("Staff assignment created successfully.");
            Console.WriteLine(assignment);
            Console.WriteLine(staff);
        }

        public static void ViewStaffDutyHours()
        {
            Console.WriteLine("=== View Staff Cumulative Duty Hours ===");
            GroundStaff staff = GetStaffById(
                ReadInt("Enter Staff ID: ", "Invalid input. Please enter a whole number."));

            Console.WriteLine(staff);
            if (staff.Assignments.Count == 0)
            {
                Console.WriteLine("This staff member has no assignments in the current shift.");
                return;
            }

            Console.WriteLine("Assignments:");
            foreach (StaffAssignment assignment in staff.Assignments)
            {
                Console.WriteLine(assignment);
            }
        }

        public static int ReadMenuOption(string prompt, int minimum, int maximum)
        {
            while (true)
            {
                int option = ReadInt(prompt, "Invalid input. Please enter a whole number.");
                if (option >= minimum && option <= maximum)
                {
                    return option;
                }

                Console.WriteLine($"Please select an option between {minimum} and {maximum}.");
            }
        }

        public static int ReadInt(string prompt, string errorMessage)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value))
                {
                    return value;
                }

                Console.WriteLine(errorMessage);
            }
        }

        public static double ReadDouble(string prompt, string errorMessage)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) ||
                    double.TryParse(input, out value))
                {
                    return value;
                }

                Console.WriteLine(errorMessage);
            }
        }

        public static DateTime ReadDateTime(string prompt, string errorMessage)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (DateTime.TryParseExact(
                    input,
                    "yyyy-MM-dd HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime value))
                {
                    return value;
                }

                Console.WriteLine(errorMessage);
            }
        }

        public static T ReadEnum<T>(string prompt, string errorMessage) where T : struct, Enum
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (Enum.TryParse(input, true, out T value) && Enum.IsDefined(value))
                {
                    return value;
                }

                Console.WriteLine(errorMessage);
            }
        }

        public static string ReadString(string prompt, string errorMessage, int maximumLength = 100)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input) && input.Trim().Length <= maximumLength)
                {
                    return input.Trim();
                }

                Console.WriteLine(errorMessage);
            }
        }

        public static bool ReadBool(string prompt, string errorMessage)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();

                if (bool.TryParse(input, out bool value))
                {
                    return value;
                }

                Console.WriteLine(errorMessage);
            }
        }

        public static void CheckFlightIdUniqueness(int id)
        {
            if (Flights.Any(flight => flight.Id == id))
            {
                throw new MeridianSystemException($"Flight ID {id} already exists.");
            }
        }

        public static void CheckFlightNameUniqueness(string name)
        {
            if (Flights.Any(flight => flight.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new MeridianSystemException($"Flight {name} already exists.");
            }
        }

        public static void CheckGateIdUniqueness(int id)
        {
            if (Gates.Any(gate => gate.Id == id))
            {
                throw new MeridianSystemException($"Gate ID {id} already exists.");
            }
        }

        public static void CheckGateNameUniqueness(string name)
        {
            if (Gates.Any(gate => gate.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new MeridianSystemException($"Gate {name} already exists.");
            }
        }

        public static Gate GetGateByName(string name)
        {
            return Gates.FirstOrDefault(
                gate => gate.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                ?? throw new MeridianSystemException($"Gate {name} does not exist.");
        }

        public static Flight GetFlightByName(string name)
        {
            return Flights.FirstOrDefault(
                flight => flight.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                ?? throw new MeridianSystemException($"Flight {name} does not exist.");
        }

        public static Passenger GetPassengerById(int id)
        {
            return Passengers.FirstOrDefault(passenger => passenger.Id == id)
                ?? throw new MeridianSystemException($"Passenger ID {id} does not exist.");
        }

        public static GroundStaff GetStaffById(int id)
        {
            return GroundStaffMembers.FirstOrDefault(staff => staff.Id == id)
                ?? throw new MeridianSystemException($"Staff ID {id} does not exist.");
        }
    }
}
