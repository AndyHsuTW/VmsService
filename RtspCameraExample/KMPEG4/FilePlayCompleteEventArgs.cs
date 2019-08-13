using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtspCameraExample.KMPEG4
{
    public class FilePlayCompleteEventArgs
    {
        public uint UserParam { get; private set; }

        public FilePlayCompleteEventArgs(uint userParam)
        {
            UserParam = userParam;
        }
    }
}
