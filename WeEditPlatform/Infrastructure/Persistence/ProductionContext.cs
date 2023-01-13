using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences
{
    public class ProductionContext : DbContext, IDatabaseService
    {
        private string ConnectionString { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<JobStep> JobSteps { get; set; }
        public DbSet<ProductLevel> ProductLevels { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<StaffShift> StaffShifts { get; set; }
        public DbSet<StaffProductLevel> StaffProductLevels { get; set; }
        public DbSet<StaffRole> StaffRoles { get; set; }
        public DbSet<StaffGroup> StaffGroups { get; set; }

        public DbSet<Operation> Operations { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<OperationSetting> OperationSettings { get; set; }

        public DbSet<Route> Routes { get; set; }

        /// <summary>
        /// Must concreate first for ef migrations
        /// </summary>
        public ProductionContext() { }

        public ProductionContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public ProductionContext(DbContextOptions<ProductionContext> options) : base(options) { }

        DbSet<T> IDatabaseService.GetDbSet<T>()
        {
            return Set<T>();
        }

        Task IDatabaseService.SaveChanges()
        {
            return Task.FromResult(base.SaveChanges());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // TODO: Hardcode connection string for migration CLI 
                // Set Allow User Variables = True, For solve issue when migrations: MySql.Data.MySqlClient.MySqlException (0x80004005): Parameter @X must be defined.

            }
        }

        /// <summary>
        /// ref: https://docs.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().ToTable("Customers");

            // uuid userId from sso
            modelBuilder.Entity<Staff>().Property(s => s.UserId).IsRequired().HasMaxLength(36);
            modelBuilder.Entity<Staff>().HasIndex(s => s.UserId).IsUnique();

            //Many Staff with many ProductLevel
            modelBuilder.Entity<Staff>()
              .HasMany(i => i.ProductLevels)
              .WithMany(i => i.Staffs)
              .UsingEntity<StaffProductLevel>(
              j => j
                  .HasOne(w => w.ProductLevel)
                  .WithMany(w => w.StaffProductLevels)
                  .HasForeignKey(w => w.ProductLevelId)
                  .OnDelete(DeleteBehavior.NoAction),
              j => j
                  .HasOne(w => w.Staff)
                  .WithMany(w => w.StaffProductLevels)
                  .HasForeignKey(w => w.StaffId)
                  .OnDelete(DeleteBehavior.NoAction),
              j =>
              {
                  j.Ignore(w => w.Id).HasKey(w => new { w.StaffId, w.ProductLevelId });
              });

            // Job has many steps, one ProductLevel, 
            modelBuilder.Entity<Job>()
            .HasMany(i => i.Steps)
            .WithMany(i => i.Jobs)
            .UsingEntity<JobStep>(
            j => j
                .HasOne(w => w.Step)
                .WithMany(w => w.JobSteps)
                .HasForeignKey(w => w.StepId)
                .OnDelete(DeleteBehavior.NoAction),
            j => j
                .HasOne(w => w.Job)
                .WithMany(w => w.JobSteps)
                .HasForeignKey(w => w.JobId)
                .OnDelete(DeleteBehavior.Cascade),
            j =>
            {
                j.HasKey(w => w.Id).HasName("Id");
                //j.Ignore(w => w.Id).HasKey(w => new { w.JobId, w.StepId });
            });

            // Step has one ProductLevel
            modelBuilder.Entity<Step>().HasOne(w => w.ProductLevel).WithMany(w => w.Steps).HasForeignKey(w => w.ProductLevelId);

            // Step of Job has one Worker
            modelBuilder.Entity<JobStep>().HasOne(w => w.Worker).WithMany(w => w.JobSteps).HasForeignKey(w => w.WorkerId);
            modelBuilder.Entity<JobStep>().HasOne(w => w.Shift).WithMany(w => w.JobSteps).OnDelete(DeleteBehavior.NoAction)
                .HasForeignKey(w => w.ShiftId).OnDelete(DeleteBehavior.NoAction);

            // Job has one productLevel
            modelBuilder.Entity<Job>().HasOne(w => w.ProductLevel).WithMany(w => w.Jobs).HasForeignKey(w => w.ProductLevelId);

            // Enumration Shift to table
            modelBuilder.Entity<Shift>().ToTable("Shifts").HasKey(c => c.Id);
            modelBuilder.Entity<Shift>().Property(c => c.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();
            modelBuilder.Entity<Shift>().Property(c => c.Name)
                 .HasMaxLength(250)
                 .IsRequired();

            // May Staff with many Shift
            modelBuilder.Entity<Staff>()
           .HasMany(i => i.Shifts)
           .WithMany(i => i.Staffs)
           .UsingEntity<StaffShift>(
           j => j
               .HasOne(w => w.Shift)
               .WithMany(w => w.StaffShifts)
               //.HasForeignKey(w => w.ShiftId)
               .OnDelete(DeleteBehavior.NoAction),
           j => j
               .HasOne(w => w.Staff)
               .WithMany(w => w.StaffShifts)
               //.HasForeignKey(w => w.StaffId)
               .OnDelete(DeleteBehavior.Cascade),
           j =>
           {
               j.HasKey(w => w.Id).HasName("Id");
           });

            // Enumration Role to table
            modelBuilder.Entity<Role>().ToTable("Roles").HasKey(c => c.Id);
            modelBuilder.Entity<Role>().Property(c => c.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();
            modelBuilder.Entity<Role>().Property(c => c.Name)
                 .HasMaxLength(250)
                 .IsRequired();

            // Groups
            modelBuilder.Entity<Group>().ToTable("Groups").HasKey(c => c.Id);
            modelBuilder.Entity<Group>().Property(c => c.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();
            modelBuilder.Entity<Group>().Property(c => c.Name)
                 .HasMaxLength(250)
                 .IsRequired();


            // Staff has many roles
            modelBuilder.Entity<Staff>()
           .HasMany(i => i.Roles)
           .WithMany(i => i.Staffs)
           .UsingEntity<StaffRole>(
           j => j
               .HasOne(w => w.Role)
               .WithMany(w => w.StaffRoles)
               .HasForeignKey(w => w.RoleId)
               .OnDelete(DeleteBehavior.NoAction),
           j => j
               .HasOne(w => w.Staff)
               .WithMany(w => w.StaffRoles)
               .HasForeignKey(w => w.StaffId)
               .OnDelete(DeleteBehavior.Cascade),
           j =>
           {
               j.Ignore(w => w.Id).HasKey(w => new { w.StaffId, w.RoleId });
           });


            // Staff has many Groups
            modelBuilder.Entity<Staff>()
           .HasMany(i => i.Groups)
           .WithMany(i => i.Staffs)
           .UsingEntity<StaffGroup>(
           j => j
               .HasOne(w => w.Group)
               .WithMany(w => w.StaffGroups)
               .HasForeignKey(w => w.GroupId)
               .OnDelete(DeleteBehavior.NoAction),
           j => j
               .HasOne(w => w.Staff)
               .WithMany(w => w.StaffGroup)
               .HasForeignKey(w => w.StaffId)
               .OnDelete(DeleteBehavior.Cascade),
           j =>
           {
               j.Ignore(w => w.Id).HasKey(w => new { w.StaffId, w.GroupId });
           });


            // Many Operations with many Settings
            modelBuilder.Entity<Operation>()
              .HasMany(i => i.Settings)
              .WithMany(i => i.Operations)
              .UsingEntity<OperationSetting>(
              j => j
                  .HasOne(w => w.Setting)
                  .WithMany(w => w.OperationSettings)
                  .HasForeignKey(w => w.SettingId)
                  .OnDelete(DeleteBehavior.NoAction),
              j => j
                  .HasOne(w => w.Operation)
                  .WithMany(w => w.OperationSettings)
                  .HasForeignKey(w => w.OperationId)
                  .OnDelete(DeleteBehavior.Cascade), // delete operation then delete settings
              j =>
              {
                  j.Ignore(w => w.Id).HasKey(w => new { w.OperationId, w.SettingId });
              });


            // Flow has many Operations
            modelBuilder.Entity<Flow>().ToTable("Flows").HasKey(c => c.Id);
            modelBuilder.Entity<Operation>().HasOne(w => w.Flow).WithMany(w => w.Operations).HasForeignKey(w => w.FlowId);


            // Operation has many Routes, Routes has FromOperation and ToOperation
            modelBuilder.Entity<Route>().ToTable("Routes").HasKey(c => c.Id);
            modelBuilder.Entity<Route>().HasOne(w => w.FromOperation).WithMany(w => w.Routes).HasForeignKey(w => w.FromOperationId);
        }

    }
}
