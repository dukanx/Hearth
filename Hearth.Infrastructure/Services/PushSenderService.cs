using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hearth.Infrastructure.Services;

// Prazni PushQueue van HTTP zahteva. Svaka poruka dobija svoj DI scope
// jer WebPushNotifier koristi scoped AppDbContext.
public sealed class PushSenderService : BackgroundService
{
    private readonly PushQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PushSenderService> _logger;

    public PushSenderService(
        PushQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<PushSenderService> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var notifier = scope.ServiceProvider.GetRequiredService<WebPushNotifier>();
                await notifier.SendToUsersAsync(message.UserIds, message.Message, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slanje web push obaveštenja iz reda nije uspelo.");
            }
        }
    }
}
