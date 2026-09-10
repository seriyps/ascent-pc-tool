using System.Linq;
using System.Text;

namespace CaddxTool.Protocol.Tests;

public class AscentClientTests
{
    // Mirrors ResDeviceInfoV2's 300-byte layout: ReceiveMaxSize:int32,
    // SdkVersion[32], DeviceName[64], CpuTemp:int32, FirmwareInfo[64],
    // SerialNumber[32], HardwareVersion[32], Status:int32, Detail[64].
    private static byte[] BuildDeviceInfoPayload(string deviceName, string firmware, int cpuTemp)
    {
        byte[] buf = new byte[300];
        int o = 0;
        BitConverter.GetBytes(1048576).CopyTo(buf, o); o += 4; // ReceiveMaxSize
        o += 32; // SdkVersion, left zeroed
        Encoding.ASCII.GetBytes(deviceName).CopyTo(buf, o); o += 64;
        BitConverter.GetBytes(cpuTemp).CopyTo(buf, o); o += 4;
        Encoding.ASCII.GetBytes(firmware).CopyTo(buf, o); o += 64;
        o += 32; // SerialNumber
        o += 32; // HardwareVersion
        o += 4;  // Status
        return buf;
    }

    [Fact]
    public void GetDeviceInfo_HappyPath_ParsesResponse()
    {
        var transport = new FakeAscentTransport();
        transport.OnSend = sent =>
        {
            Assert.Equal(ArConstantsV2.CMD_FIND_DEVICE, sent.Cmd);
            byte[] payload = BuildDeviceInfoPayload("Ascent_lite_plus", "Ascent_H_Sky_18_21_10", 56);
            return FakeAscentTransport.BuildResponse(ArConstantsV2.CMD_FIND_DEVICE, sent.Seq, payload);
        };
        using var client = new AscentClient(transport);

        ResDeviceInfoV2 info = client.GetDeviceInfo();

        Assert.Equal(1048576, info.ReceiveMaxSize);
        Assert.Equal(56, info.CpuTemp);
        Assert.Contains("Ascent_lite_plus", Encoding.ASCII.GetString(info.DeviceName));
        Assert.Contains("Ascent_H_Sky_18_21_10", Encoding.ASCII.GetString(info.FirmwareInfo));
    }

    [Fact]
    public void GetDeviceInfo_BadMagic_Throws()
    {
        var transport = new FakeAscentTransport();
        transport.OnSend = sent =>
        {
            byte[] resp = FakeAscentTransport.BuildResponse(ArConstantsV2.CMD_FIND_DEVICE, sent.Seq, BuildDeviceInfoPayload("x", "y", 0));
            resp[0] ^= 0xFF; // corrupt the magic field
            return resp;
        };
        using var client = new AscentClient(transport);

        Assert.Throws<InvalidOperationException>(() => client.GetDeviceInfo());
    }

    [Fact]
    public void SendWithAck_RetriesOnSilentDrops_ThenSucceeds()
    {
        var clock = new ManualClock();
        var transport = new FakeAscentTransport { Clock = clock };
        int attempt = 0;
        transport.OnSend = sent =>
        {
            attempt++;
            if (attempt < 3)
            {
                return null; // simulate no response, forcing a resend
            }
            return FakeAscentTransport.BuildResponse(ArConstantsV2.CMD_FIND_DEVICE, sent.Seq);
        };
        var client = new AscentClient(transport, clock.Now);

        AckResult ack = client.SendWithAck(ArConstantsV2.CMD_FIND_DEVICE, payload: null);

        Assert.True(ack.IsSuccess);
        Assert.Equal(2u, ack.RetryCount);
        Assert.Equal(3, attempt);
    }

    [Fact]
    public void SendWithAck_NeverResponds_TimesOutWithinTotalBudget()
    {
        var clock = new ManualClock();
        var transport = new FakeAscentTransport { Clock = clock };
        transport.OnSend = _ => null;
        var client = new AscentClient(transport, clock.Now);

        AckResult ack = client.SendWithAck(ArConstantsV2.CMD_FIND_DEVICE, payload: null, totalTimeoutMs: 5000);

        Assert.Equal(AckStatus.Timeout, ack.Status);
        Assert.True(ack.RetryCount >= 1);
    }

    [Fact]
    public void SendWithAck_IgnoresUnrelatedPacket_ThenMatches()
    {
        var transport = new FakeAscentTransport();
        transport.OnSend = sent =>
        {
            // Respond with a stale ack for a different seq first, then the real one,
            // by queuing both into the same response.
            byte[] stale = FakeAscentTransport.BuildResponse(ArConstantsV2.CMD_FIND_DEVICE, seq: 999);
            byte[] real = FakeAscentTransport.BuildResponse(ArConstantsV2.CMD_FIND_DEVICE, sent.Seq);
            return stale.Concat(real).ToArray();
        };
        using var client = new AscentClient(transport);

        AckResult ack = client.SendWithAck(ArConstantsV2.CMD_FIND_DEVICE, payload: null);

        Assert.True(ack.IsSuccess);
        Assert.NotEqual(999u, ack.Seq);
    }
}
