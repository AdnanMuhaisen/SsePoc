using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;

namespace SsePoc.Api.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController(Channel<Product> channel) : ControllerBase
{
    [HttpGet("manual")]
    public async Task SendManual(CancellationToken cancellationToken)
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

    [HttpGet]
    public ServerSentEventsResult<Product> Send(CancellationToken cancellationToken)
    {
        return TypedResults.ServerSentEvents((GetProductsAsync(cancellationToken)), eventType: "product");
    }

    private async IAsyncEnumerable<Product> GetProductsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var reader = channel.Reader;
        while (!cancellationToken.IsCancellationRequested)
        {
            if (reader is { CanCount: true, Count: > 0 })
            {
                while (reader.Count > 0)
                {
                    yield return await reader.ReadAsync(cancellationToken);
                }
            }

            await Task.Delay(500, cancellationToken);
        }
    }
}