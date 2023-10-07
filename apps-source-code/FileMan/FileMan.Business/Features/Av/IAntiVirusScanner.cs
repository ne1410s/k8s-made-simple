namespace FileMan.Business.Features.Av;

public interface IAntiVirusScanner
{
    public Task<bool> IsContentSafe(Stream content);
}
