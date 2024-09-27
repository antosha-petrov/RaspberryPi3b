using Iot.Device.Ads1115;
using UnitsNet;

namespace Pi.Sensors
{
    /// <summary>
    /// Интерфейс описывающй датчик света.
    /// </summary>
    public interface ILightAnalogSensor
    {
        /// <summary>
        /// Свойство, содержащие значение на датчике.
        /// </summary>
        ElectricPotential ElectricPotential { get; set; }

        /// <summary>
        /// Свойство содержит АЦП.
        /// </summary>
        Ads1115 Ads1115 { get; set; }

        /// <summary>
        /// Метод, получающий напряжение на датчике.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/>Возвращает значения напряжения.</returns>
        Task<ElectricPotential> ReadSensorAsync();
    }
}
