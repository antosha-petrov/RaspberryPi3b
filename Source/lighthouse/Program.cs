using System.Device.Gpio;
using System.Device.I2c;
using CommonUtils;
using Iot.Device.Ads1115;
using Lighthouse;

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
    var count = 0;
    double waitTime = 2000;

    controller.OpenPin(pinAO, PinMode.Input);
    controller.OpenPin(pinDO, PinMode.Input);
    controller.OpenPin(ledPin, PinMode.Output);

    var reader = new LightSensorReader(controller, pinDO);

    try
    {
        Console.WriteLine("Введите минимальное время между срабатываниями, для защиты от частых срабатываний(milliseconds):");
        waitTime = Convert.ToDouble(Console.ReadLine());
    }
    catch
    {
        Console.WriteLine("установденно значение по умолчанию(2000 milliseconds)");
    }

    using var resultReader = new ResultReader();
    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    CancellationToken token = cancellationTokenSource.Token;

    Reader resultreader = new();

    while (!cancellationToken.IsCancellationRequested)
    {
        try
        {
            var changeType = await reader.WaitForValueChanging(cancellationToken).ConfigureAwait(false);

            resultReader.ReadDelayed(controller, ads, changeType, waitTime, ledPin);

            count++;

            if (count == 2)
            {
                cancellationTokenSource.Cancel();
                CancellationToken token1 = cancellationTokenSource.Token;
                await resultreader.ReadAsync(controller, ads, changeType, ledPin, token1).ConfigureAwait(false);
            }
            else if (count == 1)
            {
                await resultreader.ReadAsync(controller, ads, changeType, ledPin, token).ConfigureAwait(false);
            }
            else
            {
                cancellationTokenSource.Cancel();
                CancellationToken token1 = cancellationTokenSource.Token;
                await resultreader.ReadAsync(controller, ads, changeType, ledPin, token1).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            controller.Write(ledPin, PinValue.Low);
        }
    }
}
