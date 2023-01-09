using Application.Models;
using Domain;

namespace Application.Services
{
    public interface IStaffService
    {
        Staff CreateStaff(CreateStaffVM request);

        Staff UpdateStaff(UpdateStaffVM request);

        void RemoveStaff(int staffId);

        Staff FindStaff(int id, bool isInclude);
        Staff FindStaffByUserId(string userId, bool isInclude);
        List<Staff> FindStaffs(int[] ids, bool isInclude);
        List<Staff> FindAllStaffs();
        List<Staff> FindStaffs(int pageNumber, int pageSize, string columnOrders, string searchValue, bool isInclude, out int totalRecords);
        List<JobStep> FindJobStepsOfWorker(int workerId);

        void AddRolesForStaff(AddRolesForStaffVM request);
        void RemoveRolesForStaff(RemoveRolesForStaffVM request);

        void AddProductLevelForStaff(AddProductLevelsForStaffVM request);
        void RemoveProductLevelForStaff(RemoveProductLevelsForStaffVM request);

        void SetStaffOutShift(StaffOutShiftVM request);
        void SetStaffInShift(StaffInShiftVM request);

        List<StaffShift> GetLastInShifts(int staffId, int? number = 1);
        List<StaffShift> GetLastOutShifts(int staffId, int? number = 1);

        JobStep UpdateStepStatus(UpdateStepStatusVM request);
    }
}
