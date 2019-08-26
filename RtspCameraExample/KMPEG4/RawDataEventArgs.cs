using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtspCameraExample.KMPEG4
{
    public class RawDataEventArgs : EventArgs
    {
        public uint UserParam { get; set; }
        public uint DataType { get; set; }
        public IntPtr Buf { get; set; }
        public uint Len { get; set; }

        public RawDataEventArgs(uint userParam, uint dataType, IntPtr buf, uint len)
        {
            this.UserParam = userParam;
            this.DataType = dataType;
            this.Buf = buf;
            this.Len = len;
        }
    }
}
