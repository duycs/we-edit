using Domain;
using Infrastructure.Commands;
using Infrastructure.Extensions;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace Application.Operations.Actions.AssignAction
{
    public class AssignAction : OperationBase
    {
        public AssignAction(
            Operation operation,
            InvokeResult invokeResult,
            ILogger logger,
            ICommandDispatcher commandDispatcher) : base(operation, invokeResult, logger, commandDispatcher)
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

            Command = new AssignActionCommand(
                 settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.MatchingAssignSetting)),
                 settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.RawSqlFilterJobStep)),
                 settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.ValidJobStepIsExpriedMethod)),
                 settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.RawSqlFilterStaff))
                 );

            InvokeResult.AddMessage($"Map Settings to Command, mapping count: {settings.Count}")
                .SetSuccessTrue();

            return this;
        }

        public override async Task<OperationBase> Processing()
        {
            InvokeResult.AddMessage($"Processing");

            var invokeResultCommand = await CommandDispatcher.SendGetResponse(Command);

            InvokeResult.ConcatMessageAndSetResult(invokeResultCommand);

            InvokeResult.SetSuccessTrue();

            return this;
        }

        public override OperationBase PostProcessing()
        {
            InvokeResult.AddMessage($"PostProcessing").SetSuccessTrue();

            return this;
        }
    }
}
