using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Caddx_PCsoftware;

public class TCPServerHelper
{
	private bool _isRunAccept;

	private Thread _acceptThr;

	private bool _isRunReceive;

	private List<Thread> _receiveThrList;

	private TcpListener _listener;

	private List<TcpClient> _clientList;

	private List<NetworkStream> _netStreamList;

	private IPAddress _ip;

	private int _port;

	private int _clientIdx;

	public event EventHandler<ServerEventArgs> OnClientConnectEvent;

	public event EventHandler<ServerEventArgs> OnClientDisconnectEvent;

	public event EventHandler<ServerEventArgs> OnServerReceiveDataEvent;

	public event EventHandler<ServerEventArgs> OnServerErrorHappenEvent;

	public TCPServerHelper()
	{
		_ip = IPAddress.Parse("127.0.0.1");
		_port = 5088;
	}

	public TCPServerHelper(string ip, int port, int clientIdx)
	{
		_ip = IPAddress.Parse(ip);
		_port = port;
		_clientIdx = clientIdx;
	}

	public bool OpenServer()
	{
		bool result = false;
		try
		{
			_listener = new TcpListener(_ip, _port);
			_listener.Start();
			_isRunAccept = true;
			_acceptThr = new Thread(AcceptThreadMethod);
			_acceptThr.Priority = ThreadPriority.Lowest;
			_acceptThr.IsBackground = true;
			_acceptThr.Start();
			_clientList = new List<TcpClient>();
			_netStreamList = new List<NetworkStream>();
			_receiveThrList = new List<Thread>();
			result = true;
			return result;
		}
		catch (Exception)
		{
			return result;
		}
	}

	public void CloseServer()
	{
		if (_listener != null)
		{
			_isRunAccept = false;
			_acceptThr = null;
		}
	}

	public bool SendToClient(string msg)
	{
		bool result = false;
		ServerEventArgs e = new ServerEventArgs();
		try
		{
			if (_clientList == null || _clientList.Count < 1)
			{
				return result;
			}
			if (_clientList[0] == null || !_clientList[0].Connected)
			{
				return result;
			}
			byte[] bytes = Encoding.Default.GetBytes(msg);
			_netStreamList[0].Write(bytes, 0, bytes.Length);
			result = true;
			return result;
		}
		catch (Exception ex)
		{
			e.ErrDesc = ex.Message;
			e.ErrCode = 3;
			OnServerErrorHappenEvent?.Invoke(null, e);
			return result;
		}
	}

	private void AcceptThreadMethod()
	{
		ServerEventArgs e = new ServerEventArgs();
		try
		{
			while (_isRunAccept)
			{
				TcpClient tcpClient = _listener.AcceptTcpClient();
				_clientList.Add(tcpClient);
				_netStreamList.Add(tcpClient.GetStream());
				string[] array = tcpClient.Client.RemoteEndPoint.ToString().Split(new char[1] { ':' });
				e.ClientIP = array[0];
				e.ClientPort = int.Parse(array[1]);
				e.ClientIdx = _clientIdx;
				OnClientConnectEvent?.Invoke(tcpClient, e);
				_isRunReceive = true;
				Thread thread;
				(thread = new Thread((ThreadStart)delegate
				{
					ReceiveThreadMethod(_clientList.Count - 1);
				})).Start();
				thread.IsBackground = true;
				_receiveThrList.Add(thread);
			}
		}
		catch (Exception ex)
		{
			e = new ServerEventArgs();
			e.ErrDesc = ex.Message;
			e.ErrCode = 1;
			OnServerErrorHappenEvent?.Invoke(null, e);
		}
	}

	private void ReceiveThreadMethod(int idx)
	{
		ServerEventArgs e = new ServerEventArgs();
		try
		{
			while (_isRunReceive)
			{
				byte[] array = new byte[_clientList[idx].ReceiveBufferSize];
				int num = _netStreamList[idx].Read(array, 0, array.Length);
				e.ClientIdx = _clientIdx;
				string[] array2 = _clientList[idx].Client.RemoteEndPoint.ToString().Split(new char[1] { ':' });
				e.ClientIP = array2[0];
				e.ClientPort = int.Parse(array2[1]);
				if (num == 0)
				{
					OnClientDisconnectEvent?.Invoke(_clientList[idx], e);
					_clientList.Remove(_clientList[idx]);
					_netStreamList.Remove(_netStreamList[idx]);
					_receiveThrList.Remove(_receiveThrList[idx]);
					break;
				}
				string data = Encoding.Default.GetString(array, 0, num);
				e.Data = data;
				OnServerReceiveDataEvent?.Invoke(_clientList[idx], e);
			}
		}
		catch (Exception ex)
		{
			e.ErrDesc = ex.Message;
			e.ErrCode = 2;
			OnServerErrorHappenEvent?.Invoke(null, e);
		}
	}
}
