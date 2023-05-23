using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiEventos.DTOs;
using WebApiEventos.Entities;
using WebApiEventos.Services;

namespace WebApiEventos.Controllers
{
   
    [ApiController]
    [Authorize(Policy = "OrganizerPolicy")]
    public class OrganizersController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly OrganizersService service;
        private readonly CommentsService commentsService;

        public OrganizersController(ApplicationDbContext dbContext, OrganizersService service, CommentsService commentsService)
        {
            this.dbContext = dbContext;
            this.service = service;
            this.commentsService = commentsService;
          
        }

        [HttpGet("get")]
        public async Task<IEnumerable<Organizers>> Get()
        {
            return await service.Get();
        }

        [HttpGet("getComments")]
        public async Task<IEnumerable<CommentsDto>> GetComments()
        {
            //Consiguiendo id del usuario
            int organizerId = int.Parse((HttpContext.User.FindFirst("UserId")).Value);
            return await commentsService.GetOrganizerComments(organizerId);
        }



    }
}
