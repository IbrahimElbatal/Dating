using DatingApp.API.Data.Interfaces;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Data.Implementation
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DatingContext _context;

        public DatingRepository(DatingContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users
                .Include(u => u.Photos)
                .Where(u => u.Id != userParams.UserId);

            if (!string.IsNullOrEmpty(userParams.Gender))
            {
                users = users.Where(u => u.Gender == userParams.Gender);
            }

            if (userParams.Likees)
            {
                users = users.Where(u => GetUserLikes(userParams.UserId, false).Contains(u.Id));
            }

            if (userParams.Likers)
            {
                users = users.Where(u => GetUserLikes(userParams.UserId, true).Contains(u.Id));
            }

            if (userParams.AgeFrom != 18 || userParams.AgeTo != 99)
            {
                var minAge = DateTime.Today.AddYears(-userParams.AgeTo - 1);
                var maxAge = DateTime.Today.AddYears(-userParams.AgeFrom);

                users = users.Where(u => u.DateOfBirth >= minAge && u.DateOfBirth <= maxAge);
            }

            if (!string.IsNullOrEmpty(userParams.Sort))
            {
                switch (userParams.Sort.ToLower())
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageSize, userParams.PageNumber);
        }

        public async Task<User> GetUser(int id)
        {
            return await _context.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Photo> GetMainPhoto(int id)
        {
            return await _context.Photos
                .Where(p => p.UserId == id)
                .FirstOrDefaultAsync(p => p.IsMain == true);
        }

        public async Task<Like> GetUserLike(int id, int recipientId)
        {
            return await _context.Likes
                .Include(l => l.Likee)
                .Include(l => l.Liker)
                .FirstOrDefaultAsync(l => l.LikerId == id && l.LikeeId == recipientId);
        }

        public IEnumerable<int> GetUserLikes(int id, bool likers)
        {
            var user = _context.Users
                .Include(u => u.Photos)
                .Include(u => u.Likers)
                .Include(u => u.Likees)
                .FirstOrDefault(u => u.Id == id);

            if (likers)
            {
                return user.Likers.Where(u => u.LikerId == id).Select(l => l.LikeeId);
            }
            else
            {
                return user.Likees.Where(u => u.LikeeId == id).Select(l => l.LikerId);
            }
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .ThenInclude(um => um.Photos)
                .Include(m => m.Recipient)
                .ThenInclude(um => um.Photos)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams param)
        {
            var messages = _context.Messages
                .Include(m => m.Sender)
                .ThenInclude(sm => sm.Photos)
                .Include(m => m.Recipient)
                .ThenInclude(rm => rm.Photos)
                .AsQueryable();

            switch (param.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(m => m.RecipientId == param.UserId && m.RecipientDelete == false);
                    break;
                case "Outbox":
                    messages = messages.Where(m => m.SenderId == param.UserId && m.SenderDelete == false);
                    break;
                default:
                    messages = messages.Where(m => m.RecipientId == param.UserId && m.RecipientDelete == false && m.IsRead == false);
                    break;
            }

            messages = messages.OrderByDescending(m => m.DateSent);

            return await PagedList<Message>.CreateAsync(messages, param.PageSize, param.PageNumber);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .ThenInclude(um => um.Photos)
                .Include(m => m.Recipient)
                .ThenInclude(um => um.Photos)
                .Where(m => m.SenderId == userId && m.RecipientId == recipientId && m.SenderDelete == false ||
                            m.SenderId == recipientId && m.RecipientId == userId && m.RecipientDelete == false
            ).OrderByDescending(m => m.DateSent).ToListAsync();
        }

    }
}
