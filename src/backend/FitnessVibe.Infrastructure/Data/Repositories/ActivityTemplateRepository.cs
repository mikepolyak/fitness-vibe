using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FitnessVibe.Domain.Entities.Activities;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Domain.Enums;

namespace FitnessVibe.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for activity templates
    /// </summary>
    public class ActivityTemplateRepository : IActivityTemplateRepository
    {
        private readonly FitnessVibeDbContext _context;

        public ActivityTemplateRepository(FitnessVibeDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ActivityTemplate?> GetByIdAsync(Guid id)
        {
            return await _context.ActivityTemplates.FindAsync(id);
        }

        public async Task<IEnumerable<ActivityTemplate>> GetAllAsync()
        {
            return await _context.ActivityTemplates.ToListAsync();
        }

        public async Task AddAsync(ActivityTemplate template)
        {
            await _context.ActivityTemplates.AddAsync(template);
        }

        public async Task UpdateAsync(ActivityTemplate template)
        {
            _context.Entry(template).State = EntityState.Modified;
        }

        public async Task DeleteAsync(ActivityTemplate template)
        {
            template.IsDeleted = true;
            await UpdateAsync(template);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ActivityTemplate>> GetFeaturedTemplatesAsync()
        {
            return await _context.ActivityTemplates
                .Where(t => t.IsFeatured)
                .OrderByDescending(t => t.UsageCount)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityTemplate>> GetByCategoryAsync(ActivityCategory category)
        {
            return await _context.ActivityTemplates
                .Where(t => t.Category == category)
                .OrderByDescending(t => t.UsageCount)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityTemplate>> GetByDifficultyRangeAsync(int minDifficulty, int maxDifficulty)
        {
            return await _context.ActivityTemplates
                .Where(t => t.DifficultyLevel >= minDifficulty && t.DifficultyLevel <= maxDifficulty)
                .OrderBy(t => t.DifficultyLevel)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityTemplate>> GetByEquipmentAsync(string equipment)
        {
            return await _context.ActivityTemplates
                .Where(t => EF.Functions.JsonContains(EF.Property<string>(t, "_requiredEquipment"), equipment))
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityTemplate>> GetByTagAsync(string tag)
        {
            return await _context.ActivityTemplates
                .Where(t => EF.Functions.JsonContains(EF.Property<string>(t, "_tags"), tag))
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityTemplate>> GetMostPopularAsync(int count)
        {
            return await _context.ActivityTemplates
                .OrderByDescending(t => t.UsageCount)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityTemplate>> GetByRatingRangeAsync(decimal minRating, decimal maxRating)
        {
            return await _context.ActivityTemplates
                .Where(t => t.AverageRating >= minRating && t.AverageRating <= maxRating)
                .OrderByDescending(t => t.AverageRating)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityTemplate>> SearchAsync(string searchTerm)
        {
            return await _context.ActivityTemplates
                .Where(t => t.Name.Contains(searchTerm) || t.Description.Contains(searchTerm))
                .OrderByDescending(t => t.UsageCount)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityTemplate>> GetByDurationRangeAsync(int minMinutes, int maxMinutes)
        {
            return await _context.ActivityTemplates
                .Where(t => t.EstimatedDurationMinutes >= minMinutes && t.EstimatedDurationMinutes <= maxMinutes)
                .OrderBy(t => t.EstimatedDurationMinutes)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityTemplate>> GetSimilarTemplatesAsync(Guid templateId, int count = 5)
        {
            var template = await GetByIdAsync(templateId);
            if (template == null)
                return Enumerable.Empty<ActivityTemplate>();

            return await _context.ActivityTemplates
                .Where(t => t.Id != templateId &&
                           (t.Category == template.Category ||
                            t.DifficultyLevel == template.DifficultyLevel))
                .OrderByDescending(t => t.UsageCount)
                .Take(count)
                .ToListAsync();
        }
    }
}
