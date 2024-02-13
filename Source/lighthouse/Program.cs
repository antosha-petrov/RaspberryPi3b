/*
https://github.com/dotnet/iot/blob/main/src/devices/Ads1115/README.md
https://learn.microsoft.com/ru-ru/dotnet/api/iot.device.ads1115.ads1115?view=iot-dotnet-latest
*/
// set I2C bus ID: 1
// ADS1115 Addr Pin connect to GND
using CommonUtils;
using Iot.Device.Ads1115;
using System.Device.Gpio;
using System.Device.I2c;

I2cConnectionSettings settings = new I2cConnectionSettings(1, (int)I2cAddress.GND);
I2cDevice device = I2cDevice.Create(settings);

// pass in I2cDevice
// measure the voltage AIN0
// set the maximum range to 6.144V
using (Ads1115 adc = new Ads1115(device, InputMultiplexer.AIN0, MeasuringRange.FS6144))
{
    // read raw data form the sensor
    short raw = adc.ReadRaw();
    // raw data convert to voltage
    var voltage = adc.RawToVoltage(raw);
}


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