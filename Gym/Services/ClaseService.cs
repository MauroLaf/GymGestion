using Gym.Models;
using Microsoft.EntityFrameworkCore;
using Gym.Results;

namespace Gym.Services
{
    public class ClaseService
    {
        private AppDbContext _context;
        public ClaseService (AppDbContext context)
        {
            _context = context;
        }
    }
}
