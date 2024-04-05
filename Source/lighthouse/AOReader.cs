using Iot.Device.Ads1115;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lighthouse
{
    internal class AOReader
    {
        readonly PeriodicTimer timer;
        private TaskCompletionSource<bool> completed;
        private CancellationToken cancellationToken;
        public AOReader(PeriodicTimer timer)
        {
            this.timer = timer;
        }

        public async Task ReadAOValueAsync(CancellationToken cancellationToken, Ads1115 ads)
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                
            }
        }

        private void OnPinValueChanged(object sender, bool what)
        {
            completed?.TrySetResult(what);
        }

        private void OnCancelled()
        {
            completed?.TrySetCanceled();
        }
    }
}
