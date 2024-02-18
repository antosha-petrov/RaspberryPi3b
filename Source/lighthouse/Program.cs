/*
https://github.com/dotnet/iot/blob/main/src/devices/Ads1115/README.md
https://learn.microsoft.com/ru-ru/dotnet/api/iot.device.ads1115.ads1115?view=iot-dotnet-latest
*/
// set I2C bus ID: 1
// ADS1115 Addr Pin connect to GND
using CommonUtils;
using Iot.Device.Ads1115;
using lighthouse;
using System.Device.Gpio;
using System.Device.I2c;

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

    controller.OpenPin(pinAO, PinMode.Input);
    controller.OpenPin(pinDO, PinMode.Input);
    controller.OpenPin(ledPin, PinMode.Output);

    var reader = new LightSensorReader(controller, pinDO);

    while (!cancellationToken.IsCancellationRequested)
    {
        try
        {
            var changeType = await reader.WaitForValueChanging(cancellationToken);

            if (changeType == PinEventTypes.Rising)
            {
                controller.Write(ledPin, PinValue.High);
                Console.WriteLine("lighting on! Success!");
            }
            else
            {
                controller.Write(ledPin, PinValue.Low);
                Console.WriteLine("lighting off! Success!");
            }

            // read raw data form the sensor
            short raw = ads.ReadRaw();

            // raw data convert to voltage
            var voltage = ads.RawToVoltage(raw);

            Console.WriteLine($"Voltage: {voltage.Value} {voltage.Unit}");
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            controller.Write(ledPin, PinValue.Low);
        }
    }
}