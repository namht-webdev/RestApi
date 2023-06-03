using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
namespace QandA.Hubs
{
    public class QuestionsHub : Hub
    {
        // when a client connects to this hub, this OnConnectedAsync method will be called
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("Message", "Successfully connected");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Caller.SendAsync("Message", "Successfully disconnected");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SubscribeQuestion(int questionId)
        {
            // TODO - add the client to a group of clients interested in getting updates on the question
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Question-{questionId}");

            // TODO - send a message to the client to indicate that the subscription was successful
            await Clients.Caller.SendAsync("Message", "Successfully subscribed");
        }

        public async Task UnsubscribeQuestion(int questionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Question-{questionId}");
            await Clients.Caller.SendAsync("Message", "Successfully unsubscribed");
        }
    }

}
