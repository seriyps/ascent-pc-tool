using System;
using System.Drawing;
using System.IO.Ports;
using System.Threading;

namespace Caddx_PCTool;

public class MavSerialport
{
	private BlockQueue<SPEventArgs> _queue;

	private bool _isRunSendThread;

	private Thread _sendThr;

	private bool _isSPReadRun;

	private Thread _spReadThr;

	private SerialPort _sp;

	private object readlock = new object();

	private MAVLink.MavlinkParse _mavParse = new MAVLink.MavlinkParse();

	public SerialPort SPObj => _sp;

	public event EventHandler<MavSP> OnSPReceivedEvnet;

	public event EventHandler<ErrorEventArgs> OnSPErrorHappenEvnet;

	public bool OpenSP(string comName, int baudRate = 115200)
	{
		try
		{
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public bool InitSendQueue()
	{
		if (_sp == null || !_sp.IsOpen)
		{
			return false;
		}
		_queue = new BlockQueue<SPEventArgs>(100);
		_isRunSendThread = true;
		_sendThr = new Thread(SPSendThreadMethod);
		_sendThr.Start(0);
		_isSPReadRun = true;
		_spReadThr = new Thread(SPReadMethod);
		_spReadThr.Start();
		return true;
	}

	private void SPSendThreadMethod(object workIdx)
	{
		try
		{
			while (_isRunSendThread)
			{
				int idx = (int)workIdx;
				SerialportSend(idx);
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("SPSendThreadMethod error happen，Desc=" + ex.Message, Color.Red);
		}
	}

	private void SerialportSend(int idx)
	{
		int num = 10;
		try
		{
			if (_queue.TryDequeue(out var value))
			{
				_sp.Write(value.DataBuffer, 0, value.DataBuffer.Length);
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("SerialportSend error:" + ex.Message, Color.Red);
		}
	}

	private void SPReadMethod()
	{
		while (_isSPReadRun)
		{
			MAVLink.MAVLinkMessage mAVLinkMessage = new MAVLink.MAVLinkMessage();
			try
			{
				lock (readlock)
				{
					if (_sp.IsOpen)
					{
						mAVLinkMessage = _mavParse.ReadPacket(_sp.BaseStream);
						if (mAVLinkMessage == null || mAVLinkMessage.data == null)
						{
							continue;
						}
						goto end_IL_0018;
					}
					end_IL_0018:;
				}
				uint msgid = mAVLinkMessage.msgid;
				string msgtypename = mAVLinkMessage.msgtypename;
			}
			catch (Exception)
			{
			}
		}
	}
}
