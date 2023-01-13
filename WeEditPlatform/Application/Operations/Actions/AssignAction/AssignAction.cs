using Domain;
using Google.Protobuf;
using Infrastructure.Commands;
using Infrastructure.Exceptions;
using Infrastructure.Extensions;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Application.Operations.Actions.AssignAction
{
    public class AssignAction : OperationBase
    {
        private readonly ILogger _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        private readonly Operation _operation;
        private InvokeResult _invokeResult;
        private CommandResponse<InvokeResult> _command;
        private StringBuilder _message;

        public AssignAction(
            Operation operation,
            ILogger logger,
            ICommandDispatcher commandDispatcher)
        {
            _message = new StringBuilder();
            _invokeResult = new InvokeResult(false);
            _operation = operation;
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        public override OperationBase PreProcessing()
        {
            _message.AppendLine($"Operation {_operation.Id}. PreProcessing");

            if (_operation == null || _operation.Settings == null || !_operation.Settings.Any())
            {
                _message.AppendLine($"Not found any Settings of Operation {_operation.Id}.");

                _logger.LogError(_message.ToString());

                _invokeResult = new InvokeResult(false, _message.ToString());

                return this;
            }

            var settings = _operation.Settings.ToList();

            _command = new AssignActionCommand(
                 settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.MatchingAssignSetting)),
                 settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.RawSqlFilterJobStep)),
                 settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.ValidJobStepIsExpriedMethod)),
                 settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.RawSqlFilterStaff))
                 );

            _invokeResult = new InvokeResult(true, _message.ToString());

            return this;
        }

        public override async Task<OperationBase> Processing()
        {
            _invokeResult = await _commandDispatcher.SendGetResponse(_command);

            _message.AppendLine($"Operation {_operation.Id}. Processing");
            _invokeResult.AddMessage(_message.ToString());

            return this;
        }

        public override OperationBase PostProcessing()
        {
            _message.AppendLine($"Operation {_operation.Id}. PostProcessing");
            _invokeResult.AddMessage(_message.ToString());

            return this;
        }

        public override OperationBase IsValid()
        {
            if (_invokeResult == null)
            {
                throw new OperationException($"Error no _invokeResult");
            }

            if (!_invokeResult.Success)
            {
                throw new OperationException($"Error message: {_invokeResult?.Message}");
            }

            return this;
        }

        public override string GetMessage()
        {
            return _message.ToString();
        }

        public override InvokeResult GetInvokeResult()
        {
            return _invokeResult;
        }
    }
}
