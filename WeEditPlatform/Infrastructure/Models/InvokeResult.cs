using Domain;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using System.Text;

namespace Infrastructure.Models
{
    public class InvokeResult
    {
        public bool _success { get; set; }
        private object? _result { get; set; }
        private string? _message { get; set; }

        private StringBuilder _messageBuilder { get; set; }
        private Operation? _operation { get; set; }

        public InvokeResult(bool success, Operation? operation = null, string? message = "", string? result = "")
        {
            _operation = operation;
            _success = success;
            _result = result;
            _messageBuilder = new StringBuilder();
            _message = _messageBuilder.ToString();
        }

        public InvokeResult ConcatMessageAndSetResult(InvokeResult invokeResult)
        {
            AddMessage(invokeResult.GetMessage());
            SetResult(invokeResult.GetResult());

            return this;
        }

        public InvokeResult AddMessage(string message)
        {
            if (_operation != null)
            {
                _messageBuilder.AppendLine($"[Operation {_operation.ExecutionName}, Id: {_operation.Id}]  {message}");
            }
            else
            {
                _messageBuilder.AppendLine($"[Operation ]  {message}");
            }

            return this;
        }

        public string GetMessage()
        {
            _message = _messageBuilder.ToString();

            return _message;
        }

        public bool IsSuccess()
        {
            return _success;
        }

        public void SetSuccessFalse()
        {
            _success = false;
        }

        public void SetSuccessTrue()
        {
            _success = true;
        }

        public string SerializeResult()
        {
            return JsonConvert.SerializeObject(_result);
        }

        public object DeserializeResult(string result)
        {
            return JsonConvert.DeserializeObject(result);
        }

        public object GetResult()
        {
            return _result;
        }

        public InvokeResult SetResult(object result)
        {
            _result = result;

            return this;
        }

    }
}
