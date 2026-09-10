using System;
using System.Drawing;

namespace Caddx_PCTool;

public static class AscentCommParseData
{
	public static short ParseDeviceInfo(byte[] frame, out ResDeviceInfo devInfo)
	{
		short result = -1;
		devInfo = new ResDeviceInfo();
		try
		{
			devInfo.receiveMaxSize = BitConverter.ToInt32(frame, 0);
			int sourceIndex = 4;
			Array.Copy(frame, sourceIndex, devInfo.sdkversion, 0, 32);
			int num = 36;
			Array.Copy(frame, num, devInfo.devicename, 0, 64);
			int num2 = num + 64;
			devInfo.cputemp = BitConverter.ToInt32(frame, num2);
			int num3 = num2 + 4;
			Array.Copy(frame, num3, devInfo.firmwareInfo, 0, 64);
			int num4 = num3 + 64;
			Array.Copy(frame, num4, devInfo.serialNumber, 0, 32);
			int num5 = num4 + 32;
			Array.Copy(frame, num5, devInfo.hardwareVersion, 0, 32);
			int num6 = num5 + 32;
			devInfo.status = BitConverter.ToInt32(frame, num6);
			int sourceIndex2 = num6 + 4;
			Array.Copy(frame, sourceIndex2, devInfo.detail, 0, 64);
			return result;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("AscentCommParseData.ParseDeviceInfo Exception: " + ex.Message, Color.DarkBlue);
			return result;
		}
	}
}
