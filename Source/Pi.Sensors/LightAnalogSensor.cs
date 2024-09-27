using System.Diagnostics.CodeAnalysis;
using Iot.Device.Ads1115;
using UnitsNet;

namespace Pi.Sensors
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated by DI-container.")]
    internal class LightAnalogSensor : ILightAnalogSensor
    {
        public LightAnalogSensor(Ads1115 ads1115)
        {
            Ads1115 = ads1115;
        }

        public ElectricPotential ElectricPotential { get; set; }

        public Ads1115 Ads1115 { get; set; }

        public async Task<ElectricPotential> ReadSensorAsync()
        {
            var voltage = await Task.Run(() => Ads1115.ReadVoltage()).ConfigureAwait(false);
            ElectricPotential = voltage;
            return ElectricPotential;
        }
    }
}
