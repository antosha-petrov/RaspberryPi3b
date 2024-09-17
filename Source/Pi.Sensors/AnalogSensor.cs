using System.Diagnostics.CodeAnalysis;
using Iot.Device.Ads1115;
using UnitsNet;

namespace Pi.Sensors
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated by DI-container.")]
    internal class AnalogSensor : IAnalogSensor
    {
        public ElectricPotential ElectricPotential { get; set; }

        public async Task<ElectricPotential> ReadDelayAsync(Ads1115 ads115)
        {
            var raw = await Task.Run(() => ads115.ReadRaw()).ConfigureAwait(false);
            var voltage = ads115.RawToVoltage(raw);
            ElectricPotential = voltage;
            return ElectricPotential;
        }
    }
}
