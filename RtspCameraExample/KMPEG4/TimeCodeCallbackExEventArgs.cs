using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtspCameraExample.KMPEG4
{
    public class TimeCodeCallbackExEventArgs : EventArgs
    {
        public uint UserParam { get; private set; }
        public KMPEG4.TimeVal TimeVal { get; private set; }
        public TimeCodeCallbackExEventArgs(uint userParam, KMPEG4.TimeVal time_val)
        {
            UserParam = userParam;
            TimeVal = time_val;
        }
    }
}
