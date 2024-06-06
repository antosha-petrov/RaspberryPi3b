using Iot.Device.Ads1115;
using UnitsNet;

namespace Pi.Sensors
{
    /// <summary>
    /// Интерфейс описывающий аналоговый датчик.
    /// </summary>
    public interface IAnalogSensor
    {
        /// <summary>
        /// Свойство содержащие значение на датчике.
        /// </summary>
        ElectricPotential ElectricPotential { get; set; }

        /// <summary>
        /// Метод для чтения значния на датчике.
        /// </summary>
        /// <param name="ads">
        /// <see cref="Ads1115"/> для чтения значения на себе же.
        /// </param>
        /// <returns>
        /// <see cref="Task"/> представляет асинхронную операцию.
        /// </returns>
        Task<ElectricPotential> ReadDelayedAsync(Ads1115 ads);
    }
}
