namespace FileMan.Business.Features.Av;

public interface IAntiVirusScanner
{
    public Task AssertIsClean(Stream content);
}
