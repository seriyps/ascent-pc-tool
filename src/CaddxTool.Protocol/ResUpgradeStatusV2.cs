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
}
