using Application.Categories.Commands.CreateCategory;
using Application.Categories.Queries;
using FullCart.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace FullCart.API.Controllers;

public class CategoryController:ApiController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetCategoryByIdResponse>> Get(Guid id)
      => Ok(await Mediator.Send(new GetCategoryByIdQuery(id)));
    
    [HttpGet]
    public async Task<ActionResult<GetCategoryByIdResponse>> Get()
        => Ok(await Mediator.Send(new GetCategoriesListQuery()));

    [HttpPost]
    public async Task<ActionResult<Guid>> Post([FromForm] IFormFile image, CreateCategoryRequest request)
        => Ok(await Mediator.Send(new CreateCategoryCommand(request.Name, image)));
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> Put([FromForm] IFormFile image, UpdateCategoryRequest request)
        => Ok(await Mediator.Send(new CreateCategoryCommand(request.Name, image)));
    
}
