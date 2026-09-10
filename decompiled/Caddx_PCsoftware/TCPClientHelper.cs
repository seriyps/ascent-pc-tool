using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Caddx_PCsoftware;

public class TCPClientHelper
{
	private TcpClient _client;

	private NetworkStream _stream;

	private Thread threadReceive;

	private bool _isRunRec;

	private bool _isConnect;

	private string _ip;

	private int _port;

	private int _idx;

	private byte[] _buffer = new byte[2048];

	public bool _isConnectServer => _isConnect;

	public event EventHandler<ClientrEventArgs> OnClientReceiveDataEvent;

	public TCPClientHelper(string ip = "", int prot = 5088, int idx = 0)
	{
		_ip = ip;
		_port = prot;
		_idx = idx;
	}

	public void ConnectServer()
	{
		try
		{
			_client = new TcpClient(_ip, _port);
			_isConnect = _client.Connected;
			if (_isConnect)
			{
				_stream = _client.GetStream();
				_isRunRec = true;
				threadReceive = new Thread(ReceiveCallBack);
				threadReceive.IsBackground = true;
				threadReceive.Priority = ThreadPriority.BelowNormal;
				threadReceive.Start();
			}
		}
		catch (Exception)
		{
		}
	}

	public void CloseClient()
	{
		try
		{
			_isRunRec = false;
			Thread.Sleep(200);
			threadReceive = null;
			_client?.Close();
		}
		catch (Exception)
		{
		}
	}

	public bool SendToServer(string msg)
	{
		if (_client == null || !_isConnect || msg.Length < 1)
		{
			return false;
		}
		byte[] bytes = Encoding.ASCII.GetBytes(msg);
		_stream.Write(bytes, 0, bytes.Length);
		return true;
	}

	private void ReceiveCallBack()
	{
		try
		{
			ClientrEventArgs e = new ClientrEventArgs();
			while (_isRunRec)
			{
				int num = _stream.Read(_buffer, 0, _buffer.Length);
				if (num != 0)
				{
					e.Data = Encoding.ASCII.GetString(_buffer, 0, num);
					e.ServerIP = _ip;
					e.ServerIdx = _idx;
					OnClientReceiveDataEvent?.Invoke("", e);
				}
			}
		}
		catch (Exception)
		{
		}
	}
}
