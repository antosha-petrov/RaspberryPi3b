using System.Diagnostics.CodeAnalysis;

namespace lighthouse
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated by DI-container.")]
    internal class ResultReader : IDisposable
    {
        private CancellationTokenSource? cancellationTokenSource;
        private bool disposed;

        public void ReadDelayed(GpioController controller, Ads1115 ads, PinEventTypes pinEventTypes, double waitTime, int ledPin)
        {
            Cancel();
            
            _ = ReadDelayedAsync(controller, ads, pinEventTypes, waitTime, ledPin);
        }

        public async Task ReadDelayedAsync(GpioController controller, Ads1115 ads, PinEventTypes pinEventTypes, double waitTime, int ledPin)
        {
            InvalidateCancellationTokenSource();

            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(waitTime), cancellationTokenSource.Token);

                var raw = ads.ReadRaw();
                var voltage = ads.RawToVoltage(raw);
                if (pinEventTypes == PinEventTypes.Falling)
                {
                    controller.Write(ledPin, PinValue.Low);
                    Console.WriteLine("lighting off! Success!");
                    Console.WriteLine($"Voltage: {voltage.Value} {voltage.Unit}");
                }
                else
                {
                    controller.Write(ledPin, PinValue.High);
                    Console.WriteLine("lighting on! Success!");
                    Console.WriteLine($"Voltage: {voltage.Value} {voltage.Unit}");
                }
            }
            catch(OperationCanceledException) 
            {
                return;
            }
        }

        public void Cancel() => cancellationTokenSource?.Cancel();

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            InvalidateCancellationTokenSource();
            disposed = true;
        }

        private void InvalidateCancellationTokenSource()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }
        }
    }
}
