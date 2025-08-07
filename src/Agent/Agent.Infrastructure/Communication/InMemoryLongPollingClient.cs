using System.Net.Http.Json;
using Agent.Application.Abstractions;
using Agent.Domain.Context;
using Microsoft.Extensions.Logging;

namespace Agent.Infrastructure.Communication;

public class InMemoryLongPollingClient<TResponse>: ILongPollingClient<TResponse> where TResponse : class
{
  private readonly ILogger<InMemoryLongPollingClient<TResponse>> _logger;
	private readonly HttpClient _httpClient;
  private readonly AgentStateContext _context;
	private readonly string _pollingUrl;

	public InMemoryLongPollingClient(
    ILogger<InMemoryLongPollingClient<TResponse>> logger,
    HttpClient httpClient,
    AgentStateContext context,
	  string pollingUrl)
  {
    _context = context ?? throw new ArgumentNullException(nameof(context));
		_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		_pollingUrl = pollingUrl ?? throw new ArgumentNullException(nameof(pollingUrl));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task StartListeningAsync(Func<TResponse, Task> handleMessage, CancellationToken cancellationToken)
	{
    _logger.LogInformation("Starting long polling client for URL: {PollingUrl}", _pollingUrl);

		while (!cancellationToken.IsCancellationRequested)
		{
      using var request = new HttpRequestMessage(HttpMethod.Get, _pollingUrl);

      request.Headers.Authorization = new System.Net.Http.Headers
          .AuthenticationHeaderValue("Bearer", _context.AuthenticationToken);

      using var response = await _httpClient.SendAsync(
          request,
          HttpCompletionOption.ResponseHeadersRead,
          cancellationToken);

      if (response.IsSuccessStatusCode)
      {
        _logger.LogInformation("Received response from long polling URL: {PollingUrl}", _pollingUrl);

        var content = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);

        if (content is not null)
          await handleMessage(content);
      }
      else
      {
        _logger.LogError("Failed to receive response from long polling URL: {PollingUrl}, Status Code: {StatusCode}",
            _pollingUrl,
            response.StatusCode);

        await Task.Delay(1000, cancellationToken);
      }
    }
  }
}

