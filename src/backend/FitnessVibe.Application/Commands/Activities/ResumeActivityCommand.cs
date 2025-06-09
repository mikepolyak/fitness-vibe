using System;
using MediatR;

namespace FitnessVibe.Application.Commands.Activities
{
    /// <summary>
    /// Command to resume a paused activity session
    /// </summary>
    public class ResumeActivityCommand : IRequest<ResumeActivityResponse>
    {
        /// <summary>
        /// The ID of the user resuming the activity
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The ID of the activity session to resume
        /// </summary>
        public Guid SessionId { get; set; }
    }

    /// <summary>
    /// Response for the resume activity command
    /// </summary>
    public class ResumeActivityResponse
    {
        /// <summary>
        /// When the activity was resumed
        /// </summary>
        public DateTime ResumedAt { get; set; }

        /// <summary>
        /// Total pause duration in minutes
        /// </summary>
        public int PauseDurationMinutes { get; set; }

        /// <summary>
        /// Any motivational messages or tips for resuming
        /// </summary>
        public IEnumerable<string> MotivationalTips { get; set; } = Array.Empty<string>();
    }
}
