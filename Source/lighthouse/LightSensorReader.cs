using System.Device.Gpio;

namespace Lighthouse
{
    /// <summary>
    /// Читает что-нибудь.
    /// </summary>
    internal class LightSensorReader
    {
        private readonly GpioController gpioController;
        private TaskCompletionSource<PinEventTypes>? tcs;
        private CancellationToken cancellationToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="LightSensorReader"/> class.
        /// </summary>
        /// <param name="gpioController"> gpio-контролер для управления портами.</param>
        /// <param name="pinAO">номер пина gpio с анлоговым входом.</param>
        public LightSensorReader(GpioController gpioController, int pinAO)
        {
            this.gpioController = gpioController;
            this.gpioController.RegisterCallbackForPinValueChangedEvent(pinAO, PinEventTypes.Rising | PinEventTypes.Falling, OnPinValueChanged);
        }

        /// <summary>
        /// Метод ожидания смены значения.
        /// </summary>
        /// <param name="cancellationToken">токен для отмены операции.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
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
