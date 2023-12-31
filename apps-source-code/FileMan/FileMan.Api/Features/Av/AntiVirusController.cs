using FileMan.Business.Features.Av;
using Microsoft.AspNetCore.Mvc;

namespace FileMan.Api.Features.Av;

[ApiController]
[Route("[controller]")]
public class AntiVirusController : ControllerBase
{
    private readonly IAntiVirusScanner _scanner;

    public AntiVirusController(IAntiVirusScanner scanner)
    {
        _scanner = scanner;
    }

    [HttpPost]
    [Route("scan")]
    [RequestSizeLimit(100_000_000)]
    public async Task Scan(IFormFile file)
    {
        using var str = file.OpenReadStream();
        await _scanner.AssertIsClean(str);
    }
}