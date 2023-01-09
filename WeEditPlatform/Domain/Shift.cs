
using System.Text.Json.Serialization;

namespace Domain
{
    public class Shift : Enumeration
    {
        public static Shift Shift1 = new Shift(1, nameof(Shift1).ToLowerInvariant());
        public static Shift Shift2 = new Shift(2, nameof(Shift2).ToLowerInvariant());
        public static Shift Shift3 = new Shift(3, nameof(Shift3).ToLowerInvariant());
        public static Shift Out = new Shift(4, nameof(Out).ToLowerInvariant());
        public static Shift Free = new Shift(5, nameof(Free).ToLowerInvariant());
        public static Shift None = new Shift(6, nameof(None).ToLowerInvariant());


        [JsonIgnore]
        public ICollection<StaffShift> StaffShifts { get; set; }
        public ICollection<Staff> Staffs { get; set; }

        public ICollection<JobStep> JobSteps { get; set; }


        public Shift(int id, string name) : base(id, name)
        {
        }

        public static Shift CalculateShift(DateTime inShiftAt)
        {
            // 22h30 - 6h30 yesterday: shift 3 of yesterday
            if (22.5 < inShiftAt.AddDays(-1).Hour && inShiftAt.Hour <= 6.5)
            {
                return Shift.Shift3;
            }
    
            // 6h30 - 14h30 today: shift 1 today
            if (6.5 < inShiftAt.Hour && inShiftAt.Hour <= 14.5)
            {
                return Shift.Shift1;
            }

            // 14h30 - 22h30 today: shift 2 today
            if (14.5 < inShiftAt.Hour && inShiftAt.Hour <= 22.5)
            {
                return Shift.Shift2;
            }

            // 22h30 - 6h30 tomorrow: shift 3 today
            if (22.5 < inShiftAt.Hour && inShiftAt.AddDays(1).Hour <= 6.5)
            {
                return Shift.Shift3;
            }

            return Shift.Free;
        }
    }
}