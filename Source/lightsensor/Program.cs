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
    var pinAO = 17;
    var pinDO = 27;

    controller.OpenPin(pinAO, PinMode.Input);
    controller.OpenPin(pinDO, PinMode.Input);

    while (!cancellationToken.IsCancellationRequested)
    {
        try
        {
            controller.RegisterCallbackForPinValueChangedEvent(pinDO, PinEventTypes.Rising | PinEventTypes.Falling, LightSensorTriggered);

            Console.ReadLine();

            static void LightSensorTriggered(object sender, PinValueChangedEventArgs args)
            {
                Console.WriteLine(args.ChangeType);

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





/*using System;
using System.Threading;
using Microsoft.VisualBasic;
using System.Device.Gpio;

var pinAO = 17;
var pinDO = 27;

using GpioController controller = new GpioController();
controller.OpenPin(pinAO, PinMode.Input);
controller.OpenPin(pinDO, PinMode.Input);

controller.RegisterCallbackForPinValueChangedEvent(pinDO, PinEventTypes.Rising | PinEventTypes.Falling, LightSensorTriggered);

Console.ReadLine();

static void LightSensorTriggered(object sender, PinValueChangedEventArgs args)
{
    Console.WriteLine(args.ChangeType);
}*/