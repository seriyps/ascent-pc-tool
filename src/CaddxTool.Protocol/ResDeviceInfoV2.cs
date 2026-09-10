using System.Text;

namespace CaddxTool.Protocol;

// Ported from Caddx_PCTool.ResDeviceInfoV2. Response payload layout for the
// FindDevice/device-info request (cmd 0x3C), verified live: 300-byte payload
// following the 36-byte header (336 bytes total on the wire).
public class ResDeviceInfoV2
{
    public int ReceiveMaxSize;
    public byte[] SdkVersion = new byte[32];
    public byte[] DeviceName = new byte[64];
    public int CpuTemp;
    public byte[] FirmwareInfo = new byte[64];
    public byte[] SerialNumber = new byte[32];
    public byte[] HardwareVersion = new byte[32];
    public int Status;
    public byte[] Detail = new byte[64];

    public static ResDeviceInfoV2 Parse(byte[] frame)
    {
        var info = new ResDeviceInfoV2
        {
            ReceiveMaxSize = System.BitConverter.ToInt32(frame, 0)
        };
        int offset = 4;
        System.Array.Copy(frame, offset, info.SdkVersion, 0, 32);
        offset += 32;
        System.Array.Copy(frame, offset, info.DeviceName, 0, 64);
        offset += 64;
        info.CpuTemp = System.BitConverter.ToInt32(frame, offset);
        offset += 4;
        System.Array.Copy(frame, offset, info.FirmwareInfo, 0, 64);
        offset += 64;
        System.Array.Copy(frame, offset, info.SerialNumber, 0, 32);
        offset += 32;
        System.Array.Copy(frame, offset, info.HardwareVersion, 0, 32);
        offset += 32;
        info.Status = System.BitConverter.ToInt32(frame, offset);
        offset += 4;
        System.Array.Copy(frame, offset, info.Detail, 0, 64);
        return info;
    }

    private static string Ascii(byte[] bytes) => Encoding.ASCII.GetString(bytes).TrimEnd('\0');

    public override string ToString()
    {
        return $"DeviceName: {Ascii(DeviceName)} | Fw: {Ascii(FirmwareInfo)} | SN: {Ascii(SerialNumber)} | HW: {Ascii(HardwareVersion)} | CPU Temp: {CpuTemp}C | MaxRecv: {ReceiveMaxSize}kb";
    }
}
