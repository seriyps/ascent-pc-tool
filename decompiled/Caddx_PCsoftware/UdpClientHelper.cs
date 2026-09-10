using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Caddx_PCsoftware;

public class UdpClientHelper
{
	private UdpClient _udpClient;

	private IPEndPoint _serverEndPoint;

	private IPEndPoint _localEndPoint;

	private bool _isListening = false;

	private Thread _listenThread;

	public bool IsConnect => _udpClient.Client.Connected;

	public bool IsListening => _isListening;

	public int LocalPort => _localEndPoint.Port;

	public event EventHandler<UdpReceivedEventArgs> DataReceived;

	public UdpClientHelper(int localPort = 0)
	{
		_localEndPoint = new IPEndPoint(IPAddress.Any, localPort);
		_udpClient = new UdpClient(_localEndPoint.Port);
		_localEndPoint = new IPEndPoint(IPAddress.Any, (_udpClient.Client.LocalEndPoint is IPEndPoint iPEndPoint) ? iPEndPoint.Port : 0);
	}

	public void Connect(string serverIp, int serverPort)
	{
		_serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
		_udpClient.Client.Connect("127.0.0.1", 14541);
	}

	public void Send(string msg)
	{
		if (_serverEndPoint != null)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(msg);
			_udpClient.Send(bytes, bytes.Length, _serverEndPoint);
		}
	}

	public void Send(byte[] msg)
	{
		if (_serverEndPoint != null)
		{
			_udpClient.Send(msg, msg.Length, _serverEndPoint);
		}
	}

	public void Send(byte[] msg, int port, string ip)
	{
		if (_serverEndPoint != null)
		{
			_udpClient.Send(msg, msg.Length, ip, port);
		}
	}

	public void SendTo(string ip, int port, string message)
	{
		IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
		byte[] bytes = Encoding.UTF8.GetBytes(message);
		_udpClient.Send(bytes, bytes.Length, endPoint);
	}

	public void StartReceive()
	{
		if (!_isListening)
		{
			_isListening = true;
			_listenThread = new Thread(ListenLoop);
			_listenThread.IsBackground = true;
			_listenThread.Start();
		}
	}

	public void StopReceive()
	{
		_isListening = false;
	}

	private void ListenLoop()
	{
		try
		{
			while (_isListening)
			{
				if (_udpClient.Client == null || !_udpClient.Client.Connected)
				{
					_udpClient.Client.Connect("127.0.0.1", 14541);
					Thread.Sleep(5);
					continue;
				}
				IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
				byte[] array = _udpClient.Receive(ref remoteEP);
				string message = Encoding.UTF8.GetString(array);
				DataReceived?.Invoke(this, new UdpReceivedEventArgs(remoteEP, message, array));
			}
		}
		catch (Exception ex) when (ex is SocketException || ex is ObjectDisposedException)
		{
			Console.WriteLine("监听结束或连接中断：" + ex.Message);
		}
	}

	public void Close()
	{
		_isListening = false;
		_udpClient.Close();
	}
}
