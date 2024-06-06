using System.Device.Gpio;

namespace lighthouse
{
    internal class LightSensorReader
    {
        private readonly GpioController gpioController;
        private TaskCompletionSource<PinEventTypes>? tcs;
        private CancellationToken cancellationToken;

        public LightSensorReader(GpioController gpioController, int pinAO)
        {
            this.gpioController = gpioController;
            this.gpioController.RegisterCallbackForPinValueChangedEvent(pinAO, PinEventTypes.Rising | PinEventTypes.Falling, OnPinValueChanged);
        }

        public Task<PinEventTypes> WaitForValueChanging(CancellationToken cancellationToken)
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
