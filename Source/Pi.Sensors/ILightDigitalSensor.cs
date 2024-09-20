using System.Device.Gpio;

namespace Pi.Sensors
{
    /// <summary>
    /// Интерфейс описываищий цифровой датчик.
    /// </summary>
    public interface ILightDigitalSensor
    {
        /// <summary>
        /// Свойство обозначающие состояние датчика.
        /// </summary>
        PinValue PinValue { get; set; }

        /// <summary>
        /// метод аксинхронного сяитывания значения.
        /// </summary>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/> для отмены асинхронной операции.
        /// </param>
        /// <returns>
        /// <see cref="Task"/> представляет асинхронную операцию.
        /// </returns>
        Task<PinValue> ReadValueAsync(CancellationToken cancellationToken);
    }
}
