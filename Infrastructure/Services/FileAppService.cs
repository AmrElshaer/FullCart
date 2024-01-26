using Application.Common.Enums;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class FileAppService:IFileAppService
{
    public Task<string> UploadFileAsync(FileType type, IFormFile file)
    {
        return  Task.FromResult($"{type}-{Guid.NewGuid()}");
    }

    public Task DeleteFileAsync(FileType type, string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> GetFileAsync(FileType type, string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<string> UploadFileAsync(FileType type, IFormFile file, DirectoryType directory)
    {
        throw new NotImplementedException();
    }
}
