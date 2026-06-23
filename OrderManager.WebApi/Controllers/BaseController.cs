using Microsoft.AspNetCore.Mvc;
using OrderManager.WebApi.Interfaces;

namespace OrderManager.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class BaseController : ControllerBase
{
    private ICurrentUserService? _currentUserService;
    protected ICurrentUserService CurrentUserService =>
        _currentUserService ??= HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

    protected int CurrentUserId => CurrentUserService.UserId;
}