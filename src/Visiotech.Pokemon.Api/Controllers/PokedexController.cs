using Microsoft.AspNetCore.Mvc;
using Visiotech.Pokemon.Api.Infrastructure;

namespace Visiotech.Pokemon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PokedexController : ControllerBase
{
    private readonly AppDbContext _context;

    public PokedexController(AppDbContext context)
    {
        _context = context;
    }
}