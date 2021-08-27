namespace FileUploader.Helpers
{
    public interface IYaDiskPathParameterService
    {
        string GetUploadPath(string fileName);
    }
}