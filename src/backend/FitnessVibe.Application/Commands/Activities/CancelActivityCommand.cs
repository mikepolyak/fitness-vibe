using System;
using MediatR;

namespace FitnessVibe.Application.Commands.Activities
{
    /// <summary>
    /// Command to cancel an ongoing activity session
    /// </summary>
    public class CancelActivityCommand : IRequest<CancelActivityResponse>
    {
        /// <summary>
        /// The ID of the user cancelling the activity
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The ID of the activity session to cancel
        /// </summary>
        public Guid SessionId { get; set; }

        /// <summary>
        /// Optional reason for cancellation
        /// </summary>
        public string? CancellationReason { get; set; }
    }

    /// <summary>
    /// Response for the cancel activity command
    /// </summary>
    public class CancelActivityResponse
    {
        /// <summary>
        /// When the activity was cancelled
        /// </summary>
        public DateTime CancelledAt { get; set; }

        /// <summary>
        /// Final duration of the activity in minutes
        /// </summary>
        public int FinalDurationMinutes { get; set; }

        /// <summary>
        /// Final calories burned
        /// </summary>
        public decimal FinalCaloriesBurned { get; set; }

        /// <summary>
        /// Any achievements or stats that were recorded before cancellation
        /// </summary>
        public IEnumerable<string> PartialAchievements { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Optional suggestions for the next workout
        /// </summary>
        public IEnumerable<string> NextWorkoutSuggestions { get; set; } = Array.Empty<string>();
    }
}
