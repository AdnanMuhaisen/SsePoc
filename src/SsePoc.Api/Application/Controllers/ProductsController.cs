using Microsoft.AspNetCore.Mvc;
using SsePoc.Api.Application.Commands;
using SsePoc.Api.Infrastructure.Data;
using System.Net.Mime;
using System.Threading.Channels;

namespace SsePoc.Api.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class ProductsController(ApplicationDbContext dbContext, Channel<Product> channel) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (await dbContext.Products.AnyAsync(p => p.Name.ToLower() == command.Name.ToLower(), cancellationToken))
        {
            return Problem(title: "DUPLICATED_PRODUCT", detail: "Product with the same name already exists", statusCode: StatusCodes.Status400BadRequest);
        }

        var product = new Product
        {
            Name = command.Name,
            UnitPrice = command.UnitPrice,
            Description = command.Description
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        await channel.Writer.WriteAsync(product, cancellationToken);

        return Created();
    }
}