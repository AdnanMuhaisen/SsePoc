using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Channels;

namespace SsePoc.Api.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController(Channel<Product> channel) : ControllerBase
{
    [HttpGet("manual")]
    public async Task Send(CancellationToken cancellationToken)
    {
        var reader = channel.Reader;
        HttpContext.Response.Headers.Append(HeaderNames.ContentType, MediaTypeNames.Text.EventStream);

        while (!cancellationToken.IsCancellationRequested)
        {
            if (reader is { CanCount: true, Count: > 0 })
            {
                await foreach (var product in reader.ReadAllAsync(cancellationToken))
                {
                    await HttpContext.Response.WriteAsync($"data: {JsonSerializer.Serialize(product)}\n\n", cancellationToken);
                    await HttpContext.Response.Body.FlushAsync(cancellationToken);
                }
            }

            await Task.Delay(500, cancellationToken);
        }
    }
}