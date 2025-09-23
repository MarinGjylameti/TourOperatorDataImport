using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;

namespace TourOperatorDataImport.Application.Hubs;

public class ProgressHub(ILogger<ProgressHub> logger) : Hub
{
    public override async Task OnConnectedAsync()
    {
        logger.LogInformation("🚀 Client connected: {ConnectionId}", Context.ConnectionId);
        
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task RegisterForUpload(string uploadId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, uploadId);
        logger.LogInformation("Client {ConnectionId} registered for upload {UploadId}", Context.ConnectionId, uploadId);
    }

    public async Task<string> GetConnectionId()
    {
        return Context.ConnectionId;
    }

}