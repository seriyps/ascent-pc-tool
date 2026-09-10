using System.Text;

namespace CaddxTool.Protocol;

// Ported from Caddx_PCTool.ArFileInfo + FileTransferSession.BuildFileInfo.
// 328-byte SENDFILE_START payload: MD5[64] + length:int32 + saveAsFile:int32
// + filePath[128] + fileDir[128].
public static class ArFileInfoCodec
{
    public const int MD5_FIELD_LENGTH = 64;
    public const int PATH_FIELD_LENGTH = 128;
    public const int TOTAL_LENGTH = MD5_FIELD_LENGTH + 4 + 4 + PATH_FIELD_LENGTH + PATH_FIELD_LENGTH;

    // md5Hex: lowercase hex string of the file's MD5 (no dashes).
    // remotePath: destination path on the device, e.g. "/tmp/pc/firmware.bin".
    // localPath: source path on this machine; right-truncated to the last 128
    // chars if longer, matching FileTransferSession.BuildFileInfo.
    public static byte[] Build(string md5Hex, int length, string remotePath, string localPath)
    {
        byte[] buf = new byte[TOTAL_LENGTH];
        int o = 0;

        WriteAscii(buf, o, MD5_FIELD_LENGTH, md5Hex); o += MD5_FIELD_LENGTH;
        System.BitConverter.GetBytes(length).CopyTo(buf, o); o += 4;
        System.BitConverter.GetBytes(1).CopyTo(buf, o); o += 4; // saveAsFile
        WriteAscii(buf, o, PATH_FIELD_LENGTH, remotePath); o += PATH_FIELD_LENGTH;

        string localForWire = localPath.Replace('\\', '/');
        if (localForWire.Length > PATH_FIELD_LENGTH)
        {
            localForWire = localForWire[^PATH_FIELD_LENGTH..];
        }
        WriteAscii(buf, o, PATH_FIELD_LENGTH, localForWire); o += PATH_FIELD_LENGTH;

        return buf;
    }

    private static void WriteAscii(byte[] buf, int offset, int fieldLength, string value)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(value);
        int n = System.Math.Min(bytes.Length, fieldLength);
        System.Array.Copy(bytes, 0, buf, offset, n);
    }
}
