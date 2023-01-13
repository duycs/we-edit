namespace Infrastructure.Models
{
    public class InvokeResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public InvokeResult(bool success, string? message = "")
        {
            Success = success;
            Message = message ?? "";
        }

        public void AddMessage(string message)
        {
            Message += $"\n{message}";
        }
    }
}
