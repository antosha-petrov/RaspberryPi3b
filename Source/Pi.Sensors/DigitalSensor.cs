using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi.Sensors
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated by DI-container.")]
    internal class DigitalSensor : IDigitalSensor
    {
        private readonly IDigitalReader digitalReader;

        public DigitalSensor(IDigitalReader digitalReader)
        {
            this.digitalReader = digitalReader ?? throw new ArgumentNullException(nameof(digitalReader));
        }

        public PinValue PinValue { get; set; }

        public async Task<PinValue> ReadValueAsync(CancellationToken cancellationToken)
        {
            var eventType = await digitalReader.ReadValueAsync(cancellationToken).ConfigureAwait(false);

            switch (eventType)
            {
                case PinEventTypes.Rising:
                    PinValue = PinValue.Low;
                    return PinValue;
                case PinEventTypes.Falling:
                    PinValue = PinValue.High;
                    return PinValue;
                default:
                    PinValue = PinValue.Low;
                    return PinValue;
            }
        }
    }
}
