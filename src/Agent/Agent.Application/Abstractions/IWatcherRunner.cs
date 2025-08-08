namespace Agent.Application.Abstractions;

public interface IWatcherRunner
{
	Task StartWatchingAsync(CancellationToken cancellationToken = default);
}
