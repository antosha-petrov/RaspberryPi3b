using System.Device.Gpio;
using UnitsNet;

namespace Pi.Sensors
{
    /// <summary>
    /// Интерфейс описываищий датчик.
    /// </summary>
    public interface ILightSensor
    {
        /// <summary>
        /// Свойство обозначающие состояние датчика.
        /// </summary>
        PinValue PinValue { get; set; }

        /// <summary>
        /// Свойство, содержащие значение на датчике.
        /// </summary>
        ElectricPotential? ElectricPotential { get; set; }

        /// <summary>
        /// Свойство содерпжащие анлаговую часть датчика.
        /// </summary>
        ILightAnalogSensor? LightAnalogSensor { get; set; }

        /// <summary>
        /// Строка, содержащяя результаты сичтывания.
        /// </summary>
        string ResultString { get; set; }

        /// <summary>
        /// метод аксинхронного сяитывания значения.
        /// </summary>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/> для отмены асинхронной операции.
        /// </param>
        /// <returns>
        /// <see cref="Task"/> представляет асинхронную операцию.
        /// </returns>
        Task<string> ReadValueAsync(CancellationToken cancellationToken);
    }
}
