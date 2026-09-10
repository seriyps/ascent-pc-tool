using System.Text;

namespace CaddxTool.Protocol;

// Ported from Caddx_PCTool.ResFileDataInfoV2 — the SENDFILE_END ack payload.
public class ResFileDataInfoV2
{
    public int Length;
    public int Cursize;
    public int Totalsize;
    public int Status;
    public byte[] Detail = new byte[64];

    public static ResFileDataInfoV2 Parse(byte[] frame)
    {
        var info = new ResFileDataInfoV2
        {
            Length = System.BitConverter.ToInt32(frame, 0),
            Cursize = System.BitConverter.ToInt32(frame, 4),
            Totalsize = System.BitConverter.ToInt32(frame, 8),
            Status = System.BitConverter.ToInt32(frame, 12),
        };
        System.Array.Copy(frame, 16, info.Detail, 0, 64);
        return info;
    }

    public string DetailText => Encoding.ASCII.GetString(Detail).TrimEnd('\0');
}
