using System;

namespace Caddx_PCsoftware;

public class ClientrEventArgs : EventArgs
{
	public string Data { get; set; }

	public string ServerIP { get; set; }

	public int ServerPort { get; set; }

	public int ServerIdx { get; set; }

	public int ErrCode { get; set; } = 0;

	public string ErrDesc { get; set; } = "";

	public ClientrEventArgs(string data = "", string ip = "127.0.0.1", int port = 5088, int idx = 0)
	{
		Data = data;
		ServerIP = ip;
		ServerPort = port;
		ServerIdx = idx;
	}
}
