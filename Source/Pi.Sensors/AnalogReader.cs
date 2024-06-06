using System.Device.Gpio;
using System.Diagnostics.CodeAnalysis;
using Iot.Device.Ads1115;
using UnitsNet;

namespace Pi.Sensors
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated by DI-container.")]
    internal class AnalogReader : IAnalogReader
    {
        public ElectricPotential ReadDelayAsync(Ads1115 ads115)
        {
            var raw = ads115.ReadRaw();
            var voltage = ads115.RawToVoltage(raw);
            return voltage;
        }
    }
}
