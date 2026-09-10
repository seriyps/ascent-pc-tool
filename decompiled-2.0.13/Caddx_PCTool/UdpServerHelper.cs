using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Caddx_PCTool;

public class UdpServerHelper
{
	private UdpClient udpServer;

	private IPEndPoint remoteEndPoint;

	private bool isRunning;

	private Thread receiveThread;

	public int ListenPort { get; private set; }

	public event Action<string, byte[], IPEndPoint> OnMessageReceived;

	public UdpServerHelper(string ip, int port)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		try
		{
			ListenPort = port;
			udpServer = new UdpClient(port);
			remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	public void Start()
	{
		if (!isRunning)
		{
			isRunning = true;
			receiveThread = new Thread(ReceiveLoop);
			receiveThread.IsBackground = true;
			receiveThread.Start();
			Console.WriteLine($"UDP服务器已启动，监听端口: {ListenPort}");
		}
	}

	public void Stop()
	{
		isRunning = false;
		udpServer?.Close();
		receiveThread?.Join();
		Console.WriteLine("UDP服务器已停止");
	}

	public void Send(string message, string ip, int port)
	{
		try
		{
			byte[] bytes = Encoding.UTF8.GetBytes(message);
			udpServer.Send(bytes, bytes.Length, ip, port);
		}
		catch (Exception ex)
		{
			Console.WriteLine("发送失败: " + ex.Message);
		}
	}

	public void Send(byte[] msg, int port = 14550, string ip = "127.0.0.1")
	{
		try
		{
			udpServer.Send(msg, msg.Length, ip, port);
		}
		catch (Exception ex)
		{
			Console.WriteLine("发送失败: " + ex.Message);
		}
	}

	public void Send(string message)
	{
		try
		{
			byte[] bytes = Encoding.UTF8.GetBytes(message);
			int num = udpServer.Send(bytes, bytes.Length, "127.0.0.1", 14550);
		}
		catch (Exception ex)
		{
			Console.WriteLine("发送失败: " + ex.Message);
		}
	}

	private void ReceiveLoop()
	{
		try
		{
			while (isRunning)
			{
				byte[] array = udpServer.Receive(ref remoteEndPoint);
				string arg = Encoding.UTF8.GetString(array);
				OnMessageReceived?.Invoke(arg, array, remoteEndPoint);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("接收错误: " + ex.Message);
		}
	}
}
