using Microsoft.AspNetCore.SignalR;

namespace Backend;

public class Hub : Microsoft.AspNetCore.SignalR.Hub
{
    public async void Send(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}