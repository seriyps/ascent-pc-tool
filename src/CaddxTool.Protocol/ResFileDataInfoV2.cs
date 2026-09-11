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

    // Encoder counterpart to Parse — used by CaddxTool.FakeDevice for the
    // SENDFILE_END ack.
    public static byte[] Build(int length, int cursize, int totalsize, int status, string detail)
    {
        byte[] buf = new byte[16 + 64];
        System.BitConverter.GetBytes(length).CopyTo(buf, 0);
        System.BitConverter.GetBytes(cursize).CopyTo(buf, 4);
        System.BitConverter.GetBytes(totalsize).CopyTo(buf, 8);
        System.BitConverter.GetBytes(status).CopyTo(buf, 12);
        byte[] bytes = Encoding.ASCII.GetBytes(detail ?? "");
        System.Array.Copy(bytes, 0, buf, 16, System.Math.Min(bytes.Length, 64));
        return buf;
    }
}
