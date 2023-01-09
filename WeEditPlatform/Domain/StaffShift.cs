namespace Domain
{
    public class StaffShift : EntityBase
    {
        public int StaffId { get; set; }
        public Staff Staff { get; set; }

        public int ShiftId { get; set; }
        public Shift Shift { get; set; }

        public DateTime InShiftAt { get; set; }
        public DateTime OutShiftAt { get; set; }
   

        public static StaffShift CreateInShift(Staff staff, DateTime inShiftAt)
        {
            return new StaffShift()
            {
                StaffId = staff.Id,
                InShiftAt = inShiftAt,
                ShiftId = Shift.CalculateShift(inShiftAt).Id
            };
        }

        public static StaffShift CreateOutShift(Staff staff, DateTime outShiftAt)
        {
            return new StaffShift()
            {
                StaffId = staff.Id,
                OutShiftAt = outShiftAt,
                ShiftId = Shift.Out.Id
            };
        }
        
    }
}
