using FitnessVibe.Domain.Entities.Social;
using FitnessVibe.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessVibe.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for social feature operations
/// </summary>
public class SocialRepository : ISocialRepository
{
    private readonly FitnessVibeDbContext _context;

    public SocialRepository(FitnessVibeDbContext context)
    {
        _context = context;
    }

    public async Task<UserConnection> CreateConnectionAsync(UserConnection connection)
    {
        _context.UserConnections.Add(connection);
        await _context.SaveChangesAsync();
        return connection;
    }

    public async Task RemoveConnectionAsync(Guid followerId, Guid followedId)
    {
        var connection = await _context.UserConnections
            .FirstOrDefaultAsync(x => x.FollowerId == followerId && x.FollowedId == followedId);

        if (connection != null)
        {
            _context.UserConnections.Remove(connection);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ConnectionExistsAsync(Guid followerId, Guid followedId)
    {
        return await _context.UserConnections
            .AnyAsync(x => x.FollowerId == followerId && x.FollowedId == followedId);
    }

    public async Task<IEnumerable<UserConnection>> GetFollowersAsync(Guid userId)
    {
        return await _context.UserConnections
            .Include(x => x.Follower)
            .Where(x => x.FollowedId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserConnection>> GetFollowingAsync(Guid userId)
    {
        return await _context.UserConnections
            .Include(x => x.Followed)
            .Where(x => x.FollowerId == userId)
            .ToListAsync();
    }

    public async Task<ActivityShare> ShareActivityAsync(ActivityShare share)
    {
        _context.ActivityShares.Add(share);
        await _context.SaveChangesAsync();
        return share;
    }

    public async Task<ActivityShare> UpdateShareAsync(ActivityShare share)
    {
        _context.ActivityShares.Update(share);
        await _context.SaveChangesAsync();
        return share;
    }

    public async Task DeleteShareAsync(Guid shareId)
    {
        var share = await _context.ActivityShares.FindAsync(shareId);
        if (share != null)
        {
            _context.ActivityShares.Remove(share);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<ActivityShare?> GetShareByIdAsync(Guid shareId)
    {
        return await _context.ActivityShares
            .Include(x => x.User)
            .Include(x => x.Activity)
            .Include(x => x.Likes)
            .Include(x => x.Comments)
            .FirstOrDefaultAsync(x => x.Id == shareId);
    }

    public async Task<IEnumerable<ActivityShare>> GetFeedAsync(Guid userId, int skip = 0, int take = 20)
    {
        var following = await _context.UserConnections
            .Where(x => x.FollowerId == userId)
            .Select(x => x.FollowedId)
            .ToListAsync();

        return await _context.ActivityShares
            .Include(x => x.User)
            .Include(x => x.Activity)
            .Include(x => x.Likes)
            .Include(x => x.Comments)
            .Where(x => following.Contains(x.UserId) || x.UserId == userId)
            .OrderByDescending(x => x.SharedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<ActivityShare>> GetUserSharesAsync(Guid userId, int skip = 0, int take = 20)
    {
        return await _context.ActivityShares
            .Include(x => x.User)
            .Include(x => x.Activity)
            .Include(x => x.Likes)
            .Include(x => x.Comments)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.SharedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<ActivityLike> LikeActivityAsync(ActivityLike like)
    {
        _context.ActivityLikes.Add(like);
        await _context.SaveChangesAsync();
        return like;
    }

    public async Task UnlikeActivityAsync(Guid userId, Guid shareId)
    {
        var like = await _context.ActivityLikes
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ActivityShareId == shareId);

        if (like != null)
        {
            _context.ActivityLikes.Remove(like);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ActivityLike>> GetShareLikesAsync(Guid shareId)
    {
        return await _context.ActivityLikes
            .Include(x => x.User)
            .Where(x => x.ActivityShareId == shareId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<ActivityComment> AddCommentAsync(ActivityComment comment)
    {
        _context.ActivityComments.Add(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<ActivityComment> UpdateCommentAsync(ActivityComment comment)
    {
        _context.ActivityComments.Update(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task DeleteCommentAsync(Guid commentId)
    {
        var comment = await _context.ActivityComments.FindAsync(commentId);
        if (comment != null)
        {
            _context.ActivityComments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ActivityComment>> GetShareCommentsAsync(Guid shareId, int skip = 0, int take = 20)
    {
        return await _context.ActivityComments
            .Include(x => x.User)
            .Where(x => x.ActivityShareId == shareId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}
