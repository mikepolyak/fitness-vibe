using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using FitnessVibe.Domain.Enums;

namespace FitnessVibe.Application.Commands.Challenges
{
    /// <summary>
    /// Command to create a new challenge
    /// </summary>
    public class CreateChallengeCommand : IRequest<CreateChallengeResponse>
    {
        /// <summary>
        /// Title of the challenge
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Description of the challenge
        /// </summary>
        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// When the challenge starts
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// When the challenge ends (optional)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Type of challenge
        /// </summary>
        [Required]
        public ChallengeType Type { get; set; }

        /// <summary>
        /// Target value to achieve
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal TargetValue { get; set; }

        /// <summary>
        /// Unit of measurement
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Type of activity (optional)
        /// </summary>
        public ActivityType? ActivityType { get; set; }

        /// <summary>
        /// Whether the challenge is private
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// ID of user creating the challenge
        /// </summary>
        public Guid CreatedById { get; set; }
    }

    /// <summary>
    /// Response from creating a new challenge
    /// </summary>
    public class CreateChallengeResponse
    {
        /// <summary>
        /// ID of the created challenge
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the challenge
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// When the challenge starts
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// When the challenge was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
