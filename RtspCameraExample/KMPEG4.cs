using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ACTi.NVR3.OemDvrMiniDriver
{
    class KMPEG4
    {
        #region DllImport KMPEG4
        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KOpenInterface")]
        public static extern IntPtr KOpenInterface();

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetMediaConfig2")]
        public static extern bool KSetMediaConfig2(IntPtr handle, ref Media_Connect_Config3 MediaConfig);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetMediaConfig3")]
        public static extern bool KSetMediaConfig3(IntPtr handle, ref Media_Connect_Config3 MediaConfig);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetMediaConfig4")]
        public static extern bool KSetMediaConfig4(IntPtr handle, ref Media_Connect_Config4 MediaConfig);

        [DllImport("KMpeg4.dll", EntryPoint = "KConnect", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool KConnect(IntPtr Handel);

        [DllImport("KMpeg4.dll", EntryPoint = "KStartStreaming", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool KStartStreaming(IntPtr Handel);

        [DllImport("KMpeg4.dll", EntryPoint = "KPlay", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KPlay(IntPtr Handel);

        [DllImport("KMpeg4.dll", EntryPoint = "KEnableRender", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KEnableRender(IntPtr Handel, bool EnableRender);

        [DllImport("KMpeg4.dll", EntryPoint = "KEnableDecoder", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KEnableDecoder(IntPtr Handel, bool EnableDecorder);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetRenderInfo")]
        public static extern void KSetRenderInfo(IntPtr Handel, ref MediaRenderInfo renderInfo);

        [DllImport("KMpeg4.dll", EntryPoint = "KStop", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KStop(IntPtr Handel);

        [DllImport("KMpeg4.dll", EntryPoint = "KStopStreaming", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KStopStreaming(IntPtr Handel);

        [DllImport("KMpeg4.dll", EntryPoint = "KDisconnect", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KDisconnect(IntPtr Handel);

        [DllImport("KMpeg4.dll", EntryPoint = "KCloseInterface", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KCloseInterface(IntPtr Handel);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetPortInfoByHTTP")]
        public static extern bool KGetPortInfoByHTTP(IntPtr Handel, ref Media_Port_Info mri, IntPtr httpIp, uint HTTPPort, IntPtr UID, IntPtr PWD, uint ChannelNO);
        //public static extern bool KGetPortInfoByHTTP(IntPtr Handel, ref Media_Port_Info mri, string httpIp, uint HTTPPort, string UID, string PWD, uint ChannelNO);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetAfterRenderCallbackEx")]
        public static extern void KSetAfterRenderCallbackEx(IntPtr Handel, uint userParam, AfterRenderCallbackEx fnAfterRenderCallback);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetNetworkLossCallback")]
        public static extern void KSetNetworkLossCallback(IntPtr Handel, uint userParam, NetworkLossCallback fnNetworkLossCallback);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetResolutionChangeCallback")]
        public static extern void KSetResolutionChangeCallback(IntPtr Handel, uint userParam, ResolutionChangeCallback fnResolutionChangeCallback);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetTimeCodeCallbackEx")]
        public static extern void KSetTimeCodeCallbackEx(IntPtr Handel, uint UserParam, TimeCodeCallbackEx fnTimeCodeCallbackEx);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetMotionDetectionCallback2")]
        public static extern void KSetMotionDetectionCallback2(IntPtr Handel, uint userParam, MotionDetectionCallback2 fnMotionDetectionCallback);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetMotionInfoEx")]
        public static extern void KGetMotionInfoEx(IntPtr Handel, ref MediaMotionInfoEx motionInfoEx);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetMotionInfoEx")]
        public static extern void KSetMotionInfoEx(IntPtr Handel, ref MediaMotionInfoEx motionInfoEx);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KEnablePTZProtocol")]
        public static extern bool KEnablePTZProtocol(IntPtr Handel, bool enable);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KPTZLoadProtocol")]
        [return: MarshalAsAttribute(UnmanagedType.I1)]
        public static extern bool KPTZLoadProtocol(IntPtr Handel, ref MediaPtzProtocol mediaPTZProtocol);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KPTZMove")]
        public static extern bool KPTZMove(IntPtr Handel, int addressID, int speed, PtzMoveOperation ptzMoveOperation);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KDigitalPTZEnable")]
        public static extern void KDigitalPTZEnable(IntPtr Handel, bool enable);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KDigitalPTZTo")]
        public static extern void KDigitalPTZTo(IntPtr Handel, int nXSrc, int nYSrc, int nWidth, int nHeight);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetSubWindowInfo")]
        public static extern void KGetSubWindowInfo(IntPtr Handel, ref SubWindowInfo subwindowinfo);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KEnableSubWindow")]
        public static extern void KEnableSubWindow(IntPtr Handel, bool enable);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KEnableStretchMode")]
        public static extern void KEnableStretchMode(IntPtr Handel, bool enable);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetCurrentTimeUTC")]
        public static extern void KSetCurrentTimeUTC(IntPtr Handel, uint dwTimecode);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetCurrentTime")]
        public static extern void KSetCurrentTime(IntPtr Handel, uint dwTimecode);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KPause")]
        public static extern void KPause(IntPtr Hande);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KShowLastFrame")]
        public static extern bool KShowLastFrame(IntPtr Hande);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetFilePlayCompleteCallback")]
        public static extern bool KSetFilePlayCompleteCallback(IntPtr Hande, uint UserParam, FilePlayCompleteCallback fnFilePlayCompeleteCallback);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetPlayRate")]
        public static extern void KSetPlayRate(IntPtr Hande, int Rate);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KStepNextFrame")]
        public static extern void KStepNextFrame(IntPtr Hande);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KStepPrevFrame")]
        public static extern void KStepPrevFrame(IntPtr Hande);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetFirstB2Callback")]
        public static extern void KSetFirstB2Callback(IntPtr Hande, uint UserParam, FirstB2Callback fnFirstB2Callback);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetRawDataCallback")]
        public static extern void KSetRawDataCallback(IntPtr Hande, uint UserParam, RawDataCallback fnRawDataCallback);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetLastFrame2")]
        public static extern void KGetLastFrame2(IntPtr Hande, ref IntPtr ppBitmapInfo, ref IntPtr ppBitmapData);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetLastFrame3")]
        public static extern void KGetLastFrame3(IntPtr Hande, ref IntPtr ppBitmapInfo, ref IntPtr ppBitmapData, bool isOrginal);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetDICallback2")]
        public static extern void KSetDICallback2(IntPtr Hande, uint UserParam, DICallback2 fnDICallback);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetImageCallback3")]
        public static extern void KSetImageCallback3(IntPtr Hande, uint UserParam, ImageCallback3 fnImageCallback);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetCurrentTimeUTC")]
        public static extern uint KGetCurrentTimeUTC(IntPtr Hande);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetBeginTimeUTC")]
        public static extern void KGetBeginTimeUTC(IntPtr Hande, ref uint dwBeginTime);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetEndTimeUTC")]
        public static extern void KGetEndTimeUTC(IntPtr Hande, ref uint dwEndTime);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetBeginTime")]
        public static extern void KGetBeginTime(IntPtr Hande, ref uint dwBeginTime);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetEndTime")]
        public static extern void KGetEndTime(IntPtr Hande, ref uint dwEndTime);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KPTZEnumerateProtocol")]
        public static extern bool KPTZEnumerateProtocol(IntPtr Hande, IntPtr pVender, IntPtr pProtocol, ref uint dwLen);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KPTZEnumerateVender")]
        public static extern bool KPTZEnumerateVender(IntPtr Hande, IntPtr pVender, ref uint dwLen);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetDownloadLocalFileName")]
        public static extern void KSetDownloadLocalFileName(IntPtr Hande, string fullfilename);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSendDO")]
        public static extern void KSendDO(IntPtr h, byte bDoData);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetAudioToken")]
        [return: MarshalAsAttribute(UnmanagedType.I1)]
        public static extern bool KGetAudioToken(IntPtr Hande, string pholder);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KFreeAudioToken")]
        public static extern void KFreeAudioToken(IntPtr Hande);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSendAudio")]
        [return: MarshalAsAttribute(UnmanagedType.I1)]
        public static extern bool KSendAudio(IntPtr Hande, IntPtr pAudioBuffer, int nLen);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetDICallback3")]
        public static extern void KSetDICallback3(IntPtr Hande, uint userParam, DiCallback3 fnDICallback);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetDIDefaultValue2")]
        public static extern void KSetDIDefaultValue2(IntPtr Hande, UInt16 bDefault);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetDIConfig")]
        public static extern void KSetDIConfig(IntPtr Hande, UInt16 bDefault);

        [DllImport("KMpeg4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetDO")]
        public static extern void KSetDO(IntPtr Hande, int do_num, int value);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetMotionDetectionCallback3")]
        public static extern void KSetMotionDetectionCallback3(IntPtr Handel, uint userParam, MotionDetectionCallback3 fnMotionDetectionCallback);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetVideoLossCallback2")]
        public static extern void KSetVideoLossCallback2(IntPtr Handel, uint userParam, VIDEO_LOSS_CALLBACK2 fnVideoLossCallback);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetVideoRecoveryCallback2")]
        public static extern void KSetVideoRecoveryCallback2(IntPtr Handel, uint userParam, VIDEO_RECOVERY_CALLBACK2 fnVideoRecoveryCallback);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetDownloadProgressCallback")]
        public static extern void KSetDownloadProgressCallback(IntPtr Handel, uint userParam, DownloadProgressCallback Download_Progress_Callback);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetMute")]
        public static extern void KSetMute(IntPtr Handel, [MarshalAsAttribute(UnmanagedType.I1)] bool bMute);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetVolume")]
        public static extern void KSetVolume(IntPtr Handel, int nLeftVolume, int nRightVolume);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetVolume")]
        public static extern void KGetVolume(IntPtr Handel, ref int nLeftVolume, ref int nRightVolume);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetHdcSnapshot")]
        public static extern void KGetHdcSnapshot(IntPtr Hande, ref IntPtr ppBitmapInfo, ref IntPtr ppBitmapData);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KCODECCreate")]
        public static extern IntPtr KCODECCreate();

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KCODECReset")]
        public static extern void KCODECReset(IntPtr h);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KCODECDecode1")]
        public static extern bool KCODECDecode1(IntPtr h, ref IntPtr in_buf, int in_buf_len, ref IntPtr out_buf, ref int out_buf_len);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KCODECDecode2")]
        public static extern bool KCODECDecode2(IntPtr h, byte[] in_buf, int in_buf_len, ref IntPtr bimapinfo, ref IntPtr out_buf, ref int out_buf_len);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KCODECCreate")]
        public static extern void KCODECRelease(IntPtr Hande);

        [DllImport("KMPEG4.DLL", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetDecodeIFrameOnly")]
        public static extern void KSetDecodeIFrameOnly(IntPtr h, [MarshalAsAttribute(UnmanagedType.I1)] bool bDecodeIOnly);


        #region Fisheye

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KEnableFishEye")]
        public static extern void KEnableFishEye(IntPtr Hande, bool enable);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KEnableFishEyeSubWindow")]
        public static extern void KEnableFishEyeSubWindow(IntPtr Hande, bool enable);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetFishEyeMode")]
        public static extern void KSetFishEyeMode(IntPtr Hande, FISHEYE_MODE FisheyeMode);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetFishEyeModule")]
        public static extern bool KSetFishEyeModule(IntPtr Hande, FISHEYE_MODULE FisheyeModule, bool AutoCenter);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetFishEyeRotationAngle")]
        public static extern void KSetFishEyeRotationAngle(IntPtr Hande, int idx, double ViewX, double ViewY);

        //[DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetFishEyeCurrentCoordinate")]
        //public static extern void KGetFishEyeCurrentCoordinate(IntPtr h, ref _tagFishEyeCurrentCoordinate orgcoord);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KFishEyeMoveTo")]
        public static extern void KFishEyeMoveTo(IntPtr h, int idx, int x, int y, int z);       //不使用此來做PTZMove

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KFishEyeRelativeMove")]
        public static extern void KFishEyeRelativeMove(IntPtr h, int idx, int x, int y, int z);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KFishEyeWindowRelativeMove")]
        public static extern void KFishEyeWindowRelativeMove(IntPtr h, int idx, int x, int y);   //使用此來做PTZMove

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KFishEyeSetCircle")]
        public static extern bool KFishEyeSetCircle(IntPtr h, int x, int y, int radius, int width, int height);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KFishEyeGetCircle")]
        public static extern bool KFishEyeGetCircle(IntPtr h, ref int x, ref int y, ref int radius, ref int width, ref int height);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KSetFishEyePTZMoveTo")]
        public static extern void KSetFishEyePTZMoveTo(IntPtr h, int idx, int x, int y, int z);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetFishEyePTZMoveTo")]
        public static extern void KGetFishEyePTZMoveTo(IntPtr h, int idx, ref int x, ref int y, ref int z);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KGetFishEyeRotationAngle")]
        public static extern void KGetFishEyeRotationAngle(IntPtr h, int idx, ref double ViewX, ref double ViewY);

        [DllImport("KMpeg4.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "KFishEyeGetDefaultCircle")]
        public static extern bool KFishEyeGetDefaultCircle(IntPtr h, ref int x, ref int y, ref int radius, ref int width, ref int height);

        #endregion

        #endregion

        #region Struct
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Media_Connect_Config3
        {
            public int ContactType;
            public byte ChannelNumber;
            public byte TCPVideoStreamID;
            public byte RTPVideoTrackNumber;
            public byte RTPAudioTrackNumber;


            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string UniCastIP;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string MultiCastIP;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string PlayFileName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string UserID;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Password;

            public UInt32 RegisterPort;
            public UInt32 StreamingPort;
            public UInt32 ControlPort;
            public UInt32 MultiCastPort;
            public UInt32 SearchPortC2S;
            public UInt32 SearchPortS2C;
            public UInt32 HTTPPort;
            public UInt32 RTSPPort;
            public UInt32 Reserved1;
            public UInt32 Reserved2;
            public UInt16 ConnectTimeOut;
            public UInt16 EncryptionType;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Media_Connect_Config4
        {
            public int ContactType;
            public byte ChannelNumber;
            public byte TCPVideoStreamID;
            public byte RTPVideoTrackNumber;
            public byte RTPAudioTrackNumber;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string UniCastIP;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string MultiCastIP;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string PlayFileName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string UserID;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Password;

            public UInt32 RegisterPort;
            public UInt32 StreamingPort;
            public UInt32 ControlPort;
            public UInt32 MultiCastPort;
            public UInt32 SearchPortC2S;
            public UInt32 SearchPortS2C;
            public UInt32 HTTPPort;
            public UInt32 RTSPPort;
            public UInt32 Reserved1;
            public UInt32 Reserved2;
            public UInt16 ConnectTimeOut;
            public UInt16 EncryptionType;
            public bool IsONVIF;				// RTSP/RTP JPEG streaming by ONVIF format
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct MediaRenderInfo
        {
            public int DrawerInterface;
            public IntPtr hWnd;
            public Rect rect;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct Media_Port_Info   /** Device port info. */
        {
            public uint PORT_HTTP;				///< HTTP Port
            public uint PORT_SearchPortC2S;		///< Search Port 1
            public uint PORT_SearchPortS2C;		///< Search Port 2
            public uint PORT_Register;			///< Register Port
            public uint PORT_Control;				///< Control Port
            public uint PORT_Streaming;			///< Streaming Port
            public uint PORT_Multicast;			///< Multicast Port
            public uint PORT_RTSP;				///< RTSP Port
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct TimeVal
        {
            public int tv_sec;
            public int tv_usec;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct MediaMotionInfoEx
        {
            /// <summary>Flag to enable motion</summary>
            public uint dwEnable;
            /// <summary>Number of Ranger count</summary>
            public uint dwRangeCount;
            /// <summary>Range area(4 can be set)</summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRange;
            /// <summary>Sensitive of range(4 can be set).</summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
            public uint[] dwSensitive;
            /// <summary>dwTime is the motion timer and the range is 0 to 300</summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
            public uint[] dwTime;
            /// <summary>Threshold of the percentage of motion triggered microblocks in the motion region and the range is 0 to 100.</summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
            public uint[] dwThreshold;
            /// <summary>State of the motion region. 0: disable, 1: enable.</summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
            public uint[] bEnable;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MediaPtzProtocol
        {
            /// <summary>Specify the source type is inside resource or a PTZ protocol file</summary>
            public int nSourceType;
            /// <summary>The vender name.</summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szVender;
            /// <summary>The protocol name.</summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szProtocol;
            /// <summary>The PTZ protocol file name.</summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 512)]
            public string szProtocolFileName;
            /// <summary>Address ID</summary>
            public int dwAddressID;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct SubWindowInfo
        {
            public int iX;
            public int iY;
            public int iWidth;
            public int iHeight;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct B2Frame
        {
            public B2_HEADER Head;
            public PRIVATE_DATA Data;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct B2_HEADER
        {
            public byte key1;
            public byte key2;
            public byte key3;
            public byte key4;

            public byte type;
            public byte Stream_id;
            public byte Ext_b2_len;
            public byte Rsvd;
            public uint Len;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct PRIVATE_DATA
        {
            public uint time;
            public byte TimeZone;
            public byte VideoLoss;
            public byte Motion;

            public byte DIO;
            public int Cnt;
            public byte Resolution;
            public byte BitRate;
            public byte FpsMode;
            public byte FpsNum;

            public int tv_sec;
            public int tv_usec;

            public ushort MDActives1;
            public ushort MDActives2;
            public ushort MDActives3;

            public byte DaylightBias;

            //public byte test;
            public byte isReset;
            //public byte isPre;
            //public byte PreCounts;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct _tagFishEyeCurrentCoordinate
        {
            int Pan;
            int Tilt;
            int Zoom;

            int point_counts;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 128)]
            Point[] pts;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct tagBITMAPINFO
        {
            public tagBITMAPINFOHEADER bmiHeader;

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 1)]
            public tagRGBQUAD[] bmiColors;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct tagBITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public UInt16 biPlanes;
            public UInt16 biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct tagRGBQUAD
        {
            byte rgbBlue;
            byte rgbGreen;
            byte rgbRed;
            byte rgbReserved;
        }


        #endregion

        public enum CONTACT_TYPE
        {
            CONTACT_TYPE_UNUSE = 0,
            CONTACT_TYPE_UNICAST_WOC_PREVIEW = 1,
            CONTACT_TYPE_MULTICAST_WOC_PREVIEW,
            CONTACT_TYPE_RTSP_PREVIEW,
            CONTACT_TYPE_CONTROL_ONLY,
            CONTACT_TYPE_UNICAST_PREVIEW,
            CONTACT_TYPE_MULTICAST_PREVIEW,
            CONTACT_TYPE_PLAYBACK,
            CONTACT_TYPE_CARD_PREVIEW,

            CONTACT_TYPE_MULTIPLE_PLAYBACK,
            CONTACT_TYPE_HTTP_RTSP_WOC_PREVIEW,
            CONTACT_TYPE_HTTP_RTSP_PREVIEW,

            CONTACT_TYPE_HTTP,

            CONTACT_TYPE_RTSP_RTPUDP_PREVIEW,
            CONTACT_TYPE_RTSP_RTPUDP_WOC_PREVIEW,
            CONTACT_TYPE_RTSP_RTPTCP_PREVIEW,
            CONTACT_TYPE_RTSP_RTPTCP_WOC_PREVIEW,

            CONTACT_TYPE_HTTP_WOC_PREVIEW,
            CONTACT_TYPE_HTTP_PREVIEW,
            CONTACT_TYPE_HTTP_CONTROL_ONLY,
            CONTACT_TYPE_HTTP_MESSAGE, // dual session ; listen and POST Message


            CONTACT_TYPE_HTTP_REMOTE_PLAYBACK,         // Remote Playback (search/play)
            //CONTACT_TYPE_HTTP_AUDIO_TRANSFER,          // Send Audio to Device

            CONTACT_TYPE_PLAYBACK_AVI = 60,

            CONTACT_TYPE_MAX,
        }

        public enum DRAWER_TYPES			/** Drawer interface types */
        {
            DGDI,					///< Request to use Windows GDI for draw
            DXDRAW					///< Request to use Direct Draw for draw
        }

        public enum PtzMoveOperation
        {
            PtzMoveUp,
            PtzMoveDown,
            PtzMoveLeft,
            PtzMoveRight,
            PtzMoveUpLeft,
            PtzMoveUpRight,
            PtzMoveDownLeft,
            PtzMoveDownRight,
            PtzMoveStop,
        }

        public enum PLAY_RATES /** Play rate */
        {
            RATE_0_5, ///< #0# - 1/2 Speed
            RATE_1_0, ///< #1# - 1.0 Speed
            RATE_2_0, ///< #2# - 2.0 Speed
            RATE_4_0, ///< #3# - 4.0 Speed
            RATE_8_0  ///< #4# - 8.0 Speed
        };

        public enum FISHEYE_MODULE
        {
            enACTi_KCM3911 = 0,//Support KCM3911
            enACTi_KCM7911 = 1,//Support KCM7911
            enACTi_B54 = 2,//Support B54
            enACTi_B55 = 3,//Support B55
            enACTi_B56 = 4,//Support B56
            enACTi_E96 = 5,//Support E96
            enACTi_E919 = 6,//Support E919
            enACTi_E921 = 7,//Support E921
            enACTi_E923 = 8,//Support E923	
            enACTi_I51 = 9,//Support I51
            enACTi_I71 = 10,//Support I71
            enACTi_E98 = 11,//Support E98
            enACTi_E15 = 12,//Support E15
            enACTi_E16 = 13,//Support E16
            enACTi_E925 = 14,//Support E925
            enACTi_E927 = 15,//Support E927
            enACTi_E929 = 16,//Support E929
            enACTi_I73 = 17,//Support I73
            enACTi_Q111 = 18,//Support Q111
            enACTi_Q13 = 19,//Support Q13

            enACTi_Auto = 999,//Support all camera
        };

        public enum FISHEYE_MODE
        {
            enWall_Dewarping = 0,
            enWall_Panorama = 1,
            enCeiling_Dewarping = 2,
            enCeiling_Panorama = 3,
            enCeiling_DoublePanorama = 4,
            enGround_Dewarping = 5,
            enGround_Panorama = 6,
            enGround_DoublePanorama = 7,

            enWall_DoublePanorama = 20,
            enWall_PanoramaFocus = 21,
            enWall_Surround = 22,
            enCeiling_PanoramaFocus = 23,
            enCeiling_Surround = 24,
            enGround_PanoramaFocus = 25,
            enGround_Surround = 26,
        };

        public enum DO_STATUS
        {
            CLEAN_DO = 0x00,
            DO1 = 0x01,
            DO2 = 0x02,
            DO_BOTH = 0x03
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate void AfterRenderCallbackEx(uint userParam, IntPtr hdc);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate void NetworkLossCallback(uint userParam);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate void ResolutionChangeCallback(uint userParam, int nResolution);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate void TimeCodeCallbackEx(uint userParam, TimeVal timeCodeIncludeMicroseconds);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate void MotionDetectionCallback2(uint userParam, byte motion, byte pir);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate void MotionDetectionCallback3(uint userParam, uint motion, byte pir);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate void FilePlayCompleteCallback(uint userParam);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate void FirstB2Callback(uint userParam, IntPtr buf, uint len);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RawDataCallback(uint UserParam, uint dwdataType, IntPtr buf, uint len);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void DICallback2(uint UserParam, byte value);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate void DiCallback3(uint userParam, uint di);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ImageCallback3(uint UserParam, IntPtr b2, IntPtr bmpinfo, IntPtr buf, uint len, uint dwWidth, uint dwHeight);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate void VIDEO_LOSS_CALLBACK2(uint userParam, byte VideoLossFlag);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate void VIDEO_RECOVERY_CALLBACK2(uint userParam, byte VideoRecoveryFlag);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
        public delegate void DownloadProgressCallback(uint userParam, Int64 filesize, Int64 downloadsize, Int64 downloadtime);

    }
}
