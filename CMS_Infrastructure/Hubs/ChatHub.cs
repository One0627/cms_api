using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Infrastructure.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        protected static readonly List<string> userList = new List<string>();
        protected static readonly Dictionary<string, string> _connections = new Dictionary<string, string>();
        // 客户端连接时操作
        public override Task OnConnectedAsync()
        {
            var userId = Context.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (!userList.Contains(userId) && !string.IsNullOrWhiteSpace(userId))
            {
                userList.Add(userId);
                _connections.Add(userId, Context.ConnectionId);
            }
            else
            {
                _connections[userId] = Context.ConnectionId;
            }
            if (_connections.Count > 1)
            {
                var ids = _connections.Where(x => x.Key != userId).Select(x => x.Value).ToList();
                Clients.Clients(ids as IReadOnlyList<string>).SendAsync("ReceiveMessage", new { title = "消息提示", message = $"用户{userId}上线了", type = "info" });
            }
            base.OnConnectedAsync();
            return Task.CompletedTask;
        }
        //当客户端断开连接时执行的操作。 如果客户端有意断开连接(通过调用connection.stop()，例如)，则exception参数将为null。 
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = _connections.First(x => x.Value == Context.ConnectionId).Key;
            if (_connections.ContainsValue(Context.ConnectionId))
            {
                _connections.Remove(userId);
            }
            if (_connections.Count > 0)
            {
                Clients.All.SendAsync("ReceiveMessage", new { title = "消息提示", message = $"用户{userId}下线了", type = "info" });
            }
            base.OnDisconnectedAsync(exception);
            return Task.CompletedTask;
        }

        public Task SendMessage(string user, string message)
        {
            if (_connections.ContainsKey(user))
            {
                return Clients.Client(_connections[user]).SendAsync("ReceiveMessage",new { title="消息提示",message=$"用户{_connections[user]}你好！\n--用户{_connections.First(x => x.Value == Context.ConnectionId).Key}",type="error" });
            }
            return Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", new { title = "消息提示", message = $"用户{user}不在线", type = "info" } );
        }
    }
}
