using System.Text;

namespace CaddxTool.Protocol;

// Ported from Caddx_PCTool.ResUpgradeStatusV2 — the UPGRADE_STATUS poll ack payload.
public class ResUpgradeStatusV2
{
    public int Percent;
    public int Status;
    public byte[] Detail = new byte[64];

    public static ResUpgradeStatusV2 Parse(byte[] frame)
    {
        var info = new ResUpgradeStatusV2
        {
            Percent = System.BitConverter.ToInt32(frame, 0),
            Status = System.BitConverter.ToInt32(frame, 4),
        };
        System.Array.Copy(frame, 8, info.Detail, 0, 64);
        return info;
    }

    public string DetailText => Encoding.ASCII.GetString(Detail).TrimEnd('\0');

    // Encoder counterpart to Parse — used by CaddxTool.FakeDevice for the
    // UPGRADE_STATUS poll ack.
    public static byte[] Build(int percent, int status, string detail = "")
    {
        byte[] buf = new byte[8 + 64];
        System.BitConverter.GetBytes(percent).CopyTo(buf, 0);
        System.BitConverter.GetBytes(status).CopyTo(buf, 4);
        byte[] bytes = Encoding.ASCII.GetBytes(detail ?? "");
        System.Array.Copy(bytes, 0, buf, 8, System.Math.Min(bytes.Length, 64));
        return buf;
    }
}
