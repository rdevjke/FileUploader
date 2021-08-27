using FileUploader.Services;
using Microsoft.Extensions.Options;
using System.Text;

namespace FileUploader.Helpers
{
    public class YaDiskPathParameterService : IYaDiskPathParameterService
    {
        private readonly string _basePath;
        private readonly ArgsOptions _options;

        public YaDiskPathParameterService(IOptionsMonitor<ArgsOptions> optionsAccessor)
        {
            _options = optionsAccessor.CurrentValue;
            _basePath = string.Join("%2F", _options.InputPath.Split('\\'));
        }

        public string GetUploadPath(string fileName)
        {
            var builder = new StringBuilder();
            builder.Append("?path=");
            builder.Append(_basePath);
            builder.Append("%2F");
            builder.Append(fileName);
            return builder.ToString();
        }
    }
}
