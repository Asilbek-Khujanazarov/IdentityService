using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityService.DTOs;
using IdentityService.Services;
using IdentityService.Enums;

namespace IdentityService.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IIdentityService identityService, ILogger<AdminController> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }

       
    }
}