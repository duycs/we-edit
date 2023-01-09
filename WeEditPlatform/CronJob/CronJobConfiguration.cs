using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace CronJob
{
    public static class CronJobConfiguration
    {
        /// <summary>
        /// Ref: https://www.quartz-scheduler.net/documentation/quartz-3.x/packages/microsoft-di-integration.html#di-aware-job-factories
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddCronJobs(this IServiceCollection services, IConfiguration configuration)
        {
            // base configuration from appsettings.json
            //services.Configure<QuartzOptions>(configuration.GetSection("Quartz"));

            // if you are using persistent job store, you might want to alter some options
            services.Configure<QuartzOptions>(options =>
            {
                //options.Scheduling.IgnoreDuplicates = true; // default: false
                options.Scheduling.OverWriteExistingData = true; // default: true
            });

            services.AddQuartz(q =>
            {
                // handy when part of cluster or you want to otherwise identify multiple schedulers
                //q.SchedulerId = "Scheduler-Core";

                // we take this from appsettings.json, just show it's possible
                q.SchedulerName = "Quartz ASP.NET Core Production Scheduler";

                // as of 3.3.2 this also injects scoped services (like EF DbContext) without problems
                q.UseMicrosoftDependencyInjectionJobFactory();

                // or for scoped service support like EF Core DbContext
                //q.UseMicrosoftDependencyInjectionScopedJobFactory();

                // these are the defaults
                q.UseSimpleTypeLoader();

                q.UseInMemoryStore();

                //q.UsePersistentStore(s =>
                //{
                //    s.PerformSchemaValidation = true; // default
                //    s.UseProperties = true; // preferred, but not default
                //    s.RetryInterval = TimeSpan.FromSeconds(15);
                //    s.UseMySql(sql =>
                //    {
                //    });
                //    s.UseJsonSerializer();
                //    s.UseClustering(c =>
                //    {
                //        c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                //        c.CheckinInterval = TimeSpan.FromSeconds(10);
                //    });
                //});

                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });

                q.AddJob<AutoAssignStaffToStepJob>(opts => opts.WithIdentity(AutoAssignStaffToStepJob.AUTO_ASSIGN_STAFF_TO_STEP_KEY));
                q.AddTrigger(opts => opts
                    .ForJob(AutoAssignStaffToStepJob.AUTO_ASSIGN_STAFF_TO_STEP_KEY)
                    .WithIdentity("autoAssignStaffToStepKey-trigger")
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(AutoAssignStaffToStepJob.INTERVAL_SECONDS)
                        .RepeatForever()));

                // also add XML configuration and poll it for changes
                //q.UseXmlSchedulingConfiguration(x =>
                //{
                //    x.Files = new[] { "~/quartz_jobs.config" };
                //    x.ScanInterval = TimeSpan.FromSeconds(2);
                //    x.FailOnFileNotFound = true;
                //    x.FailOnSchedulingError = true;
                //});


                // auto-interrupt long-running job
                q.UseJobAutoInterrupt(options =>
                {
                    // this is the default
                    options.DefaultMaxRunTime = TimeSpan.FromMinutes(5);
                });

            });

            services.AddTransient<AutoAssignStaffToStepJob>();

            // Quartz.Extensions.Hosting allows you to fire background service that handles scheduler lifecycle
            services.AddQuartzHostedService(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });
        }
    }
}
