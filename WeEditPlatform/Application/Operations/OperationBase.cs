using Domain;
using Infrastructure.Commands;
using Infrastructure.Exceptions;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace Application.Operations
{
    /// <summary>
    /// PreProcessing prepare command -> Processing execute command -> PostProcess find route to toOperation
    /// </summary>
    public abstract class OperationBase
    {
        public Operation Operation { get; set; }
        public InvokeResult InvokeResult { get; set; }
        public ILogger Logger { get; }
        public ICommandDispatcher CommandDispatcher { get; }
        public CommandResponse<InvokeResult> Command { get; set; }

        protected OperationBase(
            Operation operation,
            InvokeResult invokeResult,
            ILogger logger,
            ICommandDispatcher commandDispatcher)
        {
            Logger = logger;
            Operation = operation;
            InvokeResult = invokeResult;
            CommandDispatcher = commandDispatcher;
        }

        /// <summary>
        /// Prepare data before processing
        /// </summary>
        /// <returns></returns>
        public abstract OperationBase PreProcessing();

        /// <summary>
        /// Processing execution
        /// eg: CRUD database and calculation processing
        /// </summary>
        /// <returns></returns>
        public abstract Task<OperationBase> Processing();

        /// <summary>
        /// After processing
        /// eg: find route to next Operation
        /// </summary>
        /// <returns></returns>
        public abstract OperationBase PostProcessing();

        /// <summary>
        /// Valid if invokeResult success, else throw exception
        /// </summary>
        /// <returns></returns>
        public OperationBase IsValid()
        {
            if (InvokeResult == null)
            {
                throw new OperationException($"Error no InvokeResult");
            }

            if (!InvokeResult.IsSuccess())
            {
                throw new OperationException($"Error message: {InvokeResult?.GetMessage()}");
            }

            return this;
        }

        public string GetMessage()
        {
            return InvokeResult.GetMessage();
        }

        public InvokeResult GetInvokeResult()
        {
            return InvokeResult;
        }
    }
}
