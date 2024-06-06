using System.Diagnostics.CodeAnalysis;
using Iot.Device.Ads1115;
using UnitsNet;

namespace Pi.Sensors
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated by DI-container.")]
    internal class AnalogSensor : IAnalogSensor
    {
        private readonly IAnalogReader analogReader;

        public AnalogSensor(IAnalogReader analogReader)
        {
            this.analogReader = analogReader ?? throw new ArgumentNullException(nameof(analogReader));
        }

        public ElectricPotential ElectricPotential { get; set; }

        public async Task<ElectricPotential> ReadDelayedAsync(Ads1115 ads115)
        {
            ElectricPotential = await analogReader.ReadDelayAsync(ads115).ConfigureAwait(false);
            return ElectricPotential;
        }
    }
}
