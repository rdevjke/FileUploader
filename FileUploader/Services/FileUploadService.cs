using FileUploader.Helpers;
using FileUploader.Models;
using FileUploader.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FileUploader
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IYaDiskPathParameterService _yaDiskPathParameterService;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly ArgsOptions _options;

        public FileUploadService(HttpClient httpClient, IYaDiskPathParameterService yaDiskPathParameterService, IConfiguration config, IOptionsMonitor<ArgsOptions> optionsAccessor, ILoggerFactory logger)
        {
            _httpClient = httpClient;
            _yaDiskPathParameterService = yaDiskPathParameterService;
            _logger = logger.CreateLogger<FileUploadService>();
            _options = optionsAccessor.CurrentValue;
            _config = config;
        }

        private async Task<UploadUrl> GetUploadUrlAsync(string path)
        {
            var response = await _httpClient.GetAsync("https://cloud-api.yandex.net/v1/disk/resources/upload" + path);
            var data = JsonSerializer.Deserialize<UploadUrl>(await response.Content.ReadAsStringAsync());
            if (string.IsNullOrEmpty(data.Href))
                throw new Exception("Не удалось получить путь для загрузки. Возможно файс с таким именем уже загружен.");
            return data;
        }

        public void Run()
        {
            var token = _config.GetValue<string>("token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
            var directory = new DirectoryInfo(_options.OutputPath);
            var files = directory.GetFiles();
            var result = Parallel.ForEach<FileInfo>(files, async file =>
            {
                _logger.LogInformation($"Загрузка файла - {file.Name}");
                using var content = File.OpenRead(file.FullName);
                try
                {
                    var path = await GetUploadUrlAsync(_yaDiskPathParameterService.GetUploadPath(file.Name));
                    var response = await _httpClient.PutAsync(path.Href, new StreamContent(content));
                    _logger.LogInformation($"{file.Name} - загружен");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Ошибка при загрузке файла {file.Name}! - {ex.Message}");
                }
            });
        }
    }
}
