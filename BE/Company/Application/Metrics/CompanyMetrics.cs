using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Application.Metrics
{
    public sealed class CompanyMetrics : IDisposable
    {
        private readonly Counter<long> _operationCounter;
        private readonly Histogram<double> _operationDuration;
        private readonly ObservableGauge<int> _operationRateLast10Min;

        private readonly ConcurrentDictionary<string, SlidingWindowCounter> _windowCounters = new();
        private readonly Timer _cleanupTimer;
        private readonly TimeSpan _windowSize = TimeSpan.FromMinutes(10);

        public CompanyMetrics(IMeterFactory meterFactory)
        {
            var meter = meterFactory.Create("CompanyApi.Application");
            
            _operationCounter = meter.CreateCounter<long>(
                "company.operation.count",
                description: "Total count of company operations");
                
            _operationDuration = meter.CreateHistogram<double>(
                "company.operation.duration",
                unit: "ms",
                description: "Duration of company operations in milliseconds");

            _operationRateLast10Min = meter.CreateObservableGauge(
                "company.operation.count.last10min",
                () => GetLast10MinutesMetrics(),
                description: "Count of operations in the last 10 minutes by operation and status");

            // Timer de nettoyage toutes les minutes pour supprimer les entrées expirées
            _cleanupTimer = new Timer(CleanupExpiredEntries, null,
                TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        public void RecordCompanyCreated() => 
            RecordOperation("create", "success");

        public void RecordCompanyUpdated() => 
            RecordOperation("update", "success");

        public void RecordCompanyDeleted() => 
            RecordOperation("delete", "success");

        public void RecordValidationError(string operation) => 
            RecordOperation(operation, "validation_error");

        public void RecordOperationError(string operation) => 
            RecordOperation(operation, "error");

        public void RecordOperationDuration(double milliseconds, string operation)
        {
            var tags = new TagList
            {
                { "operation", operation },
            };
            
            _operationDuration.Record(milliseconds, tags);
        }

        private void RecordOperation(string operation, string status)
        {
            var now = DateTime.UtcNow;
            var tags = new TagList
            {
                { "operation", operation },
                { "status", status }
            };
            
            _operationCounter.Add(1, tags);
            var key = $"{operation}:{status}";
            var counter = _windowCounters.GetOrAdd(key, _ => new SlidingWindowCounter());
            counter.Increment(now);
        }

        private IEnumerable<Measurement<int>> GetLast10MinutesMetrics()
        {
            var cutoffTime = DateTime.UtcNow - _windowSize;

            foreach (var kvp in _windowCounters)
            {
                var parts = kvp.Key.Split(':');
                if (parts.Length != 2) continue;

                var count = kvp.Value.GetCount(cutoffTime);

                if (count > 0)
                {
                    yield return new Measurement<int>(
                        count,
                        new KeyValuePair<string, object?>("operation", parts[0]),
                        new KeyValuePair<string, object?>("status", parts[1]));
                }
            }
        }

        private void CleanupExpiredEntries(object? state)
        {
            var cutoffTime = DateTime.UtcNow - _windowSize;

            foreach (var counter in _windowCounters.Values)
            {
                counter.RemovedExpired(cutoffTime);
            }
        }

        public void Dispose()
        {
            _cleanupTimer?.Dispose();
        }

        private class SlidingWindowCounter 
        { 
            private readonly ConcurrentQueue<DateTime> _timestamps = new();
            private int _count;

            public void Increment(DateTime timestamp)
            {
                _timestamps.Enqueue(timestamp);
                Interlocked.Increment(ref _count);
            }

            public int GetCount(DateTime cutofftime)
            {
                RemovedExpired(cutofftime);
                return _count;
            }

            public void RemovedExpired(DateTime cutofftime)
            {
                while(_timestamps.TryPeek(out var timestamp) && timestamp < cutofftime)
                {
                    if (_timestamps.TryDequeue(out _))
                    {
                        Interlocked.Decrement(ref _count);
                    }
                }
            }
        }

    }
}
