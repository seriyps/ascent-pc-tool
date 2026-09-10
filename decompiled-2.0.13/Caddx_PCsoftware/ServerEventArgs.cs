using System;

namespace Caddx_PCsoftware;

public class ServerEventArgs : EventArgs
{
	public string Data { get; set; }

	public string ClientIP { get; set; }

	public int ClientPort { get; set; }

	public int ClientIdx { get; set; }

	public int ErrCode { get; set; } = 0;

	public string ErrDesc { get; set; } = "";

	public ServerEventArgs(string data = "", string ip = "127.0.0.1", int port = 5088, int idx = 0)
	{
		Data = data;
		ClientIP = ip;
		ClientPort = port;
		ClientIdx = idx;
	}
}
