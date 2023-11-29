namespace FileMan.Business.Features.Pdf;

public interface IPdfConverter
{
    public Task<Stream> FromUrl(string url);
}
