using Infrastructure.Models;

namespace Application.Operations
{
    /// <summary>
    /// PreProcessing prepare command -> Processing execute command -> PostProcess find route to toOperation
    /// </summary>
    public abstract class OperationBase
    {
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
        public abstract OperationBase IsValid();
        public abstract string GetMessage();

        public abstract InvokeResult GetInvokeResult();
    }
}
