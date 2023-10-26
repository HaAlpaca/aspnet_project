using WebProject.Data.Base;
using WebProject.Models;

namespace WebProject.Data.Services
{
    public class ProducersService: EntityBaseRepository<Producer>,IProducersService
    { 
        public ProducersService(AppDbContext dbContext) : base(dbContext) { 
        }
    }

}
