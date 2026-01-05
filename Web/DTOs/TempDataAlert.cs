using System.Text.Json.Serialization;

namespace Web.DTOs
{
    public class TempDataAlert
    {
        public record AlertType
        {
            public static readonly AlertType Sucess = new("success");
            public static readonly AlertType Warning = new("warning");
            public static readonly AlertType Danger = new("danger");
            public static readonly AlertType Info = new("info");

            public string Value { get; }
            [JsonConstructor]
            public AlertType(string value) => Value = value;
        }

        public required string Message { get; set; } = string.Empty;
        public required AlertType Type { get; set; } = AlertType.Info;
    }
}
