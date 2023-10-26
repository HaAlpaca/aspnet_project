using Microsoft.EntityFrameworkCore;
using WebProject.Data.Base;
using WebProject.Models;

namespace WebProject.Data.Services
{
    public class ActorsService : EntityBaseRepository<Actor>, IActorsService
    {

        public ActorsService(AppDbContext context) : base(context) { }

    }
}
