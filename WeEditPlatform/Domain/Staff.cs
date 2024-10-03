using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain
{
    public class Staff : EntityBase
    {
        public string UserId { get; set; }
        public string FullName { get; set; }

        /// <summary>
        /// eg: doan.tv
        /// </summary>
        public string Account { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<StaffRole>? StaffRoles { get; set; }
        public ICollection<Role>? Roles { get; set; }


        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<StaffGroup>? StaffGroup { get; set; }
        public ICollection<Group>? Groups { get; set; }


        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<StaffProductLevel>? StaffProductLevels { get; set; }
        public ICollection<ProductLevel>? ProductLevels { get; set; }


        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<StaffShift>? StaffShifts { get; set; }
        public ICollection<Shift>? Shifts { get; set; }

        public ICollection<JobStep>? JobSteps { get; set; }


        public bool IsAssigned { get; set; }
        public int CurrentShiftId { get; set; }


        public static Staff Create(string userId, string fullName, string account, string email)
        {
            return new Staff()
            {
                UserId = userId,
                FullName = fullName,
                Account = account,
                Email = email
            };
        }

        public static Staff Create(string userId, string fullName, string account, string email, Role[]? roles, Group[]? groups)
        {
            return new Staff()
            {
                UserId = userId,
                FullName = fullName,
                Account = account,
                Email = email,
                Roles = roles,
                Groups = groups,
            };
        }

        public Staff Update(string? fullname, string? account, List<Role>? roles, List<Group>? groups, List<ProductLevel>? productLevels)
        {
            if (!string.IsNullOrEmpty(fullname))
            {
                FullName = fullname;
            }

            if (!string.IsNullOrEmpty(account))
            {
                Account = account;
            }

            // can be empty
            Roles = roles;
            Groups = groups;
            ProductLevels = productLevels;

            return this;
        }


        public Staff AddRoles(Role[] roles)
        {
            if (Roles == null)
            {
                Roles = new List<Role>();
            }

            foreach (var r in roles)
            {
                Roles.Add(r);
            }

            return this;
        }

        public Staff RemoveRoles(Role[] roles)
        {
            if (Roles != null && Roles.Any())
            {
                var removeRoleIds = roles.Select(r => r.Id).ToList();
                if (removeRoleIds.Any())
                {
                    Roles = Roles.Where(r => !removeRoleIds.Contains(r.Id)).ToList();
                }
            }
            return this;
        }

        public Staff AddProductLevels(ProductLevel[] productLevels)
        {
            if (ProductLevels == null)
            {
                ProductLevels = new List<ProductLevel>();
            }

            foreach (var p in productLevels)
            {
                ProductLevels.Add(p);
            }

            return this;
        }

        public Staff AddGroups(Group[] groups)
        {
            if (Groups == null)
            {
                Groups = new List<Group>();
            }

            foreach (var g in groups)
            {
                Groups.Add(g);
            }

            return this;
        }

        public Staff RemoveProductLevels(ProductLevel[] productLevels)
        {
            if (ProductLevels != null && ProductLevels.Any())
            {
                var productlevelIds = productLevels.Select(p => p.Id).ToList();
                ProductLevels = ProductLevels.Where(p => !productlevelIds.Contains(p.Id)).ToList();
            }
            return this;
        }

        /// <summary>
        /// add shift and set current shift
        /// </summary>
        /// <returns></returns>
        public Staff AddInShift(DateTime? inShiftAt = null)
        {
            var inShift = StaffShift.CreateInShift(this, inShiftAt ?? DateTime.Now);
            if (StaffShifts == null)
            {
                StaffShifts = new List<StaffShift>();
            }

            StaffShifts.Add(inShift);

            CurrentShiftId = inShift.ShiftId;

            return this;
        }

        /// <summary>
        /// add shift and set current shift
        /// </summary>
        /// <returns></returns>
        public Staff AddOutShift(DateTime? outShiftAt = null)
        {
            var outShift = StaffShift.CreateOutShift(this, outShiftAt ?? DateTime.Now);
            if (StaffShifts == null)
            {
                StaffShifts = new List<StaffShift>();

            }

            StaffShifts.Add(outShift);

            CurrentShiftId = outShift.ShiftId;

            return this;
        }

        /// <summary>
        /// any step assigned
        /// </summary>
        public Staff SetAssigned()
        {
            if (JobSteps == null || JobSteps.Count == 0)
            {
                IsAssigned = false;
            }
            else
            {
                IsAssigned = JobSteps.Any(w => w.Status != StepStatus.Todo &&
                    w.Status != StepStatus.Approved && w.Status != StepStatus.Rejected);
            }
            return this;
        }


        /// <summary>
        /// is active when current shift in 1,2,3
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            var currentShift = GetCurrentShift();
            return currentShift == Shift.Shift1 || currentShift == Shift.Shift2 || currentShift == Shift.Shift3;
        }

        public Shift GetCurrentShift()
        {
            if (CurrentShiftId == 0)
            {
                return Shift.None;
            }

            return Enumeration.FromValue<Shift>(CurrentShiftId);
        }

        public bool HasRole(Role role)
        {
            if (Roles != null && Roles.Any())
            {
                return Roles.Any(r => r.Id == role.Id);
            }

            return false;
        }
    }
}
