using Application.Operations.Actions.AssignAction;
using Domain;
using Infrastructure.Commands;
using Infrastructure.Extensions;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Application.Operations.Actions.GetJobStepAction
{
    public class GetJobStepAction : OperationBase
    {
        public GetJobStepAction(Operation operation, InvokeResult invokeResult,
            ILogger logger, ICommandDispatcher commandDispatcher)
            : base(operation, invokeResult, logger, commandDispatcher)
        {
        }

        public override OperationBase PreProcessing()
        {
            InvokeResult.AddMessage("PreProcessing");

            if (Operation == null || Operation.Settings == null || !Operation.Settings.Any())
            {
                InvokeResult.AddMessage($"Not found any Settings").SetSuccessFalse();

                Logger.LogError(InvokeResult.GetMessage());

                return this;
            }

            var settings = Operation.Settings.ToList();

            Command = new GetJobStepActionCommand(settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.RawSqlFilterJobStep)));

            InvokeResult.AddMessage($"Map Settings to Command, mapping count: {settings.Count}").SetSuccessTrue();

            return this;
        }

        public override async Task<OperationBase> Processing()
        {
            InvokeResult.AddMessage("Processing");

            var invokeResultCommand = await CommandDispatcher.SendGetResponse(Command);

            InvokeResult.ConcatMessageAndSetResult(invokeResultCommand);

            var jobSteps = (List<JobStep>)invokeResultCommand.GetResult();

            InvokeResult.AddMessage($"Processing result: {JsonConvert.SerializeObject(jobSteps)}");

            InvokeResult.SetSuccessTrue();

            return this;
        }

        public override OperationBase PostProcessing()
        {
            InvokeResult.AddMessage("PostProcessing").SetSuccessTrue();

            return this;
        }

    }
}
