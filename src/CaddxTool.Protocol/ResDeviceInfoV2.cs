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

    // Encoder counterpart to Parse — used by CaddxTool.FakeDevice to emit a
    // FIND_DEVICE response indistinguishable in shape from a real device's.
    public static byte[] Build(int receiveMaxSize, string sdkVersion, string deviceName, int cpuTemp,
        string firmwareInfo, string serialNumber, string hardwareVersion, int status = 0, string detail = "")
    {
        byte[] buf = new byte[300];
        int o = 0;
        System.BitConverter.GetBytes(receiveMaxSize).CopyTo(buf, o); o += 4;
        WriteAscii(buf, o, 32, sdkVersion); o += 32;
        WriteAscii(buf, o, 64, deviceName); o += 64;
        System.BitConverter.GetBytes(cpuTemp).CopyTo(buf, o); o += 4;
        WriteAscii(buf, o, 64, firmwareInfo); o += 64;
        WriteAscii(buf, o, 32, serialNumber); o += 32;
        WriteAscii(buf, o, 32, hardwareVersion); o += 32;
        System.BitConverter.GetBytes(status).CopyTo(buf, o); o += 4;
        WriteAscii(buf, o, 64, detail); o += 64;
        return buf;
    }

    private static void WriteAscii(byte[] buf, int offset, int fieldLength, string value)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(value ?? "");
        int n = System.Math.Min(bytes.Length, fieldLength);
        System.Array.Copy(bytes, 0, buf, offset, n);
    }

    private static string Ascii(byte[] bytes) => Encoding.ASCII.GetString(bytes).TrimEnd('\0');

    public override string ToString()
    {
        return $"DeviceName: {Ascii(DeviceName)} | Fw: {Ascii(FirmwareInfo)} | SN: {Ascii(SerialNumber)} | HW: {Ascii(HardwareVersion)} | CPU Temp: {CpuTemp}C | MaxRecv: {ReceiveMaxSize}kb";
    }
}
