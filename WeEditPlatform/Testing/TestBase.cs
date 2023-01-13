using CrossCutting;
using Domain;
using Infrastructure.Persistences;
using Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;

namespace Testing
{
    public class TestBase
    {
        public IConfiguration _configuration { get; }
        public IServiceCollection _serviceCollection { get; }
        public IServiceProvider _serviceProvider { get; set; }
        public IRepositoryService _repositoryService { get; set; }
        public ProductionContext _productionContext { get; set; }

        public TestBase()
        {
            // Set appsetting config file
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            _configuration = configuration;
            var useInMemoryDb = configuration.GetValue<bool>("UseInMemoryDb");

            // Inject services, database in memory
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddLayersInjector(configuration);

            // Then create serviceProvider
            _serviceProvider = _serviceCollection.BuildServiceProvider();
            _repositoryService = _serviceProvider.GetService<IRepositoryService>();
            //_productionContext = _serviceProvider.GetRequiredService<ProductionContext>();

            // init master data
            if (useInMemoryDb)
            {
                Init_Role_Shift_ProductLevel_Data();
            }
        }

        private void Init_Role_Shift_ProductLevel_Data()
        {
            // roles
            _repositoryService.Add(new[]
            {
                 new Role(1, nameof(Role.Admin)),
                 new Role(2, nameof(Role.CSO)),
                 new Role(3, nameof(Role.Editor)),
                 new Role(8, nameof(Role.QC)),
              });

            // group
            _repositoryService.Add(new[] {
                new Group(1, nameof(Group.Admin)),
                new Group(2, nameof(Group.QC)),
                new Group(3, nameof(Group.HighQuality)),
                new Group(4, nameof(Group.PhotoEditing)),
                new Group(5, nameof(Group.MergeRetouch)),
                new Group(6, nameof(Group.Video)),
                new Group(7, nameof(Group._2D3D)),
            });

            // shifts
            _repositoryService.Add(new[] {
                new Shift(1, nameof(Shift.Shift1)),
                new Shift(2, nameof(Shift.Shift2)),
                new Shift(3, nameof(Shift.Shift3)),
                new Shift(4, nameof(Shift.Out)),
                new Shift(5, nameof(Shift.Free)),
                new Shift(6, nameof(Shift.None)),
            });

            // productLevels
            _repositoryService.Add(new[] {
                ProductLevel.Create("PE-LV1", "Single Exposure Editing", "Chỉnh sửa ảnh đơn"),
                ProductLevel.Create("PE-LV2", "HDR Editing	Chỉnh", "sửa ảnh HDR"),
                ProductLevel.Create("PE-LV3", "Multiple Exposure Manual Blending", "Chỉnh sửa đa ảnh phơi sáng"),
                ProductLevel.Create("PE-LV4", "Flambient Editing", "Chỉnh sửa ảnh phong cách tự nhiên"),
                ProductLevel.Create("PE-LV5", "Editorial style", "Chỉnh sửa ảnh phong cách tạp chí"),
            });

            _repositoryService.SaveChanges();
        }
    }
}
