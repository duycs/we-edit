using Application.Queries;
using Domain;
using Infrastructure.Events;
using Infrastructure.Models;
using Infrastructure.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Application.Operations.Actions.AssignAction
{
    public class AssignActionCommandHandler : IRequestHandler<AssignActionCommand, InvokeResult>
    {
        private readonly IRepositoryService _repositoryService;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ILogger<AssignActionCommandHandler> _logger;

        public AssignActionCommandHandler(IRepositoryService repositoryService, IEventDispatcher eventDispatcher,
            ILogger<AssignActionCommandHandler> logger)
        {
            _repositoryService = repositoryService;
            _eventDispatcher = eventDispatcher;
            _logger = logger;
        }

        /// <summary>
        ///  default invoke resturn success true, exception return false
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<InvokeResult> Handle(AssignActionCommand request, CancellationToken cancellationToken)
        {
            // default success, false if have an exception 
            var invokeResult = new InvokeResult(true);

            try
            {
                invokeResult.AddMessage("AssignAction handler");

                // Filter JobStep action
                var filterJobStepIds = _repositoryService.List<JobStep>(request.RawSqlFilterJobStep).Select(s => s.Id).ToArray();
                var filterJobSteps = _repositoryService.List<JobStep>(filterJobStepIds);

                // And Not Expired time
                if (!string.IsNullOrEmpty(request.ValidJobStepIsExpriedMethod))
                {
                    filterJobSteps = filterJobSteps.Where(s => !s.IsExpried()).ToList();
                }

                // Filter Staff action
                var filterStaffIds = _repositoryService.List<Staff>(request.RawSqlFilterStaff).Select(s => s.Id).ToArray();
                var filterStaffs = _repositoryService.List(filterStaffIds, new StaffSpecification(true)).ToList();

                // Assign Action execute from output of FilterJobStep and FilterStaff actions
                if (filterJobSteps == null || !filterJobSteps.Any() || filterStaffs == null || !filterStaffs.Any())
                {
                    invokeResult.AddMessage($"filterJobSteps count: {filterJobSteps?.Count}, filterStaffs count: {filterStaffs?.Count}");

                    _logger.LogInformation(invokeResult.GetMessage());

                    return Task.FromResult(invokeResult);
                }

                // Assign action
                var matchingAssignSetting = JsonConvert.DeserializeObject<MatchingAssignSetting>(request.MatchingAssignSetting);
                var steps = _repositoryService.All<Step>();
                var assignedStaffs = new List<Staff>();
                while (filterStaffs != null && filterStaffs.Any())
                {
                    invokeResult.AddMessage($"In While filterStaffs count : {filterStaffs?.Count}");

                    foreach (var jobStep in filterJobSteps)
                    {
                        var step = steps.FirstOrDefault(s => s.Id == jobStep.StepId);

                        // And Group matching
                        if (matchingAssignSetting != null && matchingAssignSetting.IsGroupMatching)
                        {
                            filterStaffs = filterStaffs.Where(s => s.Groups != null && s.Groups.Select(g => g.Id).Contains(step.GroupId)).ToList();
                        }

                        // And ProductLevel matching
                        if (matchingAssignSetting != null && matchingAssignSetting.IsProductLevelMatching)
                        {
                            filterStaffs = filterStaffs.Where(s => s.ProductLevels != null &&
                                s.ProductLevels.Contains(step.ProductLevel)).ToList();
                        }

                        if (filterStaffs != null && filterStaffs.Any())
                        {
                            var matchedStaff = filterStaffs.FirstOrDefault();

                            // Assign staff to step, set status Assigned
                            jobStep.SetWorkerAtShift(matchedStaff.Id, matchedStaff.GetCurrentShift().Id);
                            jobStep.UpdateStatus(StepStatus.Assigned);
                            jobStep.SetEstimationInSeconds(step.EstimationInSeconds);
                            matchedStaff.SetAssigned();

                            // save update assigned
                            _repositoryService.Update(jobStep);
                            _repositoryService.Update(matchedStaff);
                            _repositoryService.SaveChanges();

                            // remove assigned staff from filterStaffs
                            filterStaffs.Remove(matchedStaff);

                            assignedStaffs.Add(matchedStaff);
                        }
                    }
                }

                invokeResult.AddMessage($"assignedStaffIds: {string.Join(",", assignedStaffs.Select(s => s.Id))}");

                _logger.LogInformation(invokeResult.GetMessage());

                return Task.FromResult(invokeResult);
            }
            catch (Exception ex)
            {
                invokeResult.AddMessage(ex.Message.ToString()).SetSuccessFalse();

                _logger.LogError(invokeResult.GetMessage());

                return Task.FromResult(invokeResult);
            }
        }

    }
}
