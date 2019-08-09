using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RtspCameraExample.NVR
{
    [XmlRoot(ElementName = "Product")]
    public class Product
    {
        [XmlAttribute(AttributeName = "CompanyName")]
        public string CompanyName { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "DualStreamingConfig")]
    public class DualStreamingConfig
    {
        [XmlElement(ElementName = "DefaultLive")]
        public string DefaultLive { get; set; }
        [XmlElement(ElementName = "EnlargedLive")]
        public string EnlargedLive { get; set; }
        [XmlElement(ElementName = "Recording")]
        public string Recording { get; set; }
    }

    [XmlRoot(ElementName = "MotionRange")]
    public class MotionRange
    {
        [XmlAttribute(AttributeName = "min")]
        public string Min { get; set; }
        [XmlAttribute(AttributeName = "max")]
        public string Max { get; set; }
    }

    [XmlRoot(ElementName = "VisualTrackingConfig")]
    public class VisualTrackingConfig
    {
        [XmlElement(ElementName = "Enable")]
        public string Enable { get; set; }
        [XmlElement(ElementName = "ImageSize")]
        public string ImageSize { get; set; }
        [XmlElement(ElementName = "Regions")]
        public string Regions { get; set; }
    }

    [XmlRoot(ElementName = "Config")]
    public class Config
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "FunctionConfig")]
    public class FunctionConfig
    {
        [XmlElement(ElementName = "Config")]
        public Config Config { get; set; }
    }

    [XmlRoot(ElementName = "URI")]
    public class URI
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "LiveStreamURI")]
    public class LiveStreamURI
    {
        [XmlElement(ElementName = "URI")]
        public List<URI> URI { get; set; }
    }

    [XmlRoot(ElementName = "ParamParser")]
    public class ParamParser
    {
        [XmlAttribute(AttributeName = "paramid")]
        public string Paramid { get; set; }
        [XmlAttribute(AttributeName = "prefix")]
        public string Prefix { get; set; }
        [XmlAttribute(AttributeName = "postfix")]
        public string Postfix { get; set; }
    }

    [XmlRoot(ElementName = "StreamPortURI")]
    public class StreamPortURI
    {
        [XmlElement(ElementName = "URI")]
        public string URI { get; set; }
        [XmlElement(ElementName = "ParamParser")]
        public List<ParamParser> ParamParser { get; set; }
    }

    [XmlRoot(ElementName = "StreamURI")]
    public class StreamURI
    {
        [XmlElement(ElementName = "LiveStreamURI")]
        public LiveStreamURI LiveStreamURI { get; set; }
        [XmlElement(ElementName = "StreamPortURI")]
        public StreamPortURI StreamPortURI { get; set; }
        [XmlElement(ElementName = "ControlStream")]
        public string ControlStream { get; set; }
    }

    [XmlRoot(ElementName = "IOConfiguration")]
    public class IOConfiguration
    {
        [XmlElement(ElementName = "Inputs")]
        public string Inputs { get; set; }
        [XmlElement(ElementName = "Outputs")]
        public string Outputs { get; set; }
    }

    [XmlRoot(ElementName = "Naming")]
    public class Naming
    {
        [XmlElement(ElementName = "DI")]
        public string DI { get; set; }
        [XmlElement(ElementName = "DO")]
        public string DO { get; set; }
    }

    [XmlRoot(ElementName = "DIOConfig")]
    public class DIOConfig
    {
        [XmlElement(ElementName = "DI")]
        public string DI { get; set; }
        [XmlElement(ElementName = "DO")]
        public string DO { get; set; }
        [XmlElement(ElementName = "Naming")]
        public Naming Naming { get; set; }
    }

    [XmlRoot(ElementName = "DefaultFishEye")]
    public class DefaultFishEye
    {
        [XmlAttribute(AttributeName = "Mode")]
        public string Mode { get; set; }
        [XmlAttribute(AttributeName = "AutoCenter")]
        public string AutoCenter { get; set; }
        [XmlAttribute(AttributeName = "AbsolutePTZ")]
        public string AbsolutePTZ { get; set; }
        [XmlAttribute(AttributeName = "center")]
        public string Center { get; set; }
    }

    [XmlRoot(ElementName = "Software")]
    public class Software
    {
        [XmlElement(ElementName = "SWMountingType")]
        public string SWMountingType { get; set; }
        [XmlElement(ElementName = "AutoCenter")]
        public string AutoCenter { get; set; }
        [XmlElement(ElementName = "DefaultCenter")]
        public string DefaultCenter { get; set; }
        [XmlElement(ElementName = "AbsolutePTZ")]
        public string AbsolutePTZ { get; set; }
        [XmlElement(ElementName = "FisheyeMode")]
        public string FisheyeMode { get; set; }
    }

    [XmlRoot(ElementName = "Fisheye")]
    public class Fisheye
    {
        [XmlElement(ElementName = "Mode")]
        public string Mode { get; set; }
        [XmlElement(ElementName = "MountingType")]
        public string MountingType { get; set; }
        [XmlElement(ElementName = "InstallationAngle")]
        public string InstallationAngle { get; set; }
        [XmlElement(ElementName = "DefaultCenter")]
        public string DefaultCenter { get; set; }
        [XmlElement(ElementName = "Software")]
        public Software Software { get; set; }
    }

    [XmlRoot(ElementName = "RedundantRecording")]
    public class RedundantRecording
    {
        [XmlElement(ElementName = "Enable")]
        public string Enable { get; set; }
    }

    [XmlRoot(ElementName = "LocalStorage")]
    public class LocalStorage
    {
        [XmlElement(ElementName = "RedundantRecording")]
        public RedundantRecording RedundantRecording { get; set; }
    }

    [XmlRoot(ElementName = "Settings")]
    public class Settings
    {
        [XmlElement(ElementName = "GPSEnable")]
        public string GPSEnable { get; set; }
        [XmlElement(ElementName = "GPSLatitude")]
        public string GPSLatitude { get; set; }
        [XmlElement(ElementName = "GPSLongitude")]
        public string GPSLongitude { get; set; }
        [XmlElement(ElementName = "Angle")]
        public string Angle { get; set; }
        [XmlElement(ElementName = "Direction")]
        public string Direction { get; set; }
    }

    [XmlRoot(ElementName = "IVSEvent")]
    public class IVSEvent
    {
        [XmlAttribute(AttributeName = "server")]
        public string Server { get; set; }
        [XmlAttribute(AttributeName = "camera")]
        public string Camera { get; set; }
    }

    [XmlRoot(ElementName = "Audio")]
    public class Audio
    {
        [XmlElement(ElementName = "AudioOutVolume")]
        public string AudioOutVolume { get; set; }
        [XmlElement(ElementName = "AudioInEnabled")]
        public string AudioInEnabled { get; set; }
        [XmlElement(ElementName = "AudioInSensitivity")]
        public string AudioInSensitivity { get; set; }
        [XmlElement(ElementName = "AudioMicType")]
        public string AudioMicType { get; set; }
        [XmlElement(ElementName = "AudioInLevel")]
        public string AudioInLevel { get; set; }
        [XmlElement(ElementName = "AudioInFormat")]
        public string AudioInFormat { get; set; }
        [XmlElement(ElementName = "AudioInEventState")]
        public string AudioInEventState { get; set; }
        [XmlElement(ElementName = "AudioInEventDwell")]
        public string AudioInEventDwell { get; set; }
        [XmlElement(ElementName = "AudioInEventThreshold")]
        public string AudioInEventThreshold { get; set; }
    }

    [XmlRoot(ElementName = "ChannelNumber")]
    public class ChannelNumber
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Connection")]
    public class Connection
    {
        [XmlElement(ElementName = "HostName")]
        public string HostName { get; set; }
        [XmlElement(ElementName = "UserID")]
        public string UserID { get; set; }
        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }
        [XmlElement(ElementName = "Protocol")]
        public string Protocol { get; set; }
        [XmlElement(ElementName = "ConnectionTimeout")]
        public string ConnectionTimeout { get; set; }
        [XmlElement(ElementName = "ControlPort")]
        public string ControlPort { get; set; }
        [XmlElement(ElementName = "HTTPPort")]
        public string HTTPPort { get; set; }
        [XmlElement(ElementName = "RTSPPort")]
        public string RTSPPort { get; set; }
        [XmlElement(ElementName = "LiveStreamFrom")]
        public string LiveStreamFrom { get; set; }
        [XmlElement(ElementName = "ChannelNumber")]
        public ChannelNumber ChannelNumber { get; set; }
    }

    [XmlRoot(ElementName = "StreamMode")]
    public class StreamMode
    {
        [XmlAttribute(AttributeName = "view")]
        public string View { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Quality")]
    public class Quality
    {
        [XmlElement(ElementName = "EnableFrameRateSetting")]
        public string EnableFrameRateSetting { get; set; }
        [XmlElement(ElementName = "LineFrequency")]
        public string LineFrequency { get; set; }
        [XmlElement(ElementName = "EncoderType1")]
        public string EncoderType1 { get; set; }
        [XmlElement(ElementName = "Resolution1")]
        public string Resolution1 { get; set; }
        [XmlElement(ElementName = "BitrateMode1")]
        public string BitrateMode1 { get; set; }
        [XmlElement(ElementName = "MaxBitrate1")]
        public string MaxBitrate1 { get; set; }
        [XmlElement(ElementName = "VBRMaxBitrate1")]
        public string VBRMaxBitrate1 { get; set; }
        [XmlElement(ElementName = "Bitrate1")]
        public string Bitrate1 { get; set; }
        [XmlElement(ElementName = "FrameRate1")]
        public string FrameRate1 { get; set; }
        [XmlElement(ElementName = "EventFrameRate1")]
        public string EventFrameRate1 { get; set; }
        [XmlElement(ElementName = "RecordFrameRate1")]
        public string RecordFrameRate1 { get; set; }
        [XmlElement(ElementName = "EventRecordFrameRate1")]
        public string EventRecordFrameRate1 { get; set; }
        [XmlElement(ElementName = "Quality1")]
        public string Quality1 { get; set; }
        [XmlElement(ElementName = "H264Quality1")]
        public string H264Quality1 { get; set; }
        [XmlElement(ElementName = "H265Quality1")]
        public string H265Quality1 { get; set; }
        [XmlElement(ElementName = "MPEG4Quality1")]
        public string MPEG4Quality1 { get; set; }
        [XmlElement(ElementName = "EncoderType2")]
        public string EncoderType2 { get; set; }
        [XmlElement(ElementName = "Resolution2")]
        public string Resolution2 { get; set; }
        [XmlElement(ElementName = "BitrateMode2")]
        public string BitrateMode2 { get; set; }
        [XmlElement(ElementName = "MaxBitrate2")]
        public string MaxBitrate2 { get; set; }
        [XmlElement(ElementName = "VBRMaxBitrate2")]
        public string VBRMaxBitrate2 { get; set; }
        [XmlElement(ElementName = "Bitrate2")]
        public string Bitrate2 { get; set; }
        [XmlElement(ElementName = "FrameRate2")]
        public string FrameRate2 { get; set; }
        [XmlElement(ElementName = "EventFrameRate2")]
        public string EventFrameRate2 { get; set; }
        [XmlElement(ElementName = "RecordFrameRate2")]
        public string RecordFrameRate2 { get; set; }
        [XmlElement(ElementName = "EventRecordFrameRate2")]
        public string EventRecordFrameRate2 { get; set; }
        [XmlElement(ElementName = "Quality2")]
        public string Quality2 { get; set; }
        [XmlElement(ElementName = "H264Quality2")]
        public string H264Quality2 { get; set; }
        [XmlElement(ElementName = "H265Quality2")]
        public string H265Quality2 { get; set; }
        [XmlElement(ElementName = "MPEG4Quality2")]
        public string MPEG4Quality2 { get; set; }
    }

    [XmlRoot(ElementName = "VideoQuality")]
    public class VideoQuality
    {
        [XmlElement(ElementName = "Standard")]
        public string Standard { get; set; }
        [XmlElement(ElementName = "StreamMode")]
        public StreamMode StreamMode { get; set; }
        [XmlElement(ElementName = "AspectRatio")]
        public string AspectRatio { get; set; }
        [XmlElement(ElementName = "VideoPosition")]
        public string VideoPosition { get; set; }
        [XmlElement(ElementName = "Quality")]
        public Quality Quality { get; set; }
    }

    [XmlRoot(ElementName = "Motion")]
    public class Motion
    {
        [XmlElement(ElementName = "Bottom1")]
        public string Bottom1 { get; set; }
        [XmlElement(ElementName = "Enable1")]
        public string Enable1 { get; set; }
        [XmlElement(ElementName = "Left1")]
        public string Left1 { get; set; }
        [XmlElement(ElementName = "Right1")]
        public string Right1 { get; set; }
        [XmlElement(ElementName = "Sensitivity1")]
        public string Sensitivity1 { get; set; }
        [XmlElement(ElementName = "Threshold1")]
        public string Threshold1 { get; set; }
        [XmlElement(ElementName = "Timer1")]
        public string Timer1 { get; set; }
        [XmlElement(ElementName = "Top1")]
        public string Top1 { get; set; }
        [XmlElement(ElementName = "Bottom2")]
        public string Bottom2 { get; set; }
        [XmlElement(ElementName = "Enable2")]
        public string Enable2 { get; set; }
        [XmlElement(ElementName = "Left2")]
        public string Left2 { get; set; }
        [XmlElement(ElementName = "Right2")]
        public string Right2 { get; set; }
        [XmlElement(ElementName = "Sensitivity2")]
        public string Sensitivity2 { get; set; }
        [XmlElement(ElementName = "Threshold2")]
        public string Threshold2 { get; set; }
        [XmlElement(ElementName = "Timer2")]
        public string Timer2 { get; set; }
        [XmlElement(ElementName = "Top2")]
        public string Top2 { get; set; }
        [XmlElement(ElementName = "Bottom3")]
        public string Bottom3 { get; set; }
        [XmlElement(ElementName = "Enable3")]
        public string Enable3 { get; set; }
        [XmlElement(ElementName = "Left3")]
        public string Left3 { get; set; }
        [XmlElement(ElementName = "Right3")]
        public string Right3 { get; set; }
        [XmlElement(ElementName = "Sensitivity3")]
        public string Sensitivity3 { get; set; }
        [XmlElement(ElementName = "Threshold3")]
        public string Threshold3 { get; set; }
        [XmlElement(ElementName = "Timer3")]
        public string Timer3 { get; set; }
        [XmlElement(ElementName = "Top3")]
        public string Top3 { get; set; }
        [XmlElement(ElementName = "ImageSize")]
        public string ImageSize { get; set; }
    }

    [XmlRoot(ElementName = "MotionConfig")]
    public class MotionConfig
    {
        [XmlElement(ElementName = "MotionEnable")]
        public string MotionEnable { get; set; }
        [XmlElement(ElementName = "Motion")]
        public Motion Motion { get; set; }
        [XmlElement(ElementName = "PIR")]
        public string PIR { get; set; }
    }

    [XmlRoot(ElementName = "PanSpeedRange")]
    public class PanSpeedRange
    {
        [XmlAttribute(AttributeName = "min")]
        public string Min { get; set; }
        [XmlAttribute(AttributeName = "max")]
        public string Max { get; set; }
    }

    [XmlRoot(ElementName = "TiltSpeedRange")]
    public class TiltSpeedRange
    {
        [XmlAttribute(AttributeName = "min")]
        public string Min { get; set; }
        [XmlAttribute(AttributeName = "max")]
        public string Max { get; set; }
    }

    [XmlRoot(ElementName = "ZoomSpeedRange")]
    public class ZoomSpeedRange
    {
        [XmlAttribute(AttributeName = "min")]
        public string Min { get; set; }
        [XmlAttribute(AttributeName = "max")]
        public string Max { get; set; }
    }

    [XmlRoot(ElementName = "PTZDescribe")]
    public class PTZDescribe
    {
        [XmlElement(ElementName = "PT")]
        public string PT { get; set; }
        [XmlElement(ElementName = "Zoom")]
        public string Zoom { get; set; }
        [XmlElement(ElementName = "Focus")]
        public string Focus { get; set; }
        [XmlElement(ElementName = "Aperture")]
        public string Aperture { get; set; }
        [XmlElement(ElementName = "PTSpeed")]
        public string PTSpeed { get; set; }
        [XmlElement(ElementName = "ZoomSpeed")]
        public string ZoomSpeed { get; set; }
        [XmlElement(ElementName = "Home")]
        public string Home { get; set; }
        [XmlElement(ElementName = "AutoTracking")]
        public string AutoTracking { get; set; }
        [XmlElement(ElementName = "TourScan")]
        public string TourScan { get; set; }
        [XmlElement(ElementName = "LinkedCamera")]
        public string LinkedCamera { get; set; }
        [XmlElement(ElementName = "PanSpeedRange")]
        public PanSpeedRange PanSpeedRange { get; set; }
        [XmlElement(ElementName = "TiltSpeedRange")]
        public TiltSpeedRange TiltSpeedRange { get; set; }
        [XmlElement(ElementName = "ZoomSpeedRange")]
        public ZoomSpeedRange ZoomSpeedRange { get; set; }
    }

    [XmlRoot(ElementName = "DefaultSpeed")]
    public class DefaultSpeed
    {
        [XmlAttribute(AttributeName = "pan")]
        public string Pan { get; set; }
        [XmlAttribute(AttributeName = "tilt")]
        public string Tilt { get; set; }
        [XmlAttribute(AttributeName = "zoom")]
        public string Zoom { get; set; }
    }

    [XmlRoot(ElementName = "PTZ")]
    public class PTZ
    {
        [XmlElement(ElementName = "Enable")]
        public string Enable { get; set; }
        [XmlElement(ElementName = "PTZVendor")]
        public string PTZVendor { get; set; }
        [XmlElement(ElementName = "PTZProtocol")]
        public string PTZProtocol { get; set; }
        [XmlElement(ElementName = "PTZMethod")]
        public string PTZMethod { get; set; }
        [XmlElement(ElementName = "PTZParity")]
        public string PTZParity { get; set; }
        [XmlElement(ElementName = "PTZBaudRate")]
        public string PTZBaudRate { get; set; }
        [XmlElement(ElementName = "PTZAddress")]
        public string PTZAddress { get; set; }
        [XmlElement(ElementName = "ScanPanTiltSpeed")]
        public string ScanPanTiltSpeed { get; set; }
        [XmlElement(ElementName = "DigitalTrackingEnable")]
        public string DigitalTrackingEnable { get; set; }
        [XmlElement(ElementName = "MouseMode")]
        public string MouseMode { get; set; }
        [XmlElement(ElementName = "PTZDescribe")]
        public PTZDescribe PTZDescribe { get; set; }
        [XmlElement(ElementName = "DefaultSpeed")]
        public DefaultSpeed DefaultSpeed { get; set; }
    }

    [XmlRoot(ElementName = "Device")]
    public class Device
    {
        [XmlElement(ElementName = "SerialNumber")]
        public string SerialNumber { get; set; }
        [XmlElement(ElementName = "FirmwareVersion")]
        public string FirmwareVersion { get; set; }
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Product")]
        public Product Product { get; set; }
        [XmlElement(ElementName = "InputValue")]
        public string InputValue { get; set; }
        [XmlElement(ElementName = "OutputValue")]
        public string OutputValue { get; set; }
        [XmlElement(ElementName = "isFisheyeModel")]
        public string IsFisheyeModel { get; set; }
        [XmlElement(ElementName = "MotionEditMode")]
        public string MotionEditMode { get; set; }
        [XmlElement(ElementName = "DualStreamingConfig")]
        public DualStreamingConfig DualStreamingConfig { get; set; }
        [XmlElement(ElementName = "VideoSources")]
        public string VideoSources { get; set; }
        [XmlElement(ElementName = "MotionRange")]
        public MotionRange MotionRange { get; set; }
        [XmlElement(ElementName = "UnitType")]
        public string UnitType { get; set; }
        [XmlElement(ElementName = "hasPIR")]
        public string HasPIR { get; set; }
        [XmlElement(ElementName = "Motions")]
        public string Motions { get; set; }
        [XmlElement(ElementName = "isQuadServer")]
        public string IsQuadServer { get; set; }
        [XmlElement(ElementName = "VisualTrackingConfig")]
        public VisualTrackingConfig VisualTrackingConfig { get; set; }
        [XmlElement(ElementName = "FunctionConfig")]
        public FunctionConfig FunctionConfig { get; set; }
        [XmlElement(ElementName = "StreamURI")]
        public StreamURI StreamURI { get; set; }
        [XmlElement(ElementName = "IOConfiguration")]
        public IOConfiguration IOConfiguration { get; set; }
        [XmlElement(ElementName = "DIOConfig")]
        public DIOConfig DIOConfig { get; set; }
        [XmlElement(ElementName = "EPTZ")]
        public string EPTZ { get; set; }
        [XmlElement(ElementName = "DefaultFishEye")]
        public DefaultFishEye DefaultFishEye { get; set; }
        [XmlElement(ElementName = "Fisheye")]
        public Fisheye Fisheye { get; set; }
        [XmlElement(ElementName = "LocalStorage")]
        public LocalStorage LocalStorage { get; set; }
        [XmlElement(ElementName = "Settings")]
        public Settings Settings { get; set; }
        [XmlElement(ElementName = "IVSEvent")]
        public IVSEvent IVSEvent { get; set; }
        [XmlElement(ElementName = "Audio")]
        public Audio Audio { get; set; }
        [XmlElement(ElementName = "IVSConfig")]
        public string IVSConfig { get; set; }
        [XmlElement(ElementName = "DefaultDisplayMode")]
        public string DefaultDisplayMode { get; set; }
        [XmlElement(ElementName = "AudioOut")]
        public string AudioOut { get; set; }
        [XmlElement(ElementName = "SupportAudioInEvent")]
        public string SupportAudioInEvent { get; set; }
        [XmlElement(ElementName = "SupportVideoStatus")]
        public string SupportVideoStatus { get; set; }
        [XmlElement(ElementName = "hasRFID")]
        public string HasRFID { get; set; }
        [XmlElement(ElementName = "MediaType")]
        public string MediaType { get; set; }
        [XmlElement(ElementName = "Mobile")]
        public string Mobile { get; set; }
        [XmlElement(ElementName = "Connection")]
        public Connection Connection { get; set; }
        [XmlElement(ElementName = "VideoQuality")]
        public VideoQuality VideoQuality { get; set; }
        [XmlElement(ElementName = "MotionConfig")]
        public MotionConfig MotionConfig { get; set; }
        [XmlElement(ElementName = "PTZ")]
        public PTZ PTZ { get; set; }
        [XmlElement(ElementName = "PTZPresets")]
        public string PTZPresets { get; set; }
        [XmlElement(ElementName = "PTZTours")]
        public string PTZTours { get; set; }
        [XmlAttribute(AttributeName = "uid")]
        public string Uid { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "DeviceConfig")]
    public class DeviceConfig
    {
        [XmlElement(ElementName = "Device")]
        public Device Device { get; set; }
    }

}
