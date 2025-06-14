using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Domain.Entities.Activities;
using FitnessVibe.Application.Commands.Activities;

namespace FitnessVibe.Application.Handlers.Activities
{
    /// <summary>
    /// Handler for creating new activity templates
    /// </summary>
    public class CreateActivityTemplateCommandHandler : IRequestHandler<CreateActivityTemplateCommand, CreateActivityTemplateResponse>
    {
        private readonly IActivityTemplateRepository _templateRepository;
        private readonly ILogger<CreateActivityTemplateCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the handler
        /// </summary>
        public CreateActivityTemplateCommandHandler(
            IActivityTemplateRepository templateRepository,
            ILogger<CreateActivityTemplateCommandHandler> logger)
        {
            _templateRepository = templateRepository ?? throw new ArgumentNullException(nameof(templateRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the command to create a new activity template
        /// </summary>
        public async Task<CreateActivityTemplateResponse> Handle(CreateActivityTemplateCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new activity template: {TemplateName}", request.Name);

            var template = ActivityTemplate.Create(
                name: request.Name,
                description: request.Description,
                type: request.Type,
                category: request.Category,
                estimatedDurationMinutes: request.EstimatedDurationMinutes,
                estimatedCaloriesBurned: request.EstimatedCaloriesBurned,
                difficultyLevel: request.DifficultyLevel,
                requiredEquipment: request.RequiredEquipment,
                tags: request.Tags,
                iconUrl: request.IconUrl,
                isFeatured: request.IsFeatured);

            await _templateRepository.AddAsync(template);
            await _templateRepository.SaveChangesAsync();

            _logger.LogInformation("Created activity template {TemplateId}: {TemplateName}", 
                template.Id, template.Name);

            return new CreateActivityTemplateResponse
            {
                TemplateId = template.Id,
                Name = template.Name,
                CreatedAt = template.CreatedAt
            };
        }
    }
}
