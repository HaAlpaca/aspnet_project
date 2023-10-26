using WebProject.Data.Base;
using WebProject.Models;

namespace WebProject.Data.Services
{
    public class CinemasService: EntityBaseRepository<Cinema>,ICinemasService
    {
        public CinemasService(AppDbContext context) : base(context) { }



    }
}
