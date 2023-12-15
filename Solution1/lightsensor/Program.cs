using System;
using System.Threading;
using Microsoft.VisualBasic;
using System.Device.Gpio;

var pinAO = 17;
var pinDO = 27;

using GpioController controller = new GpioController();
controller.OpenPin(pinAO, PinMode.Input);
controller.OpenPin(pinDO, PinMode.Input);

Console.CancelKeyPress += (s, e) => {controller.Dispose(); };

while (true)
{
    Console.WriteLine(controller.Read(pinAO));
    Thread.Sleep(2000);
}