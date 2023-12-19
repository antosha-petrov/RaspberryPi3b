using CommonUtils;
using Microsoft.VisualBasic;
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
    var pinAO = 17;
    var pinDO = 27;
    const int ledPin = 24;

    controller.OpenPin(pinAO, PinMode.Input);
    controller.OpenPin(pinDO, PinMode.Input);
    controller.OpenPin(ledPin, PinMode.Output);

    while (!cancellationToken.IsCancellationRequested)
    {
        try
        {
            controller.RegisterCallbackForPinValueChangedEvent(pinDO, PinEventTypes.Rising | PinEventTypes.Falling, LightSensorTriggered);

            Console.ReadLine();

            void LightSensorTriggered(object sender, PinValueChangedEventArgs args)
            {
                if (args.ChangeType == PinEventTypes.Rising)
                {
                    controller.Write(ledPin, PinValue.High);
                    Console.WriteLine("lighting on! Success!");
                }
                else
                {
                    controller.Write(ledPin, PinValue.Low);
                    Console.WriteLine("lighting off! Success!");
                }
                
                Console.WriteLine("for more enter a. Press any key to skip");

                if (Console.ReadLine() == "a")
                {
                    Console.WriteLine("change value");
                    Console.WriteLine(args.ChangeType);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
        }
    }
}