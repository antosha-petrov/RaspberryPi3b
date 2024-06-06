using System.Device.Gpio;

namespace Pi.Sensors
{
    /// <summary>
    /// Представляет читателя данных с цифрового входа.
    /// </summary>
    public interface IDigitalReader
    {
        /// <summary>
        /// Читает значение с цифрового входа.
        /// </summary>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/> для отмены асинхронной операции.
        /// </param>
        /// <returns>
        /// Асинхронная операция, результатом которой будет значение, прочитанное с цифрового входа.
        /// </returns>
        Task<PinEventTypes> ReadValueAsync(CancellationToken cancellationToken);
    }
}
