using System;
using System.Threading.Tasks;
using FitnessVibe.Domain.Entities.Activities;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FitnessVibe.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Implementation of the activity repository
    /// </summary>
    public class ActivityRepository : IActivityRepository
    {
        private readonly FitnessVibeDbContext _context;

        public ActivityRepository(FitnessVibeDbContext context)
        {
            _context = context;
        }

        public async Task<Activity?> GetByIdAsync(Guid id)
        {
            return await _context.Activities
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateAsync(Activity activity)
        {
            _context.Entry(activity).State = EntityState.Modified;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
