using System.Device.Gpio;
using System.Diagnostics.CodeAnalysis;
using Iot.Device.Ads1115;
using UnitsNet;

namespace Pi.Sensors
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated by DI-container.")]
    internal class LightSensor : ILightSensor
    {
        private readonly GpioController gpioController;
        private TaskCompletionSource<PinEventTypes>? tcs;
        private CancellationToken cancellationToken;

        public LightSensor(GpioController gpioController, int pinAO, ILightAnalogSensor? lightAnalogSensor = null, Ads1115? ads1115 = null)
        {
            this.gpioController = gpioController;
            this.gpioController.RegisterCallbackForPinValueChangedEvent(pinAO, PinEventTypes.Rising | PinEventTypes.Falling, OnPinValueChanged);
            LightAnalogSensor = lightAnalogSensor;
            ResultString = string.Empty;
        }

        public ILightAnalogSensor? LightAnalogSensor { get; set; }

        public PinValue PinValue { get; set; }

        public ElectricPotential? ElectricPotential { get; set; }

        public string ResultString { get; set; }

        public async Task<string> ReadValueAsync(CancellationToken cancellationToken)
        {
            var eventType = await GetEventType(cancellationToken).ConfigureAwait(true);

            switch (eventType)
            {
                case PinEventTypes.Rising:
                    PinValue = PinValue.Low;
                    ResultString = "Pin value - Low" + "\n" + "Pin event type - Rising";
                    break;
                case PinEventTypes.Falling:
                    ResultString = "Pin value - High" + "\n" + "Pin event type - Falling";
                    PinValue = PinValue.High;
                    break;
                default:
                    ResultString = "Something went wrong while reading the digital sensor :/";
                    return ResultString;
            }

            try
            {
                ElectricPotential = await LightAnalogSensor!.ReadSensorAsync().ConfigureAwait(false);
                ResultString += "\n" + Convert.ToString(ElectricPotential);
            }
            catch
            {
                ResultString += "\n" + "Something went wrong while reading the analog sensor :/";
            }

            return ResultString;
        }

        private Task<PinEventTypes> GetEventType(CancellationToken cancellationToken)
        {
            tcs = new TaskCompletionSource<PinEventTypes>();
            this.cancellationToken = cancellationToken;
            this.cancellationToken.Register(OnCancelled);
            return tcs.Task;
        }

        private void OnPinValueChanged(object sender, PinValueChangedEventArgs args)
        {
            tcs?.TrySetResult(args.ChangeType);
        }

        private void OnCancelled()
        {
            tcs?.TrySetCanceled();
        }
    }
}
