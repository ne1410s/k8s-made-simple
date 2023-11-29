using FileMan.Business.Features.Pdf;
using Microsoft.AspNetCore.Mvc;

namespace FileMan.Api.Features.DocConvert;

[ApiController]
[Route("[controller]")]
public class PdfController : ControllerBase
{
    private readonly IPdfConverter _converter;

    public PdfController(IPdfConverter converter)
    {
        _converter = converter;
    }

    [HttpPost]
    [Route("fromUrl")]
    public async Task<FileStreamResult> FromUrl(string url)
    {
        var str = await _converter.FromUrl(url);
        return File(str, "application/pdf", "file.pdf");
    }
}
