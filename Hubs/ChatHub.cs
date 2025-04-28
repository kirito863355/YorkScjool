using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YorkScjool.Hubs
{
    public class ChatHub : Hub
    {
        public void Send(string senderId, string receiverId, string message)
        {
            Clients.User(senderId).receiveMessage(senderId, message);
            Clients.User(receiverId).receiveMessage(senderId, message);
        }
    }
}