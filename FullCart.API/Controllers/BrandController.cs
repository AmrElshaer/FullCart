using Application.Brands.Commands.CreateBrand;
using Application.Brands.Queries;
using Application.Categories.Queries;
using FullCart.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace FullCart.API.Controllers;

public class BrandController:ApiController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetBrandByIdResponse>> Get(Guid id)
        => Ok(await Mediator.Send(new GetBrandByIdQuery(id)));
    
    [HttpGet]
    public async Task<ActionResult<GetBrandByIdResponse>> Get()
        => Ok(await Mediator.Send(new GetCategoriesListQuery()));

    [HttpPost]
    public async Task<ActionResult<Guid>> Post([FromForm] IFormFile image, CreateBrandRequest request)
        => Ok(await Mediator.Send(new CreateBrandCommand(request.Name, image)));
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> Put([FromForm] IFormFile image, UpdateBrandRequest request)
        => Ok(await Mediator.Send(new CreateBrandCommand(request.Name, image)));
    
}