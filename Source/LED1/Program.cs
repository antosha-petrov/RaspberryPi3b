using CommonUtils;
using System.Device.Gpio;

using var lifetimeManager = new ConsoleAppLifetimeManager();
using var cts = new CancellationTokenSource();
using var controller = new GpioController();

var blinkingTask = BlinkSensorAsync(controller, cts.Token);

await lifetimeManager.WaitForShutdownAsync()
    .ConfigureAwait(false);

cts.Cancel();

blinkingTask.Wait();

static async Task BlinkSensorAsync(GpioController controller, CancellationToken cancellationToken)
{
    const int ledPin = 24;
    const int ledOnTime = 1000;
    const int ledOffTime = 1000;

    controller.OpenPin(ledPin, PinMode.Output);

    while (!cancellationToken.IsCancellationRequested)
    {
        try
        {
            // включаем диод
            controller.Write(ledPin, PinValue.High);

            // ждем, диод в это время включен
            await Task.Delay(ledOnTime, cancellationToken);

            // выключаем диод
            controller.Write(ledPin, PinValue.Low);

            // ждем, диод в это время выключен
            await Task.Delay(ledOffTime, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            // выключаем диод
            controller.Write(ledPin, PinValue.Low);
        }
    }
}
