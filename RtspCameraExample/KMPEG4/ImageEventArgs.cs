using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtspCameraExample.KMPEG4
{
    public class ImageEventArgs : EventArgs
    {
        public IntPtr B2Header { get; set; }
        public IntPtr Bmpinfo { get; set; }
        public IntPtr Buffer { get; set; }
        public uint Len { get; set; }
        public uint dwWidth { get; set; }
        public uint dwHeight { get; set; }

        public ImageEventArgs(IntPtr b2, IntPtr bmpinfo, IntPtr buf, uint len, uint dwwidth, uint dwheight)
        {
            B2Header = b2;
            Bmpinfo = bmpinfo;
            Buffer = buf;
            Len = len;
            dwWidth = dwwidth;
            dwHeight = dwheight;
        }
    }

}
