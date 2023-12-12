using System;
using System.Threading;
using System.Device.Gpio;

var ledPin = 24;
var ledOnTime  = 1000;
var ledOffTime = 1000;

using GpioController controller = new GpioController();
controller.OpenPin(ledPin, PinMode.Output);

Console.CancelKeyPress += (s, e) => {controller.Dispose();};

while (true)
{
    controller.Write(ledPin, PinValue.High);
    Thread.Sleep(ledOnTime);

    controller.Write(ledPin, PinValue.Low);
    Thread.Sleep(ledOffTime);
}