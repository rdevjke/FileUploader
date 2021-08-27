using System.Text.Json.Serialization;

namespace FileUploader.Models
{
    public record UploadUrl
    {
        [JsonPropertyName("operation_id")]
        public string OperationId { get; set; }
        [JsonPropertyName("href")]
        public string Href { get; set; }
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("templated")]
        public bool Templated { get; set; }
    }
}
