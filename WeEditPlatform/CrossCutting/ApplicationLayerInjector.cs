using Application.Operations.Actions.AssignAction;
using Application.Operations.Actions.CreateWatermarkAction;
using Application.Operations.Actions.GetJobStepAction;
using Application.Services;
using Infrastructure.Commands;
using Infrastructure.Events;
using Infrastructure.Models;
using Infrastructure.Pagging;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CrossCutting
{
    public static class ApplicationLayerInjector
    {
        public static void AddServices(this IServiceCollection services)
        {
            // commands
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddMediatR(typeof(AssignActionCommandHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(GetJobStepCommandHandler).GetTypeInfo().Assembly);
            services.AddScoped<IRequestHandler<AssignActionCommand, InvokeResult>, AssignActionCommandHandler>();
            services.AddScoped<IRequestHandler<GetJobStepActionCommand, InvokeResult>, GetJobStepCommandHandler>();

            services.AddScoped<IRequestHandler<CreateCombineWatermarkCommand>, CreateCombineWatermarkCommandHandler>();
            services.AddScoped<IRequestHandler<CreateTextWatermarkCommand>, CreateTextWatermarkCommandHandler>();
            services.AddScoped<IRequestHandler<CreateImageWatermarkCommand>, CreateImageWatermarkCommandHandler>();

            // services
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<INoteService, NoteService>();
            services.AddScoped<IOperationService, OperationService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IFlowService, FlowService>();
            services.AddScoped<IRouteService, RouteService>();

            // Present
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUriService>(o =>
            {
                var request = o.GetRequiredService<IHttpContextAccessor>().HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new UriService(uri);
            });

            // client services
            services.AddHttpClient<ISSOService>();
            services.AddTransient<ISSOService, SSOService>();
        }
    }
}
