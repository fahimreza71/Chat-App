using ChatApp.API.Data;
using ChatApp.API.Dtos;
using ChatApp.API.Extentions;
using ChatApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace ChatApp.API.Hubs
{
    [Authorize]
    public class ChatHub(UserManager<AppUser> userManager, AppDbContext context) : Hub
    {
        public static readonly ConcurrentDictionary<string, OnlineUserDto> onlineUsers = new();

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var receiverId = httpContext?.Request.Query["senderId"].ToString();
            var userName = Context.User!.Identity!.Name!;
            var currentUser = await userManager.FindByNameAsync(userName);
            var connectionId = Context.ConnectionId;

            if (onlineUsers.ContainsKey(userName))
            {
                onlineUsers[userName].ConnectionId = connectionId;
            }
            else
            {
                var user = new OnlineUserDto
                {
                    ConnectionId = connectionId,
                    UserName = userName,
                    FullName = currentUser!.FullName,
                    ProfilePicture = currentUser!.ProfileImage
                };

                onlineUsers.TryAdd(userName, user);

                await Clients.AllExcept(connectionId).SendAsync("Notify", currentUser);
            }

            await Clients.All.SendAsync("Onlineusers", await GetAllUsers());

        }

        private async Task<IEnumerable<OnlineUserDto>> GetAllUsers()
        {
            var username = Context.User!.GetUserName();
            var onlineUserSet = new HashSet<string>(onlineUsers.Keys);
            var users = await userManager.Users.Select(u => new OnlineUserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                FullName = u.FullName,
                ProfilePicture = u.ProfileImage,
                IsOnline = onlineUserSet.Contains(u.UserName!),
                UnreadCount = context.Messages.Count(x => x.ReceiverId == username && x.SenderId == u.Id && !x.IsRead)
            }).OrderByDescending(u => u.IsOnline).ToListAsync();

            return users;

        }
    }
}
