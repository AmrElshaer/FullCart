using Application.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

public interface IFileAppService
{
    Task<string> UploadFileAsync(FileType type, IFormFile file);

    Task DeleteFileAsync(FileType type, string fileName);

    Task<Stream> GetFileAsync(FileType type, string fileName);
    public Task<string> UploadFileAsync(FileType type, IFormFile file, DirectoryType directory);
}
