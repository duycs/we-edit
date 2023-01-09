using Application.Models;
using Application.Services;
using Domain;
using FluentValidation;
using Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;

namespace Testing
{
    public class StaffServiceTests : TestBase
    {
        private IStaffService _staffService;

        [OneTimeSetUp]
        public void Init()
        {
            _staffService = _serviceProvider.GetService<IStaffService>();

            Init_First_Staff_Data();
        }

        [TearDown]
        public void Clean()
        {
            //_productionContext.Database.EnsureDeleted();
            //_productionContext.Dispose();
        }

        [Test]
        [TestCase("cf6c87f9-a60a-42ad-928b-2fb537ef5ebf", "Fullname test 1", "account-test-1", "emailtest1@gmail.com")]
        public void Create_New_Staff_Success(string userId, string fullName, string account, string email)
        {
            var staffCreated = _staffService.CreateStaff(new CreateStaffVM()
            {
                UserId = userId,
                FullName = fullName,
                Account = account,
                Email = email,
            });

            Assert.NotNull(staffCreated);
            Assert.NotZero(staffCreated.Id);
        }

        [Test]
        [TestCase("cf6c87f9-a60a-42ad-928b-2fb537ef5ebf", "Fullname test 1", "account-test-1", "emailtest1@gmail.com",
            new int[] { 1, 2 }, new int[] { 1, 2 }, new int[] { 1, 2 },
            new int[] { 3, 4 }, new int[] { 3, 4 }, new int[] { 3, 4 })]
        public void Create_New_Staff_Then_Update_Success(string userId, string fullName, string account, string email, int[] roleIds, int[] groupIds, int[] productLevelIds,
            int[] updateRoleIds, int[] updateGroupIds, int[] updateProductLevelIds)
        {
            var staffCreated = _staffService.CreateStaff(new CreateStaffVM()
            {
                UserId = userId,
                FullName = fullName,
                Account = account,
                Email = email,
                RoleIds = roleIds,
                GroupIds = groupIds,
                ProductLevelIds = productLevelIds
            });

            Assert.NotNull(staffCreated);
            Assert.NotZero(staffCreated.Id);
            Assert.True(staffCreated.Roles.All(r => roleIds.Contains(r.Id)));
            Assert.True(staffCreated.Groups.All(r => groupIds.Contains(r.Id)));
            Assert.True(staffCreated.ProductLevels.All(r => productLevelIds.Contains(r.Id)));

            // TODO: 
            var staffUpdated = _staffService.UpdateStaff(new UpdateStaffVM()
            {
                StaffId = staffCreated.Id,
                RoleIds = updateRoleIds,
                GroupIds = updateGroupIds,
                ProductLevelIds = updateProductLevelIds
            });

            Assert.NotNull(staffUpdated);
            Assert.True(staffUpdated.Roles.All(r => updateRoleIds.Contains(r.Id)));
            Assert.True(staffUpdated.Groups.All(r => updateGroupIds.Contains(r.Id)));
            Assert.True(staffUpdated.ProductLevels.All(r => updateProductLevelIds.Contains(r.Id)));
        }

        [Test]
        [TestCase("cf6c87f9-a60a-42ad-928b-2fb537ef5ebf", "Fullname wrong-email", "account-wrong-email", "wrong-email")]
        public void Create_New_Staff_Fail_Email_Validate(string userId, string fullName, string account, string email)
        {
            var createStaffVM = new CreateStaffVM()
            {
                UserId = userId,
                FullName = fullName,
                Account = account,
                Email = email,
            };

            Assert.That(() => _staffService.CreateStaff(createStaffVM), Throws.TypeOf<ValidationException>());
        }

        [Test]
        [TestCase("Fullname test 2", "account-test-2", "emailtest1@gmail.com", new int[] { 1, 2 })]
        public void Create_New_Staff_With_Roles_Success(string fullName, string account, string email, int[] roleIds)
        {
            // Arrange
            var staffVM = new CreateStaffVM()
            {
                FullName = fullName,
                Account = account,
                Email = email,
                RoleIds = roleIds
            };

            // Act
            var staffCreated = _staffService.CreateStaff(staffVM);

            // Assert
            Assert.NotNull(staffCreated);
            Assert.NotZero(staffCreated.Id);
        }

        [Test]
        [TestCase(new[] { 1, 2 }, 1)]
        public void Add_Roles_For_Staff_Success(int[] roleIds, int staffId)
        {
            _staffService.AddRolesForStaff(new AddRolesForStaffVM()
            {
                RoleIds = roleIds,
                StaffId = staffId
            });

            // assert
            var staff = _staffService.FindStaff(staffId, true);
            Assert.NotNull(staff);
            Assert.NotNull(staff.Roles);

            var staffRoles = _repositoryService.List<StaffRole>(w => w.StaffId == staffId && roleIds.Contains(w.RoleId));
            Assert.True(staffRoles.Any(w => w.StaffId == staffId && roleIds.Contains(w.RoleId)));
        }

        [Test]
        [TestCase(new[] { 1, 2 }, 1)]
        public void Remove_Roles_For_Staff_Success(int[] roleIds, int staffId)
        {
            _staffService.RemoveRolesForStaff(new RemoveRolesForStaffVM() { RoleIds = roleIds, StaffId = staffId });

            // assert
            var staffRoles = _repositoryService.List<StaffRole>(w => w.StaffId == staffId && roleIds.Contains(w.RoleId));
            Assert.False(staffRoles.Any(w => w.StaffId == staffId && roleIds.Contains(w.RoleId)));
        }

        [Test]
        [TestCase(2)]
        public void Remove_Staff_Success(int staffId)
        {
            // arrage
            // already init first Staff
            // add second Staff
            _repositoryService.Add(Staff.Create("99ca2e12-4268-4297-aaa1-6f32cb0db89b", "FullName second", "accountSecond", "emailSecond@gmail.com"));
            _repositoryService.SaveChanges();

            _staffService.RemoveStaff(staffId);

            // assert
            var staffRoles = _repositoryService.List<StaffRole>(w => w.StaffId == staffId);
            Assert.False(staffRoles.Any(w => w.StaffId == staffId));
        }

        [Test]
        [TestCase(1)]
        public void Staff_In_Shift_Success(int staffId)
        {
            // arrage
            // already init first Staff

            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffId });

            // assert
            var staffShifts = _staffService.GetLastInShifts(1, 10);
            Assert.NotNull(staffShifts);
            Assert.AreEqual(1, staffShifts.Count());
        }

        [Test]
        [TestCase(1)]
        public void Staff_In_Shift_Many_Time_Success(int staffId)
        {
            // arrage
            // already init first Staff

            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffId });
            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffId });

            // assert
            var staffShifts = _staffService.GetLastInShifts(1, 10);
            Assert.NotNull(staffShifts);
            Assert.AreEqual(2, staffShifts.Count());
        }

        [Test]
        [TestCase(1)]
        public void Staff_Out_Shift_Success(int staffId)
        {
            _staffService.SetStaffOutShift(new StaffOutShiftVM() { StaffId = staffId });

            // assert
            var staffShifts = _staffService.GetLastOutShifts(1, 10);
            Assert.NotNull(staffShifts);
            Assert.AreEqual(1, staffShifts.Count());
        }

        [Test]
        [TestCase(1)]
        public void Staff_Out_Shift_Many_Time_Success(int staffId)
        {
            _staffService.SetStaffOutShift(new StaffOutShiftVM() { StaffId = staffId });
            _staffService.SetStaffOutShift(new StaffOutShiftVM() { StaffId = staffId });

            // assert
            var staffShifts = _staffService.GetLastOutShifts(1, 10);
            Assert.NotNull(staffShifts);
            Assert.AreEqual(2, staffShifts.Count());
        }

        [Test]
        [TestCase(new int[] { 1, 2, 3 }, 1)]
        public void Add_ProductLevel_For_Staff_Success(int[] productLevelIds, int staffId)
        {
            // arrage
            // already init first Staff

            _staffService.AddProductLevelForStaff(new AddProductLevelsForStaffVM() { ProductLevelIds = productLevelIds, StaffId = staffId });

            // assert
            var staffProductLevels = _repositoryService.List<StaffProductLevel>(w => w.StaffId == staffId);
            Assert.NotNull(staffProductLevels);
            Assert.AreEqual(3, staffProductLevels.Count(w => productLevelIds.Contains(w.ProductLevelId) && w.StaffId == staffId));
        }

        [Test]
        [TestCase(new int[] { 1, 2, 3 }, 1)]
        public void Remove_ProductLevel_For_Staff(int[] productLevelIds, int staffId)
        {
            // arrage
            // already init first Staff

            _staffService.RemoveProductLevelForStaff(new RemoveProductLevelsForStaffVM() { ProductLevelIds = productLevelIds, StaffId = staffId });

            // assert
            var staffProductLevels = _repositoryService.List<StaffProductLevel>(w => !productLevelIds.Contains(w.ProductLevelId) && w.StaffId == staffId);
            Assert.NotNull(staffProductLevels);
            Assert.False(staffProductLevels.Any());
        }

        private void Init_First_Staff_Data()
        {
            // first Staff id 1
            _repositoryService.Add(Staff.Create("cf6c87f9-a60a-42ad-928b-2fb537ef5ebf", "FullName first", "accountFirst", "emailFirst@gmail.com"));
            _repositoryService.SaveChanges();
        }
    }
}
