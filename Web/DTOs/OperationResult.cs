namespace Web.DTOs
{
    public class OperationResult
    {
        public bool Succeeded { get; private set; }
        public string? Message { get; private set; }
        public bool IsRestored { get; private set; }

        public static OperationResult Ok(string? message = null) =>
            new() { Succeeded = true, Message = message };

        public static OperationResult Fail(string message) =>
            new() { Succeeded = false, Message = message };

        public static OperationResult Restored(string message) =>
            new() { Succeeded = true, IsRestored = true, Message = message };
    }
}
