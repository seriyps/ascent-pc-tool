using System.Text;

namespace CaddxTool.Protocol;

// Ported from Caddx_PCTool.ResAckInfoV2. Generic small ack payload (status + a
// short detail string) used by several of the file-transfer commands.
public class ResAckInfoV2
{
    public int Status;
    public byte[] Detail = new byte[64];

    public static ResAckInfoV2 Parse(byte[] frame)
    {
        var info = new ResAckInfoV2
        {
            Status = System.BitConverter.ToInt32(frame, 0),
        };
        System.Array.Copy(frame, 4, info.Detail, 0, 64);
        return info;
    }

    public string DetailText => Encoding.ASCII.GetString(Detail).TrimEnd('\0');
}
