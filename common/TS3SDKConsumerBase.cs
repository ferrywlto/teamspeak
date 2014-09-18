using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace teamspeak.definitions
{
    public abstract class TS3SDKConsumerBase
    {
        //Ferry: Below are our own Delegates
        public delegate void CustomMessageEvent(string message);

        public event CustomMessageEvent ErrorOccured;
        public event CustomMessageEvent NotificationNeeded;
        protected bool mapperInitialized = false;
        protected abstract void initMapper();
        protected void notify(string message)
        {
            if (NotificationNeeded != null) NotificationNeeded(message);
        }

        protected void notifyError(string message)
        {
            if (ErrorOccured != null) ErrorOccured(message);
        }

        public TS3SDKConsumerBase()
        {
            initMapper();
            mapperInitialized = true;
        }
    }
}