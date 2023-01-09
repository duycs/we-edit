using Application.Models;
using Application.Queries;
using Domain;
using FluentValidation;
using Infrastructure.Pagging;
using Infrastructure.Repository;
using Infrastructure.Extensions;

namespace Application.Services
{
    public class StaffService : IStaffService
    {
        private readonly IRepositoryService _repositoryService;

        public StaffService(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public void AddProductLevelForStaff(AddProductLevelsForStaffVM request)
        {
            var productLevels = _repositoryService.List<ProductLevel>(w => request.ProductLevelIds.Contains(w.Id));
            if (productLevels == null && !productLevels.Any()) throw new Exception($"productLevels not found"); ;

            var staff = _repositoryService.Find<Staff>(w => w.Id == request.StaffId);
            if (staff == null) throw new Exception($"Staff {request.StaffId} not found");

            staff.AddProductLevels(productLevels.ToArray());

            _repositoryService.Update(staff);
            _repositoryService.SaveChanges();
        }

        public void AddRolesForStaff(AddRolesForStaffVM request)
        {
            List<Role> rolesExisting = new List<Role>();
            if (request.RoleIds == null || request.RoleIds.Any())
            {
                rolesExisting = _repositoryService.List<Role>(w => request.RoleIds.Contains(w.Id));
            }

            if (rolesExisting.Any())
            {
                var staff = _repositoryService.Find<Staff>(w => w.Id == request.StaffId);
                if (staff == null) throw new Exception($"Staff {request.StaffId} not found");

                staff.AddRoles(rolesExisting.ToArray());
                _repositoryService.Update(staff);
                _repositoryService.SaveChanges();
            }
        }

        public void RemoveRolesForStaff(RemoveRolesForStaffVM request)
        {
            List<Role> rolesExisting = new List<Role>();
            if (request.RoleIds == null || request.RoleIds.Any())
            {
                rolesExisting = _repositoryService.List<Role>(w => request.RoleIds.Contains(w.Id));
            }

            if (rolesExisting.Any())
            {
                var staff = _repositoryService.Find<Staff>(w => w.Id == request.StaffId);
                if (staff == null) throw new Exception($"Staff {request.StaffId} not found");

                staff.RemoveRoles(rolesExisting.ToArray());
                _repositoryService.Update(staff);
                _repositoryService.SaveChanges();
            }
        }

        public Staff FindStaff(int id, bool isInclude)
        {
            return _repositoryService.Find<Staff>(id, new StaffSpecification(isInclude));
        }

        public List<Staff> FindStaffs(int[] ids, bool isInclude)
        {
            return _repositoryService.List<Staff>(ids, new StaffSpecification(isInclude));
        }

        public List<Staff> FindStaffs(int pageNumber, int pageSize, string columnOrders, string searchValue, bool isInclude, out int totalRecords)
        {
            var staffSpecification = new StaffSpecification(isInclude, searchValue, columnOrders.ToColumnOrders());
            var pagedStaffs = _repositoryService.Find<Staff>(pageNumber, pageSize, staffSpecification, out totalRecords).ToList();
            return pagedStaffs;
        }

        public void RemoveProductLevelForStaff(RemoveProductLevelsForStaffVM request)
        {
            var productLevels = _repositoryService.List<ProductLevel>(w => request.ProductLevelIds.Contains(w.Id));
            if (productLevels == null) throw new Exception($"productLevels not found"); ;

            var staff = _repositoryService.Find<Staff>(w => w.Id == request.StaffId);
            if (staff == null) throw new Exception($"Staff {request.StaffId} not found");

            staff.RemoveProductLevels(productLevels.ToArray());

            _repositoryService.Update(staff);
            _repositoryService.SaveChanges();
        }


        public void SetStaffInShift(StaffInShiftVM request)
        {
            var staff = _repositoryService.Find<Staff>(request.StaffId);
            if (staff == null) throw new Exception($"Staff {request.StaffId} not found");

            staff.AddInShift(request.InShiftAt);

            _repositoryService.Update(staff);
            _repositoryService.SaveChanges();
        }

        public void SetStaffOutShift(StaffOutShiftVM request)
        {
            var staff = _repositoryService.Find<Staff>(request.StaffId);
            if (staff == null) throw new Exception($"Staff {request.StaffId} not found");

            staff.AddOutShift(request.OutShiftAt);

            _repositoryService.Update(staff);
            _repositoryService.SaveChanges();
        }

        public Staff CreateStaff(CreateStaffVM request)
        {
            if (request == null) throw new Exception("Request can not empty");

            var staff = Staff.Create(request.UserId, request.FullName, request.Account, request.Email);

            // add roles and productLevels existing
            if (request.RoleIds != null && request.RoleIds.Any())
            {
                var roles = _repositoryService.List<Role>(request.RoleIds).ToArray();
                if (roles != null && roles.Any())
                {
                    staff.AddRoles(roles);
                }
            }

            if (request.ProductLevelIds != null && request.ProductLevelIds.Any())
            {
                var productLevels = _repositoryService.List<ProductLevel>(request.ProductLevelIds).ToArray();
                if (productLevels != null && productLevels.Any())
                {
                    staff.AddProductLevels(productLevels);
                }
            }

            if (request.GroupIds != null && request.GroupIds.Any())
            {
                var groups = _repositoryService.List<Group>(request.GroupIds).ToArray();
                if (groups != null && groups.Any())
                {
                    staff.AddGroups(groups);
                }
            }

            _repositoryService.Add(staff);
            _repositoryService.SaveChanges();

            return staff;
        }

        public void RemoveStaff(int staffId)
        {
            var staffExisting = _repositoryService.Find<Staff>(staffId);
            if (staffExisting == null) throw new Exception($"Staff id {staffId} not found");

            _repositoryService.Delete(staffExisting);
            _repositoryService.SaveChanges();
        }

        public List<StaffShift> GetLastInShifts(int staffId, int? number = 1)
        {
            var staffShifts = _repositoryService.List<StaffShift>(w => w.DateDeleted == 0 && w.StaffId == staffId && w.InShiftAt != DateTimeExtension.GetDefaultDateTime())
                .OrderByDescending(w => w.InShiftAt).Take(number ?? 1).ToList();

            return staffShifts;
        }

        public List<StaffShift> GetLastOutShifts(int staffId, int? number = 1)
        {
            var staffShifts = _repositoryService.List<StaffShift>(w => w.DateDeleted == 0 && w.StaffId == staffId && w.OutShiftAt != DateTimeExtension.GetDefaultDateTime())
               .OrderByDescending(w => w.OutShiftAt).Take(number ?? 1).ToList();

            return staffShifts;
        }

        public List<JobStep> FindJobStepsOfWorker(int workerId)
        {
            var jobSteps = _repositoryService.Find<JobStep>(new JobStepsOfWorkersSpecification(workerId, true)).ToList();
            return jobSteps;
        }

        public List<Staff> FindAllStaffs()
        {
            return _repositoryService.All<Staff>();
        }

        public JobStep UpdateStepStatus(UpdateStepStatusVM request)
        {
            var jobStep = _repositoryService.Find<JobStep>(w => w.JobId == request.JobId
            && w.StepId == request.StepId && w.WorkerId == request.StaffId);

            if (jobStep == null)
            {
                throw new Exception("JobStep can not empty");
            }

            var staff = _repositoryService.Find<Staff>(s => s.Id == request.StaffId);
            if (staff == null)
            {
                throw new Exception("Staff can not empty");
            }

            // assigned if any step assigned
            staff.SetAssigned();

            // TODO: validate step status pre-condition
            // start-end time with status
            if (request.Status == StepStatus.Done || request.Status == StepStatus.Rejected || request.Status == StepStatus.Approved)
            {
                jobStep.EndWithStatus(request.Status);
            }
            else if (request.Status == StepStatus.Doing)
            {
                jobStep.StartDoing();
            }
            else if (request.Status == StepStatus.Assigned || request.Status == StepStatus.Todo)
            {
                jobStep.ResetTimeWithStatus(request.Status);
            }
            else
            {
                jobStep.UpdateStatus(request.Status);
            }


            _repositoryService.Update(jobStep);

            _repositoryService.Update(staff);

            _repositoryService.SaveChanges();

            return jobStep;
        }

        public Staff UpdateStaff(UpdateStaffVM request)
        {
            var staff = _repositoryService.Find<Staff>(request.StaffId, new StaffSpecification(true));
            if (staff == null)
            {
                throw new Exception("Staff can not empty");
            }

            var roles = (request.RoleIds != null && request.RoleIds.Any()) ? _repositoryService.List<Role>(r => request.RoleIds.Contains(r.Id)).ToList() : new List<Role>();
            var groups = (request.GroupIds != null && request.GroupIds.Any()) ? _repositoryService.List<Group>(g => request.GroupIds.Contains(g.Id)).ToList() : new List<Group>();
            var productLevels = (request.ProductLevelIds != null && request.ProductLevelIds.Any()) ? _repositoryService.List<ProductLevel>(p => request.ProductLevelIds.Contains(p.Id)).ToList() : new List<ProductLevel>();

            staff.Update(request.FullName, request.Account, roles, groups, productLevels);

            _repositoryService.Update(staff);
            _repositoryService.SaveChanges();

            return staff;
        }

        public Staff FindStaffByUserId(string userId, bool isInclude)
        {
            var staff = _repositoryService.Find<Staff>(new StaffByUserIdSpecification(userId, isInclude)).FirstOrDefault();
            return staff;
        }
    }
}
