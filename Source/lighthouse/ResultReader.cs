using System.Device.Gpio;
using System.Diagnostics.CodeAnalysis;
using Iot.Device.Ads1115;

namespace Lighthouse
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
                await Task.Delay(TimeSpan.FromMilliseconds(waitTime), cancellationTokenSource.Token).ConfigureAwait(false);

                var raw = ads.ReadRaw();
                var voltage = ads.RawToVoltage(raw);
                if (pinEventTypes == PinEventTypes.Falling)
                {
                    controller.Write(ledPin, PinValue.Low);
#pragma warning disable CA1303
                    Console.WriteLine("lighting off! Success!");
#pragma warning restore CA1303
                    Console.WriteLine($"Voltage: {voltage.Value} {voltage.Unit}");
                }
                else
                {
                    controller.Write(ledPin, PinValue.High);
#pragma warning disable CA1303
                    Console.WriteLine("lighting on! Success!");
#pragma warning restore CA1303
                    Console.WriteLine($"Voltage: {voltage.Value} {voltage.Unit}");
                }
            }
            catch (OperationCanceledException)
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
