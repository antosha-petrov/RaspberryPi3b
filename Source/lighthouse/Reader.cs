﻿using Iot.Device.Ads1115;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lighthouse
{
    internal class Reader
    {
        public async Task ReadAsync(GpioController controller, Ads1115 ads, PinEventTypes changeType, int ledPin, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(350), cancellationToken);

                short raw = ads.ReadRaw();
                var voltage = ads.RawToVoltage(raw);

                if (changeType == PinEventTypes.Falling)
                {  
                    controller.Write(ledPin, PinValue.High);
                    Console.WriteLine("lighting on! Success!");
                    Console.WriteLine($"Voltage: {voltage.Value} {voltage.Unit}");
                }
                else
                {
                    controller.Write(ledPin, PinValue.Low);
                    Console.WriteLine("lighting off! Success!");
                    Console.WriteLine($"Voltage: {voltage.Value} {voltage.Unit}");
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }
}