using System.Device.Gpio;
using System.Device.I2c;
using Iot.Device.Ads1115;
using UnitsNet;

namespace Pi.Sensors
{
    /// <summary>
    /// Интерфейс описывающй датчик света.
    /// </summary>
    public interface ILightSensor
    {
        /// <summary>
        /// Свойство содержащие цифровой датчик света.
        /// </summary>
        ILightDigitalSensor LightDigitalSensor { get; set; }

        /// <summary>
        /// Свойство содержащие аналоговый датчик света.
        /// </summary>
        ILightAnalogSensor LightAnalogSensor { get; set; }

        /// <summary>
        /// Свойство обозначающие состояние датчика.
        /// </summary>
        PinValue PinValue { get; set; }

        /// <summary>
        /// Свойство содержащие значение на датчике.
        /// </summary>
        ElectricPotential ElectricPotential { get; set; }
    }
}
