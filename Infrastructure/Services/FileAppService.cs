using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Interfaces.File;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class FileAppService : IFileAppService
{
    public Task<string> UploadFileAsync(FileType type, IFormFile file)
    {
        return Task.FromResult($"{type}-{Guid.NewGuid()}");
    }

    public async Task DeleteFileAsync(FileType type, string fileName)
    {
        await Task.CompletedTask;
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