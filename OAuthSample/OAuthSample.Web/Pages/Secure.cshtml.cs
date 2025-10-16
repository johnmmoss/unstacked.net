using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OAuthSample.Web.Pages;

[Authorize]
public class SecureModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public SecureModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("The secure page was accessed!");
    }
}