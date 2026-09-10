using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Caddx_PCTool;

public class CLIModeFrm : UserControl
{
	private Process _puProc;

	private string _cliStr;

	private string _testStr;

	private string _customerStr;

	private List<string> _testList;

	private string[] _testArr;

	private Stopwatch _sw_FunTest;

	private IContainer components = null;

	private Panel panel1;

	private Button button1;

	private RichTextBox richTextBox1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem 粘贴ToolStripMenuItem;

	private ToolStripMenuItem 复制测试文本ToolStripMenuItem;

	private ToolStripMenuItem 复制重置命令ToolStripMenuItem;

	private ToolStripMenuItem 复制客户文本ToolStripMenuItem;

	private ToolStripMenuItem 全选ToolStripMenuItem;

	private ToolStripMenuItem 发送cli指令ToolStripMenuItem;

	private ToolStripMenuItem 激活CLI模式ToolStripMenuItem;

	private RichTextBox rtb_Recv;

	private Label lab_toolTip;

	private ToolStripMenuItem 退出cli模式ToolStripMenuItem;

	private ToolStripMenuItem 发送全功能测试ToolStripMenuItem;

	private ToolStripMenuItem 读取文本ToolStripMenuItem;

	public event EventHandler<CLIModeFrmEventArgs> OnCLIModeFrmEvnet;

	public CLIModeFrm()
	{
		InitializeComponent();
		_sw_FunTest = new Stopwatch();
	}

	private void CLI_ModeFrm_Load(object sender, EventArgs e)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)lab_toolTip).Text = "........";
			((Control)lab_toolTip).BackColor = Control.DefaultBackColor;
		});
	}

	private void StartPutty(string arguments)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		((Control)panel1).Controls.Clear();
		_puProc = new Process();
		_puProc.StartInfo.FileName = "putty.exe";
		_puProc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
		_puProc.StartInfo.UseShellExecute = false;
		if (!_puProc.Start())
		{
			MessageBox.Show("无法启动PuTTY进程", "错误", (MessageBoxButtons)0, (MessageBoxIcon)16);
			return;
		}
		_puProc.WaitForInputIdle();
		Thread.Sleep(100);
		IntPtr mainWindowHandle = _puProc.MainWindowHandle;
		Win32.SetParent(mainWindowHandle, ((Control)panel1).Handle);
		int windowLong = Win32.GetWindowLong(mainWindowHandle, -16);
		windowLong = 0x50000000 | (windowLong & -12582913 & -8388609);
		Win32.SetWindowLong(mainWindowHandle, -16, windowLong);
		Win32.MoveWindow(mainWindowHandle, 0, 0, ((Control)panel1).ClientSize.Width, ((Control)panel1).ClientSize.Height, bRepaint: true);
	}

	private void CLI_ModeFrm_FormClosing(object sender, FormClosingEventArgs e)
	{
		((Control)this).Hide();
		((CancelEventArgs)(object)e).Cancel = true;
	}

	private void button1_Click(object sender, EventArgs e)
	{
		Thread.Sleep(100);
		StartPutty(" ");
	}

	private void ReadTextFileAsync(string filePath, int type)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			using StreamReader streamReader = new StreamReader(filePath);
			switch (type)
			{
			case 1:
				_testStr = streamReader.ReadToEnd();
				break;
			case 2:
				_customerStr = streamReader.ReadToEnd();
				break;
			default:
				_cliStr = streamReader.ReadToEnd();
				break;
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("读取失败: " + ex.Message, "错误", (MessageBoxButtons)0, (MessageBoxIcon)16);
		}
		finally
		{
		}
	}

	private void ReadTextFile(string filePath)
	{
		_testList = new List<string>();
		using StreamReader streamReader = new StreamReader(filePath);
		string text;
		while ((text = streamReader.ReadLine()) != null)
		{
			string text2 = text.Trim();
			if (!string.IsNullOrEmpty(text2) && !text2.StartsWith("#"))
			{
				_testList.Add(text2);
			}
		}
	}

	private void 复制测试文本ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		((TextBoxBase)richTextBox1).Clear();
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)richTextBox1).Text = _testStr;
		});
	}

	private void 复制重置命令ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		((Control)richTextBox1).Text = "defaults";
	}

	private void 复制客户文本ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		((TextBoxBase)richTextBox1).Clear();
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)richTextBox1).Text = _customerStr;
		});
	}

	private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		((TextBoxBase)richTextBox1).Paste();
	}

	private void 全选ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		((TextBoxBase)richTextBox1).Clear();
		((TextBoxBase)rtb_Recv).Clear();
	}

	public void SendCLIMsg(bool isReopen)
	{
		if (GD.Inst.MySP.SPObj != null && GD.Inst.MySP.IsOpen)
		{
			string text = ((Control)richTextBox1).Text;
			GD.Inst.MySP.SendCLIMsg(text);
			GD.Inst.CurrMSPMode = MSPMode.cli;
			GD.Inst.DelayMethod(5500);
			if (isReopen)
			{
				GD.Inst.MySP.ReopenSP();
			}
			else
			{
				GD.Inst.MySP.OpenSerialPortEx();
			}
		}
	}

	public void SendCLIMsg(string msg, bool isReopen)
	{
		if (GD.Inst.MySP.SPObj != null && GD.Inst.MySP.IsOpen)
		{
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				((Control)richTextBox1).Text = msg;
			});
			GD.Inst.MySP.SendCLIMsg(msg);
			GD.Inst.CurrMSPMode = MSPMode.cli;
			GD.Inst.DelayMethod(GD.Inst.ReElectDelay);
			if (isReopen)
			{
				GD.Inst.MySP.ReopenSP();
			}
			else
			{
				GD.Inst.MySP.OpenSerialPortEx();
			}
			GD.Inst.CurrMSPMode = MSPMode.msp;
		}
	}

	private void 发送cli指令ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		SendCLIMsg(isReopen: true);
	}

	private void 激活CLI模式ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (!GD.Inst.IsUseGyroTimer && !GD.Inst.IsUseAttitTimer && GD.Inst.MySP.IsOpen)
		{
			((TextBoxBase)rtb_Recv).Clear();
			((TextBoxBase)richTextBox1).Clear();
			GD.Inst.CurrMSPMode = MSPMode.cli;
			GD.Inst.MySP.SendCLIMsg("#");
		}
	}

	public void UpdataRecvRtb(string msg)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((TextBoxBase)rtb_Recv).AppendText("\r\n" + msg);
		});
	}

	private void 退出cli模式ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)richTextBox1).Text = "exit";
		});
		SendCLIMsg(isReopen: true);
	}

	public void FuntionTest()
	{
		GD.Inst.SW_AllFuntionTest.Restart();
		_sw_FunTest.Restart();
		GD.Inst.CurrMSPMode = MSPMode.cli;
		if (GD.Inst.MySP.SPObj == null || !GD.Inst.MySP.IsOpen)
		{
			WriteLog.WriteLogFileToUI("输入全功能测试时，重新打开串口", Color.Orange);
			Thread.Sleep(150);
		}
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)lab_toolTip).Text = "........";
			((Control)lab_toolTip).BackColor = Control.DefaultBackColor;
			((Control)richTextBox1).Text = "#";
			((TextBoxBase)rtb_Recv).Clear();
		});
		GD.Inst.MySP.SendCLIMsg("#");
		GD.Inst.DelayMethod(300);
		int i;
		for (i = 0; i < _testList.Count; i++)
		{
			GD.Inst.MySP.SendCLIMsg(_testList[i]);
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				((TextBoxBase)richTextBox1).AppendText(_testList[i] + "\r\n");
			});
			Thread.Sleep(GD.Inst.FontDelay);
		}
		_sw_FunTest.Stop();
		WriteLog.WriteLogFileToUI("ct=" + _sw_FunTest.ElapsedMilliseconds, Color.Black);
		GD.Inst.DelayMethod(GD.Inst.ReElectDelay);
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)lab_toolTip).Text = "完成";
			((Control)lab_toolTip).BackColor = Color.Green;
		});
		GD.Inst.SW_AllFuntionTest.Stop();
	}

	public void DefaultsTest()
	{
		int num = 0;
		GD.Inst.CurrMSPMode = MSPMode.cli;
		if (GD.Inst.MySP.SPObj == null || !GD.Inst.MySP.IsOpen)
		{
			WriteLog.WriteLogFileToUI("输入重置命令时，重新打开串口", Color.Orange);
			Thread.Sleep(150);
		}
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((TextBoxBase)rtb_Recv).Clear();
			((TextBoxBase)richTextBox1).Clear();
			((Control)lab_toolTip).Text = "........";
			((Control)lab_toolTip).BackColor = Control.DefaultBackColor;
			((Control)richTextBox1).Text = "#";
		});
		GD.Inst.MySP.SendCLIMsg("#");
		GD.Inst.DelayMethod(1000);
		SendCLIMsg("defaults", isReopen: true);
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)lab_toolTip).Text = "完成";
			((Control)lab_toolTip).BackColor = Color.Green;
		});
	}

	public void ClearAll()
	{
		((TextBoxBase)richTextBox1).Clear();
		((TextBoxBase)rtb_Recv).Clear();
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			((Control)lab_toolTip).Text = "......";
			((Control)lab_toolTip).BackColor = Control.DefaultBackColor;
		});
	}

	private void 发送全功能测试ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		GD.Inst.CurrMSPMode = MSPMode.cli;
		_sw_FunTest.Restart();
		int i;
		for (i = 0; i < _testList.Count; i++)
		{
			GD.Inst.MySP.SendCLIMsg(_testList[i]);
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				((TextBoxBase)richTextBox1).AppendText(_testList[i] + "\r\n");
			});
			Thread.Sleep(25);
		}
		_sw_FunTest.Stop();
		WriteLog.WriteLogFileToUI("ct=" + _sw_FunTest.ElapsedMilliseconds, Color.Black);
	}

	private void 读取文本ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		ReadTextFile(GD.Inst.TestPath);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((ContainerControl)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Expected O, but got Unknown
		components = new Container();
		panel1 = new Panel();
		rtb_Recv = new RichTextBox();
		button1 = new Button();
		richTextBox1 = new RichTextBox();
		contextMenuStrip1 = new ContextMenuStrip(components);
		激活CLI模式ToolStripMenuItem = new ToolStripMenuItem();
		复制测试文本ToolStripMenuItem = new ToolStripMenuItem();
		复制重置命令ToolStripMenuItem = new ToolStripMenuItem();
		复制客户文本ToolStripMenuItem = new ToolStripMenuItem();
		退出cli模式ToolStripMenuItem = new ToolStripMenuItem();
		粘贴ToolStripMenuItem = new ToolStripMenuItem();
		全选ToolStripMenuItem = new ToolStripMenuItem();
		发送cli指令ToolStripMenuItem = new ToolStripMenuItem();
		lab_toolTip = new Label();
		发送全功能测试ToolStripMenuItem = new ToolStripMenuItem();
		读取文本ToolStripMenuItem = new ToolStripMenuItem();
		((Control)panel1).SuspendLayout();
		((Control)contextMenuStrip1).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)panel1).BackColor = Color.Black;
		((Control)panel1).Controls.Add((Control)(object)rtb_Recv);
		((Control)panel1).Location = new Point(0, 0);
		((Control)panel1).Margin = new Padding(0);
		((Control)panel1).Name = "panel1";
		((Control)panel1).Size = new Size(459, 425);
		((Control)panel1).TabIndex = 0;
		((Control)rtb_Recv).BackColor = Color.FromArgb(64, 64, 64);
		((TextBoxBase)rtb_Recv).BorderStyle = (BorderStyle)0;
		((Control)rtb_Recv).Dock = (DockStyle)5;
		((Control)rtb_Recv).ForeColor = Color.White;
		((Control)rtb_Recv).Location = new Point(0, 0);
		((Control)rtb_Recv).Margin = new Padding(0);
		((Control)rtb_Recv).Name = "rtb_Recv";
		((Control)rtb_Recv).Size = new Size(459, 425);
		((Control)rtb_Recv).TabIndex = 0;
		((Control)rtb_Recv).Text = "1234";
		((Control)button1).Location = new Point(462, 12);
		((Control)button1).Name = "button1";
		((Control)button1).Size = new Size(58, 43);
		((Control)button1).TabIndex = 1;
		((Control)button1).Text = "打开putty";
		((ButtonBase)button1).UseVisualStyleBackColor = true;
		((Control)button1).Visible = false;
		((Control)button1).Click += button1_Click;
		((Control)richTextBox1).BackColor = Color.Silver;
		((TextBoxBase)richTextBox1).BorderStyle = (BorderStyle)0;
		((Control)richTextBox1).Location = new Point(0, 428);
		((Control)richTextBox1).Name = "richTextBox1";
		((Control)richTextBox1).Size = new Size(459, 394);
		((Control)richTextBox1).TabIndex = 5;
		((Control)richTextBox1).Text = "1234";
		((ToolStrip)contextMenuStrip1).Items.AddRange((ToolStripItem[])(object)new ToolStripItem[10]
		{
			(ToolStripItem)激活CLI模式ToolStripMenuItem,
			(ToolStripItem)复制测试文本ToolStripMenuItem,
			(ToolStripItem)复制重置命令ToolStripMenuItem,
			(ToolStripItem)复制客户文本ToolStripMenuItem,
			(ToolStripItem)退出cli模式ToolStripMenuItem,
			(ToolStripItem)粘贴ToolStripMenuItem,
			(ToolStripItem)全选ToolStripMenuItem,
			(ToolStripItem)发送cli指令ToolStripMenuItem,
			(ToolStripItem)发送全功能测试ToolStripMenuItem,
			(ToolStripItem)读取文本ToolStripMenuItem
		});
		((Control)contextMenuStrip1).Name = "contextMenuStrip1";
		((Control)contextMenuStrip1).Size = new Size(181, 246);
		((ToolStripItem)激活CLI模式ToolStripMenuItem).Name = "激活CLI模式ToolStripMenuItem";
		((ToolStripItem)激活CLI模式ToolStripMenuItem).Size = new Size(180, 22);
		((ToolStripItem)激活CLI模式ToolStripMenuItem).Text = "激活CLI模式";
		((ToolStripItem)激活CLI模式ToolStripMenuItem).Click += 激活CLI模式ToolStripMenuItem_Click;
		((ToolStripItem)复制测试文本ToolStripMenuItem).Name = "复制测试文本ToolStripMenuItem";
		((ToolStripItem)复制测试文本ToolStripMenuItem).Size = new Size(180, 22);
		((ToolStripItem)复制测试文本ToolStripMenuItem).Text = "复制测试文本";
		((ToolStripItem)复制测试文本ToolStripMenuItem).Click += 复制测试文本ToolStripMenuItem_Click;
		((ToolStripItem)复制重置命令ToolStripMenuItem).Name = "复制重置命令ToolStripMenuItem";
		((ToolStripItem)复制重置命令ToolStripMenuItem).Size = new Size(180, 22);
		((ToolStripItem)复制重置命令ToolStripMenuItem).Text = "复制重置命令";
		((ToolStripItem)复制重置命令ToolStripMenuItem).Click += 复制重置命令ToolStripMenuItem_Click;
		((ToolStripItem)复制客户文本ToolStripMenuItem).Name = "复制客户文本ToolStripMenuItem";
		((ToolStripItem)复制客户文本ToolStripMenuItem).Size = new Size(180, 22);
		((ToolStripItem)复制客户文本ToolStripMenuItem).Text = "复制客户文本";
		((ToolStripItem)复制客户文本ToolStripMenuItem).Click += 复制客户文本ToolStripMenuItem_Click;
		((ToolStripItem)退出cli模式ToolStripMenuItem).Name = "退出cli模式ToolStripMenuItem";
		((ToolStripItem)退出cli模式ToolStripMenuItem).Size = new Size(180, 22);
		((ToolStripItem)退出cli模式ToolStripMenuItem).Text = "退出CLI模式";
		((ToolStripItem)退出cli模式ToolStripMenuItem).Click += 退出cli模式ToolStripMenuItem_Click;
		((ToolStripItem)粘贴ToolStripMenuItem).Name = "粘贴ToolStripMenuItem";
		((ToolStripItem)粘贴ToolStripMenuItem).Size = new Size(180, 22);
		((ToolStripItem)粘贴ToolStripMenuItem).Text = "粘贴";
		((ToolStripItem)粘贴ToolStripMenuItem).Click += 粘贴ToolStripMenuItem_Click;
		((ToolStripItem)全选ToolStripMenuItem).Name = "全选ToolStripMenuItem";
		((ToolStripItem)全选ToolStripMenuItem).Size = new Size(180, 22);
		((ToolStripItem)全选ToolStripMenuItem).Text = "全部清空";
		((ToolStripItem)全选ToolStripMenuItem).Click += 全选ToolStripMenuItem_Click;
		((ToolStripItem)发送cli指令ToolStripMenuItem).Name = "发送cli指令ToolStripMenuItem";
		((ToolStripItem)发送cli指令ToolStripMenuItem).Size = new Size(180, 22);
		((ToolStripItem)发送cli指令ToolStripMenuItem).Text = "发送cli指令";
		((ToolStripItem)发送cli指令ToolStripMenuItem).Click += 发送cli指令ToolStripMenuItem_Click;
		((Control)lab_toolTip).AutoSize = true;
		((Control)lab_toolTip).Font = new Font("宋体", 18f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_toolTip).Location = new Point(465, 444);
		((Control)lab_toolTip).Name = "lab_toolTip";
		((Control)lab_toolTip).Size = new Size(106, 24);
		((Control)lab_toolTip).TabIndex = 6;
		((Control)lab_toolTip).Text = "........";
		((ToolStripItem)发送全功能测试ToolStripMenuItem).Name = "发送全功能测试ToolStripMenuItem";
		((ToolStripItem)发送全功能测试ToolStripMenuItem).Size = new Size(180, 22);
		((ToolStripItem)发送全功能测试ToolStripMenuItem).Text = "发送全功能测试";
		((ToolStripItem)发送全功能测试ToolStripMenuItem).Click += 发送全功能测试ToolStripMenuItem_Click;
		((ToolStripItem)读取文本ToolStripMenuItem).Name = "读取文本ToolStripMenuItem";
		((ToolStripItem)读取文本ToolStripMenuItem).Size = new Size(180, 22);
		((ToolStripItem)读取文本ToolStripMenuItem).Text = "读取文本";
		((ToolStripItem)读取文本ToolStripMenuItem).Click += 读取文本ToolStripMenuItem_Click;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).ClientSize = new Size(804, 825);
		((Control)this).ContextMenuStrip = contextMenuStrip1;
		((Control)this).Controls.Add((Control)(object)lab_toolTip);
		((Control)this).Controls.Add((Control)(object)richTextBox1);
		((Control)this).Controls.Add((Control)(object)button1);
		((Control)this).Controls.Add((Control)(object)panel1);
		((Control)this).Name = "CLI_ModeFrm";
		((Control)this).Text = "命令行模式";
		((UserControl)this).Load += CLI_ModeFrm_Load;
		((Control)panel1).ResumeLayout(false);
		((Control)contextMenuStrip1).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
