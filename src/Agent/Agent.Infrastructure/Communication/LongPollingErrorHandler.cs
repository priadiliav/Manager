using Microsoft.Extensions.Logging;

namespace Agent.Infrastructure.Communication;

public abstract class LongPollingErrorHandler(ILogger<LongPollingErrorHandler> logger)
{    
	public async Task ExecuteAsync(string operationName, Func<Task> action)
	{
		try
		{
			await action();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error occurred in {Operation}", operationName);
		}
	}
	
	public async Task<T> ExecuteAsync<T>(string operationName, Func<Task<T>> action)
	{
		try
		{
			return await action();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error occurred in {Operation}", operationName);
			return default!;
		}
	}
}