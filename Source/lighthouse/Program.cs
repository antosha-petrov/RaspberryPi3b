using CommonUtils;
using Iot.Device.Ads1115;
using lighthouse;
using System.Device.Gpio;
using System.Device.I2c;
using System.Timers;

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
    bool canContiune = true;
    int sleepingTime = 1250;


    Console.WriteLine("Введите режим работы dayli/short или интервал реагирования в миллесекундах // по умолчанию short");
    string enter = Console.ReadLine();
    if (enter == "dayli")
    {
        sleepingTime = 600000;
    }
    else if (enter == "short")
    {
        sleepingTime = 2000;
    }
    else
    {
        sleepingTime = Convert.ToInt32(enter);
    }

    System.Timers.Timer timer = new System.Timers.Timer(sleepingTime);
    timer.Elapsed += TimerElapsed;

    controller.OpenPin(pinAO, PinMode.Input);
    controller.OpenPin(pinDO, PinMode.Input);
    controller.OpenPin(ledPin, PinMode.Output);

    controller.Write(ledPin, PinValue.Low);

    var reader = new LightSensorReader(controller, pinDO);

    

    while (!cancellationToken.IsCancellationRequested)
    {
        try
        {
            if (canContiune)
            {
                var changeType = await reader.WaitForValueChanging(cancellationToken);

                short raw = ads.ReadRaw();
                var voltage = ads.RawToVoltage(raw);

                if (changeType == PinEventTypes.Rising)
                {
                    controller.Write(ledPin, PinValue.High);
                    Console.WriteLine("lighting on! Success!");
                    canContiune = false;
                }
                else
                {
                    controller.Write(ledPin, PinValue.Low);
                    Console.WriteLine("lighting off! Success!");
                    canContiune = false;
                }

                Console.WriteLine($"Voltage: {voltage.Value} {voltage.Unit}");

                timer.Start();
            }  
        }
        catch (OperationCanceledException)
        {
            controller.Write(ledPin, PinValue.Low);
        }
    }

    void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        canContiune = true;
    }
}

