using Iot.Device.Ads1115;
using UnitsNet;

namespace Pi.Sensors
{
    /// <summary>
    /// Интерфейс описывающий читатель анлогового датчика.
    /// </summary>
    public interface IAnalogReader
    {
        /// <summary>
        /// Читает значение с аналогового входа.
        /// </summary>
        /// <param name="ads115">
        /// <see cref="Ads1115"/> Для считывания аналогового значения.
        /// </param>
        /// <returns>
        /// Асинхронная операция, результатом которой будет значение, прочитанное с аналогового входа.
        /// </returns>
        ElectricPotential ReadDelayAsync(Ads1115 ads115);
    }
}
