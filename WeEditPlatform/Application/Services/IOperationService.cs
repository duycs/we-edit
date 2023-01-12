using Application.Models;
using Domain;

namespace Application.Services
{
    public interface IOperationService
    {
        /// <summary>
        /// Add operation with input settings
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Operation CreateOperation(CreateOperationVM request);

        Operation GetOperation(int id, bool isInclude);
        Operation UpdateOperation(UpdateOperationVM request);
        void RemoveOperation(int id);

        void Invoke(int id);
    }
}
