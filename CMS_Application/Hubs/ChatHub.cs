using CMS_Application._TableDto;
using CMS_Application.User.UserInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Application.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        protected static  List<ChatUserInfo> chats = new List<ChatUserInfo>();
        // 客户端连接时操作
        public override Task OnConnectedAsync()
        {
            var userId = Context.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var userName = Context.User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;
            var info = new ChatUserInfo();
            info.connetcionId = Context.ConnectionId;
            info.userId = userId;
            info.userName = userName;
            if (!chats.Select(x=>x.userId).Contains(userId) && !string.IsNullOrWhiteSpace(userId))
            {
                chats.Add(info);
            }
            else
            {
                Clients.Client(chats.First(x => x.userId == userId).connetcionId).SendAsync("ChatStop");
                chats.First(x => x.userId == userId).connetcionId = Context.ConnectionId;
            }
            if (chats.Count > 1)
            {
                var ids = chats.Where(x => x.userId != userId).Select(x => x.connetcionId).ToList();
                Clients.Clients(ids as IReadOnlyList<string>).SendAsync("ReceiveNotice", new { title = "消息提示", message = $"用户{chats.First(x => x.connetcionId == Context.ConnectionId).userName}上线了", type = "info" });
            }
            Clients.All.SendAsync("OnlineNum", chats.Count);
            base.OnConnectedAsync();
            return Task.CompletedTask;
        }
        //当客户端断开连接时执行的操作。 如果客户端有意断开连接(通过调用connection.stop()，例如)，则exception参数将为null。 
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var user = chats.FirstOrDefault(x => x.connetcionId == Context.ConnectionId);
            if (chats.Select(x=>x.connetcionId).Contains(Context.ConnectionId))
            {
                chats.Remove(user);
            }
            if (chats.Count > 0&&user!=null)
            {
                Clients.All.SendAsync("ReceiveNotice", new { title = "消息提示", message = $"用户{user.userName}下线了", type = "info" });
            }
            Clients.All.SendAsync("OnlineNum", chats.Count);
            base.OnDisconnectedAsync(exception);
            return Task.CompletedTask;
        }

        public Task SendNotice(string userId, string message)
        {
            if (chats.Select(x => x.userId).Contains(userId))
            {
                return Clients.Client(chats.First(x=>x.userId==userId).connetcionId).SendAsync("ReceiveNotice", new { title = "消息提示", message = $"用户{chats.First(x => x.userId == userId).userName}你好！\n--用户{chats.First(x => x.connetcionId == Context.ConnectionId).userName}", type = "error" });
            }
            return Clients.Client(Context.ConnectionId).SendAsync("ReceiveNotice", new { title = "消息提示", message = $"用户{chats.FirstOrDefault(x => x.userId == userId)?.userName}不在线", type = "info" });
        }
        public Task SendMsg(string msg)
        {
            var userId = Context.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var ids = chats.Select(x => x.userId).ToList();
           return Clients.All.SendAsync("ReceiveMsg", new { type = 1,nickname=chats.First(x => x.connetcionId == Context.ConnectionId).userName ,msg });
        }
    }
}
