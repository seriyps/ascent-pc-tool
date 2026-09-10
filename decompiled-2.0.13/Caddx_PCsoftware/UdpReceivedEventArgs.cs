using System;
using System.Net;

namespace Caddx_PCsoftware;

public class UdpReceivedEventArgs : EventArgs
{
	public IPEndPoint RemoteEndPoint { get; }

	public string Message { get; }

	public byte[] DataReceived { get; }

	public UdpReceivedEventArgs(IPEndPoint remoteEndPoint, string message, byte[] dataReceived)
	{
		RemoteEndPoint = remoteEndPoint;
		Message = message;
		DataReceived = dataReceived;
	}
}
