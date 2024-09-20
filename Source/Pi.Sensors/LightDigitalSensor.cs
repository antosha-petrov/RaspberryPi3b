using System.Device.Gpio;
using System.Diagnostics.CodeAnalysis;

namespace Pi.Sensors
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated by DI-container.")]
    internal class LightDigitalSensor : ILightDigitalSensor
    {
        private readonly GpioController gpioController;
        private TaskCompletionSource<PinEventTypes>? tcs;
        private CancellationToken cancellationToken;

        public LightDigitalSensor(GpioController gpioController, int pinDO)
        {
            this.gpioController = gpioController;
            this.gpioController.RegisterCallbackForPinValueChangedEvent(pinDO, PinEventTypes.Rising | PinEventTypes.Falling, OnPinValueChanged);
        }

        public PinValue PinValue { get; set; }

        public async Task<PinValue> ReadValueAsync(CancellationToken cancellationToken)
        {
            var eventType = await GetEventType(cancellationToken).ConfigureAwait(false);

            switch (eventType)
            {
                case PinEventTypes.Rising:
                    PinValue = PinValue.Low;
                    return PinValue;
                case PinEventTypes.Falling:
                    PinValue = PinValue.High;
                    return PinValue;
                default:
                    PinValue = PinValue.Low;
                    return PinValue;
            }
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
