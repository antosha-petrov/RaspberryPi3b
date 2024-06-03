using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi.Sensors
{
    public interface IDigitalSensor
    {
        Task<PinValue> ReadValueAsync(CancellationToken cancellationToken);
    }
}
