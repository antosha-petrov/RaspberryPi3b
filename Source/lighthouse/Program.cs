using CommonUtils;
using Iot.Device.Ads1115;
using lighthouse;
using System.Device.Gpio;
using System.Device.I2c;
using System.Runtime.InteropServices;
using System.Timers;
using UnitsNet;

var settings = new I2cConnectionSettings(1, (int)I2cAddress.GND);

using var device = I2cDevice.Create(settings);
using var ads = new Ads1115(device, InputMultiplexer.AIN0, MeasuringRange.FS6144);
using var lifetimeManager = new ConsoleAppLifetimeManager();
using var cts = new CancellationTokenSource();
using var controller = new GpioController();

var blinkingTask = BlinkSensorAsync(controller, ads, cts.Token);

await lifetimeManager.WaitForShutdownAsync()
    .ConfigureAwait(false);

cts.Cancel();

blinkingTask.Wait();
static async Task BlinkSensorAsync(GpioController controller, Ads1115 ads, CancellationToken cancellationToken)
{
    var pinAO = 17;
    var pinDO = 27;
    const int ledPin = 24;
    double waitTime = 300;

    controller.OpenPin(pinAO, PinMode.Input);
    controller.OpenPin(pinDO, PinMode.Input);
    controller.OpenPin(ledPin, PinMode.Output);

    CancellationTokenSource cancellationTokenSource = new();
    ResultReader resultReader = new();
    var reader = new LightSensorReader(controller, pinDO);

    try
    {
        await Console.Out.WriteLineAsync("Введите минимальное время между срабатываниями, для защиты от састых срабатываний(milliseconds):");
        waitTime = Convert.ToDouble(Console.ReadLine());
    }
    catch 
    {
        await Console.Out.WriteLineAsync("установденно значение по умолчанию(300 milliseconds)");
    }

    while (!cancellationToken.IsCancellationRequested)
    {
        try
        { 
            var changeType = await reader.WaitForValueChanging(cancellationToken);

            cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();
            await ResultReader.ShakeProtection(controller, ads, changeType, waitTime, ledPin, cancellationTokenSource.Token);

        }
        catch (OperationCanceledException)
        {
            controller.Write(ledPin, PinValue.Low);
        }
    }


}

