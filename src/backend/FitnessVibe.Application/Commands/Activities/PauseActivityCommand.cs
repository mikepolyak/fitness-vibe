using System;
using MediatR;

namespace FitnessVibe.Application.Commands.Activities
{
    /// <summary>
    /// Command to pause an ongoing activity session
    /// </summary>
    public class PauseActivityCommand : IRequest<PauseActivityResponse>
    {
        /// <summary>
        /// The ID of the user pausing the activity
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The ID of the activity session to pause
        /// </summary>
        public Guid SessionId { get; set; }
    }

    /// <summary>
    /// Response for the pause activity command
    /// </summary>
    public class PauseActivityResponse
    {
        /// <summary>
        /// When the activity was paused
        /// </summary>
        public DateTime PausedAt { get; set; }

        /// <summary>
        /// The current duration of the activity in minutes
        /// </summary>
        public int CurrentDurationMinutes { get; set; }

        /// <summary>
        /// Any achievements or milestones reached before pausing
        /// </summary>
        public IEnumerable<string> Achievements { get; set; } = Array.Empty<string>();
    }
}
