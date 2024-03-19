using CommonUtils;
using Iot.Device.Ads1115;
using lighthouse;
using System.Device.Gpio;
using System.Device.I2c;
using System.Timers;
using UnitsNet;

Console.Clear();

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
    Console.WriteLine("Введите чувствительность датчика:");
    Console.WriteLine("1000 = 0.1875 Volt");
    short sensitive = short.Parse(Console.ReadLine()!);

    controller.OpenPin(pinAO, PinMode.Input);
    controller.OpenPin(pinDO, PinMode.Input);
    controller.OpenPin(ledPin, PinMode.Output);
    
    ElectricPotential delta = ads.RawToVoltage(sensitive);
    ElectricPotential previos = new();

    controller.Write(ledPin, PinValue.Low);

    var reader = new LightSensorReader(controller, pinAO);

    

    while (!cancellationToken.IsCancellationRequested)
    {
        try
        {
            var changeType = await reader.WaitForValueChanging(cancellationToken);

            short raw = ads.ReadRaw();
            var voltage = ads.RawToVoltage(raw);
            
            if (voltage > previos)
            {
                if (voltage - previos > delta)
                {
                    controller.Write(ledPin, PinValue.Low);
                    Console.WriteLine("lighting off! Success!");
                    Console.WriteLine($"Voltage: {voltage.Value} {voltage.Unit}");
                    previos = voltage;

                }
            }
            else if (previos > voltage)
            {
                if (previos - voltage > delta)
                {
                    controller.Write(ledPin, PinValue.High);
                    Console.WriteLine("lighting on! Success!");
                    Console.WriteLine($"Voltage: {voltage.Value} {voltage.Unit}");
                    previos = voltage;

                }
            }
            
        }
        catch (OperationCanceledException)
        {
            controller.Write(ledPin, PinValue.Low);
        }
    }
}

