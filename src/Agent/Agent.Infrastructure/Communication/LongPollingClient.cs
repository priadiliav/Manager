using System.Net.Http.Json;
using Agent.Application.Abstractions;

namespace Agent.Infrastructure.Communication;

public class LongPollingClient<TResponse>: ILongPollingClient<TResponse> where TResponse : class
{
	private readonly LongPollingErrorHandler _errorHandler;
	private readonly HttpClient _httpClient;
	private readonly string _pollingUrl;
	
	public LongPollingClient(
		LongPollingErrorHandler errorHandler,
		HttpClient httpClient, 
		string pollingUrl)
	{
		_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		_pollingUrl = pollingUrl ?? throw new ArgumentNullException(nameof(pollingUrl));
		_errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
	}
	
	public async Task StartListeningAsync(Func<TResponse, Task> onUpdate, CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			await _errorHandler.ExecuteAsync(nameof(TResponse), async () =>
			{
				using var response = await _httpClient.GetAsync(_pollingUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
					
					if (content is not null)
						await onUpdate(content);
				}
				else
				{
					await Task.Delay(1000, cancellationToken);
				}
			});
		}
	}
}

