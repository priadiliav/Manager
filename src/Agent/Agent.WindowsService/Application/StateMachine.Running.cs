using Agent.WindowsService.Abstraction;
using Agent.WindowsService.Common;
using Agent.WindowsService.Domain;

namespace Agent.WindowsService.Application;
public partial class StateMachine
{
  private async Task HandleRunningEntryAsync()
  {
    _logger.LogInformation("Entering RunningStart state");

    List<Metric> metricCollection = [];
    List<InstructionResult> instrResultsCollection = [];
    IReadOnlyList<Metric> currentCollected = [];

    try
    {
      var storedMetricsBuffer = await _metricStore.GetAllAsync(CancellationToken.None);
      var storedInstrResultsBuffer = await _instrStore.GetAllResultsAsync(CancellationToken.None);

      currentCollected = await _metricCollector.CollectAsync(CancellationToken.None);

      metricCollection.AddRange(storedMetricsBuffer);
      metricCollection.AddRange(currentCollected);
      instrResultsCollection.AddRange(storedInstrResultsBuffer);

      var report = new ReportMessage(
        Metrics: metricCollection.Select(m => m.ToMessage()),
        InstructionResults: instrResultsCollection.Select(ir => ir.ToMessage())
      );

      _logger.LogInformation("Sending report with {MetricCount} metrics and {InstrResultCount} instruction results to server",
        metricCollection.Count, instrResultsCollection.Count);

      await Task.Delay(500, CancellationToken.None);

      await _metricStore.RemoveAllAsync(CancellationToken.None);
      await _instrStore.RemoveAllAsync(CancellationToken.None);

      _logger.LogInformation("RunningStart iteration done");
      await _machine.FireAsync(Triggers.Success);
    }
    catch (HttpRequestException httpEx)
    {
      await _metricStore.StoreAsync(currentCollected, CancellationToken.None);

      _logger.LogError(httpEx, "HTTP Error in RunningStart: {StatusCode} - {Message}", httpEx.StatusCode, httpEx.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in RunningStart");
      await _machine.FireAsync(Triggers.Failed);
    }
  }

  private Task HandleRunningExitAsync()
  {
    _logger.LogInformation("Exiting RunningStart state");
    return Task.CompletedTask;
  }
}
