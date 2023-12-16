using System;
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
}