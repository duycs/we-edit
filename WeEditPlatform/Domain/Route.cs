namespace Domain
{
    /// <summary>
    /// eg: multiple routes O1 vs O2
    /// O1 -> O2: to
    /// O2 -> O1: to
    /// O1 -> O1: retry
    /// </summary>
    public class Route : EntityBase
    {
        public int FromOperationId { get; set; }
        public Operation FromOperation { get; set; }

        /// Ignore ToOperation, have to find by ToOperationId to set toOperation
        private Operation _toOperation { get; set; }

        public int ToOperationId { get; set; }
        public string? Description { get; set; }

        public static Route Create(int fromOperationId, int toOperationId, string? description = "")
        {
            return new Route()
            {
                FromOperationId = fromOperationId,
                ToOperationId = toOperationId,
                Description = description
            };
        }

        public Route Update(int fromOperationId, int toOperationId, string? description = "")
        {
            if (fromOperationId > 0)
            {
                FromOperationId = fromOperationId;
            }

            if (toOperationId > 0)
            {
                ToOperationId = toOperationId;
            }

            if (!string.IsNullOrEmpty(description))
            {
                Description = description;
            }

            return this;
        }

        public void SetToOperation(Operation toOperation)
        {
            _toOperation = toOperation;
        }

        public Operation GetToOperation()
        {
            return _toOperation;
        }

    }
}
