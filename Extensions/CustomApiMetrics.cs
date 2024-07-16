using System.Diagnostics.Metrics;

namespace MinimalApi.Extensions;

public class CustomApiMetrics
{
    public const string Name = "MinimalApi";
    private readonly Counter<long> _apiRequestCounter;
    private readonly Histogram<double> _apiRequestDuration;

    public CustomApiMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(Name);
        _apiRequestCounter = meter.CreateCounter<long>("minimalapi.employee_requests.count");
        _apiRequestDuration = meter.CreateHistogram<double>("minimalapi.employee_requests.duration", "ms");
    }

    public void InceaseEmployeeRequestCount()
    {
        _apiRequestCounter.Add(1);
    }

    public TrackRequestDuration MeasureRequestDuration()
    {
        return new TrackRequestDuration(_apiRequestDuration);
    }
}
public class TrackRequestDuration : IDisposable
{
    private readonly long _requestStartTime = TimeProvider.System.GetTimestamp();
    private readonly Histogram<double> _histogram;

    public TrackRequestDuration(Histogram<double> histogram)
    {
        _histogram = histogram;
    }
    public void Dispose()
    {
        var elapsed = TimeProvider.System.GetElapsedTime(_requestStartTime);
        _histogram.Record(elapsed.TotalMilliseconds);
    }


}


