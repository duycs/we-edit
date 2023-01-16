using Application.Operations.Actions.AssignAction;
using Domain;
using Infrastructure.Models;
using Infrastructure.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Application.Operations.Actions.GetJobStepAction
{
    public class GetJobStepCommandHandler : IRequestHandler<GetJobStepActionCommand, InvokeResult>
    {
        private readonly IRepositoryService _repositoryService;
        private readonly ILogger<AssignActionCommandHandler> _logger;

        public GetJobStepCommandHandler(IRepositoryService repositoryService, ILogger<AssignActionCommandHandler> logger)
        {
            _repositoryService = repositoryService;
            _logger = logger;
        }

        public Task<InvokeResult> Handle(GetJobStepActionCommand request, CancellationToken cancellationToken)
        {
            var invokeResult = new InvokeResult(true);

            try
            {
                invokeResult.AddMessage("GetJobStep handler");

                var jobsteps = _repositoryService.List<JobStep>(request.RawSqlFilterJobStep).ToList();

                invokeResult.SetResult(jobsteps).SetSuccessTrue();

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
