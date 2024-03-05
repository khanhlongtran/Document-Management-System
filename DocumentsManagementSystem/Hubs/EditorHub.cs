using Microsoft.AspNetCore.SignalR;

namespace DocumentsManagementSystem.Hubs
{
    public class EditorHub : Hub
    {
        public async Task SendTextUpdate(string newText)
        {
            await Clients.All.SendAsync("ReceiveTextUpdate", newText);
        }
    }
}
