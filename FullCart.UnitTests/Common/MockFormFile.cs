using Microsoft.AspNetCore.Http;

namespace FullCart.UnitTests.Common;

public class MockFormFile:IFormFile
{
    private readonly string _fileName;
    private readonly string _contentType;
    private readonly long _length;
    private readonly byte[] _content;

    public MockFormFile(string fileName, string contentType, long length)
    {
        _fileName = fileName;
        _contentType = contentType;
        _length = length;
        _content = new byte[length];
    }

    public string ContentType => _contentType;
    public string ContentDisposition => $"form-data; name=\"file\"; filename=\"{_fileName}\"";
    public IHeaderDictionary Headers => new HeaderDictionary();
    public long Length => _length;
    public string Name => "file";
    public string FileName => _fileName;

    public void CopyTo(Stream target)
    {
        new MemoryStream(_content).CopyTo(target);
    }

    public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
    {
        await new MemoryStream(_content).CopyToAsync(target, cancellationToken);
    }

    public Stream OpenReadStream()
    {
        return new MemoryStream(_content);
    }
}