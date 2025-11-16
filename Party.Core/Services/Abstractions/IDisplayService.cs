using Party.Core.Enums;

namespace Party.Core.Services.Abstractions;

public interface IDisplayService
{
    Task DisplayMessageAsync(string message, MessageType messageType = MessageType.Info, CancellationToken cancellationToken = default);

    Task DisplayWarningAsync(string message, CancellationToken cancellationToken = default);

    Task DisplayErrorAsync(string message, CancellationToken cancellationToken = default);

    Task DisplaySuccessAsync(string message, CancellationToken cancellationToken = default);

    Task DisplaSeperatorAsync(CancellationToken cancellationToken = default);
}