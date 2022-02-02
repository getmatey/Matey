using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matey.Common
{
    public interface INotifier
    {
        public Task NotifyAsync<TNotification>(TNotification message) where TNotification : INotification;
    }
}
