using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntdUI_Ex;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class SoftwareSettingFrm : UserControl
{
	private const string RfOffsetDefaultCommand = "41 53 57 00 01 14 01";

	private const string VtxMacModeEnableDefaultCommand = "41 53 57 00 02 14 01";

	private const string VtxMacModeDisableDefaultCommand = "41 53 57 00 02 14 00";

	private const int FirstAscentVid = 7541;

	private const int LastAscentVid = 7544;

	private readonly object _cliSendLock = new object();

	private SerialPortSessionHandle _cliSession;

	private string _cliDiscoveryRestorePort;

	private bool _cliSending;

	private bool _cliCompletionScheduled;

	private bool _cliReleaseInProgress;

	private string _className = "SoftwareSettingFrm";

	private IContainer components = null;

	private Button button1;

	private GridPanel grPan_Main;

	private GridPanel gridPanel4;

	private GridPanel gridPanel5;

	private Label label1;

	private GridPanel gridPanel6;

	private PictureBox pictureBox1;

	private Label lab_userAgree;

	private Label lab_lang;

	private PictureBox pictureBox2;

	private Label lab_software;

	private PictureBox pictureBox3;

	private Select sel_lang;

	private Select_Ex sel_cmd;

	private GridPanel gridPanel1;

	private Button btn_cmdSend;

	private Input inp_cmd;

	private Select_Ex sel_comm;

	private Label label2;

	private PictureBox pictureBox4;

	public event EventHandler<HappenEventArgs> OnSWSetFrmEvnet;

	public SoftwareSettingFrm()
	{
		InitializeComponent();
		((Component)this).Disposed += SoftwareSettingFrm_Disposed;
	}

	private void SoftwareSettingFrm_Load(object sender, EventArgs e)
	{
		InitSelect();
		((IControl)gridPanel1).Visible = !IsConsumerVersion();
		if (((IControl)gridPanel1).Visible)
		{
			InitCliControls();
		}
		((Control)label1).Text = Program.SoftwareVersion;
		ReloadLang(isReloadFont: true);
	}

	private static bool IsConsumerVersion()
	{
		return !string.IsNullOrWhiteSpace(Program.SoftwareVersion) && Program.SoftwareVersion.Trim().EndsWith("_C", StringComparison.OrdinalIgnoreCase);
	}

	private void Lang_OnLanguageChanged(object sender, EventArgs e)
	{
		ReloadLang(isReloadFont: true);
	}

	private void InitSelect()
	{
		List<Lang.LangInfo> availableLanguages = Lang.GetAvailableLanguages();
		sel_lang.Items.Clear();
		BaseCollection items = sel_lang.Items;
		object[] array = availableLanguages.Select((Lang.LangInfo x) => x.DisplayName).ToArray();
		items.AddRange(array);
		((Control)sel_lang).Text = Lang.CurrentName;
	}

	private void lab_language_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		InitSelect();
	}

	private void sel_cam1_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		List<Lang.LangInfo> availableLanguages = Lang.GetAvailableLanguages();
		if (((VEventArgs<int>)(object)e).Value >= 0 && ((VEventArgs<int>)(object)e).Value < availableLanguages.Count)
		{
			GD.Inst.CurrLang = Lang.SwitchLanguage(availableLanguages[((VEventArgs<int>)(object)e).Value].Code);
			ReloadLang(isReloadFont: true);
			HappenEventArgs e2 = new HappenEventArgs
			{
				eventType = EventType.frmSign,
				Index = GD.Inst.CurrLang
			};
			OnSWSetFrmEvnet?.Invoke(null, e2);
		}
	}

	private void lab_checkUpdate_Click(object sender, EventArgs e)
	{
	}

	private void label7_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UserAgreementFrm userAgreementFrm = new UserAgreementFrm();
		((Form)userAgreementFrm).ShowDialog();
	}

	public void ReloadFont()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		Font val = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 10f);
		Label obj = lab_lang;
		Label obj2 = lab_software;
		Label obj3 = label1;
		Font val2 = (((Control)lab_userAgree).Font = val);
		Font val4 = (((Control)obj3).Font = val2);
		Font font = (((Control)obj2).Font = val4);
		((Control)obj).Font = font;
	}

	public void FontChange(bool isAdd)
	{
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Expected O, but got Unknown
			Font font = ((Control)lab_lang).Font;
			FontFamily fontFamily = ((Control)lab_lang).Font.FontFamily;
			float size = font.Size;
			size = ((!isAdd) ? (size - 2f) : (size + 2f));
			Font val = new Font(fontFamily, size);
			Label obj = lab_lang;
			Label obj2 = lab_software;
			Label obj3 = label1;
			Font val2 = (((Control)lab_userAgree).Font = val);
			Font val4 = (((Control)obj3).Font = val2);
			Font font2 = (((Control)obj2).Font = val4);
			((Control)obj).Font = font2;
		});
	}

	public void ReloadLang(bool isReloadFont = false)
	{
		((Control)lab_lang).Text = Lang.T("swFrm_lab_lang");
		((Control)lab_software).Text = Lang.T("swFrm_lab_software");
		((Control)lab_userAgree).Text = Lang.T("swFrm_lab_userAgree");
		((Control)label2).Text = Lang.T("cli.title");
		inp_cmd.PlaceholderText = Lang.T("cli.input_cmd");
		((Input)sel_comm).PlaceholderText = Lang.T("cli.select_comm");
		((Input)sel_cmd).PlaceholderText = Lang.T("cli.select_cmd");
		((Control)btn_cmdSend).Text = Lang.T("cli.btn_send");
		RefreshCliCommandItems();
		if (isReloadFont)
		{
			ReloadFont();
		}
	}

	private async void btn_cmdSend_MouseClick(object sender, MouseEventArgs e)
	{
		if ((int)e.Button != 1048576 || _cliSending)
		{
			return;
		}
		string portName = ((Control)sel_comm).Text?.Trim();
		if (string.IsNullOrEmpty(portName))
		{
			ShowCliMessage(success: false, Lang.T("cli.select_port"));
			return;
		}
		try
		{
			byte[] payload = (from s in ((Control)inp_cmd).Text.Split(new char[2] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
				select Convert.ToByte(s, 16)).ToArray();
			string normalizedPort = SerialPortSessionText.NormalizePortName(portName);
			bool flag = GD.Inst.FindDeviceFrm != null;
			bool flag2 = flag;
			if (flag2)
			{
				flag2 = await GD.Inst.FindDeviceFrm.ReleaseDiscoverySessionByPortAsync(normalizedPort);
			}
			if (flag2)
			{
				lock (_cliSendLock)
				{
					_cliDiscoveryRestorePort = normalizedPort;
				}
			}
			await StartCliSendAsync(normalizedPort, payload);
			bool hasCliSession;
			lock (_cliSendLock)
			{
				hasCliSession = _cliSession != null;
			}
			if (!hasCliSession && !IsCliReleaseInProgress())
			{
				await RestoreDiscoverySessionIfNeededAsync(normalizedPort);
			}
		}
		catch (Exception)
		{
		}
	}

	private void sel_comm_SelectedIndexChanged(object sender, IntEventArgs e)
	{
	}

	private void sel_cmd_SelectedIndexChanged(object sender, IntEventArgs e)
	{
		if (((VEventArgs<int>)(object)e).Value == 0)
		{
			((Control)inp_cmd).Text = "41 53 57 00 01 14 01";
		}
		else if (((VEventArgs<int>)(object)e).Value == 1)
		{
			((Control)inp_cmd).Text = "41 53 57 00 02 14 01";
		}
		else if (((VEventArgs<int>)(object)e).Value == 2)
		{
			((Control)inp_cmd).Text = "41 53 57 00 02 14 00";
		}
	}

	private void InitCliControls()
	{
		RefreshCliCommandItems();
		((Control)btn_cmdSend).Text = Lang.T("cli.btn_send");
	}

	private void RefreshCliCommandItems()
	{
		((Select)sel_cmd).Items.Clear();
		BaseCollection items = ((Select)sel_cmd).Items;
		object[] array = new string[3]
		{
			Lang.T("cli.command.rf_offset"),
			Lang.T("cli.command.vtx_mac_enable"),
			Lang.T("cli.command.vtx_mac_disable")
		};
		items.AddRange(array);
	}

	private void sel_comm_MouseDown(object sender, MouseEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)e.Button == 1048576 && !_cliSending)
		{
			RefreshComPorts();
		}
	}

	private void RefreshComPorts()
	{
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			string previous = ((Control)sel_comm).Text?.Trim();
			string[] array = (from device in new UsbSerialMonitorDeviceEnumerator().GetCurrentDevices()
				where IsAscentVid(device.VID)
				select device.PortName into port
				where !string.IsNullOrWhiteSpace(port)
				select port).Distinct<string>(StringComparer.OrdinalIgnoreCase).OrderBy(GetComPortNumber).ThenBy<string, string>((string port) => port, StringComparer.OrdinalIgnoreCase)
				.ToArray();
			if (array.Length < 1)
			{
				CommModalFrm commModalFrm = new CommModalFrm();
				commModalFrm.SetAllTxt(Lang.T("common.title_error"), Lang.T("cli.no_valid_serial_port"), Color.Red);
				((Form)commModalFrm).ShowDialog();
				return;
			}
			((Select)sel_comm).Items.Clear();
			BaseCollection items = ((Select)sel_comm).Items;
			object[] array2 = array;
			items.AddRange(array2);
			int num = Array.FindIndex(array, (string port) => string.Equals(port, previous, StringComparison.OrdinalIgnoreCase));
			if (num >= 0)
			{
				((Select)sel_comm).SelectedIndex = num;
				((Control)sel_comm).Text = array[num];
			}
			else if (array.Length != 0)
			{
				((Select)sel_comm).SelectedIndex = 0;
				((Control)sel_comm).Text = array[0];
			}
			else
			{
				((Control)sel_comm).Text = string.Empty;
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI(Lang.T("cli.log.refresh_failed", ex.Message), Color.Red);
		}
	}

	private static bool IsAscentVid(string vid)
	{
		int result;
		return int.TryParse(vid, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result) && result >= 7541 && result <= 7544;
	}

	private static int GetComPortNumber(string portName)
	{
		if (!string.IsNullOrEmpty(portName) && portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase) && int.TryParse(portName.Substring(3), out var result))
		{
			return result;
		}
		return int.MaxValue;
	}

	private async Task StartCliSendAsync(string portName, byte[] payload)
	{
		lock (_cliSendLock)
		{
			_cliSending = true;
			_cliCompletionScheduled = false;
		}
		((IControl)btn_cmdSend).Enabled = false;
		try
		{
			UsbDevInfo device = FindCliDevice(portName);
			if (device == null)
			{
				CompleteCliSend(success: false, Lang.T("cli.device_not_found"));
				return;
			}
			SerialPortAcquireResult acquired = await GD.Inst.SerialPortSessions.AcquireAsync(device, SerialPortOwner.CliCommand, new SerialPortSessionOpenOptions
			{
				TransportKind = SerialPortTransportKind.LegacyFsm,
				BaudRate = 115200
			}, CancellationToken.None);
			if (!acquired.Succeeded)
			{
				CompleteCliSend(success: false, GetCliSessionFailureMessage(portName, acquired));
				return;
			}
			_cliSession = acquired.Handle;
			LegacyFsmSessionTransport legacy = default(LegacyFsmSessionTransport);
			int num;
			if (GD.Inst.SerialPortSessions.TryGetTransport(_cliSession, out var transport))
			{
				legacy = transport as LegacyFsmSessionTransport;
				if (legacy != null)
				{
					num = ((legacy.Fsm == null) ? 1 : 0);
					goto IL_021a;
				}
			}
			num = 1;
			goto IL_021a;
			IL_021a:
			if (num != 0)
			{
				CompleteCliSend(success: false, Lang.T("cli.session_access_failed"));
			}
			else if (!legacy.Fsm.SendWithAckGuard(119u, payload, (uint)payload.Length, "AR_COMMAND_SEND_TEXT ACK timeout", HandleCliMatchedAck, delegate
			{
				CompleteCliSend(success: false, Lang.T("cli.device_response_timeout"));
			}, delegate(string error)
			{
				CompleteCliSend(success: false, Lang.T("cli.send_failed") + " " + error);
			}, 119u, 1500, 10000, 3000, delegate(uint cmd, uint retry)
			{
				WriteLog.WriteLogFileToUI(Lang.T("cli.log.retry", retry, cmd), Color.Gray);
			}, null, null, requireMatchingSeq: true))
			{
				CompleteCliSend(success: false, Lang.T("cli.send_failed"));
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI(Lang.T("cli.log.send_exception", portName, ex.Message), Color.Red);
			CompleteCliSend(success: false, Lang.T("cli.send_failed"));
		}
	}

	private UsbDevInfo FindCliDevice(string portName)
	{
		string normalized = SerialPortSessionText.NormalizePortName(portName);
		IReadOnlyList<UsbDevInfo> currentDevices = new UsbSerialMonitorDeviceEnumerator().GetCurrentDevices();
		return currentDevices.FirstOrDefault((UsbDevInfo device) => string.Equals(SerialPortSessionText.NormalizePortName(device.PortName), normalized, StringComparison.OrdinalIgnoreCase));
	}

	private string GetCliSessionFailureMessage(string portName, SerialPortAcquireResult result)
	{
		if (result.FailureReason == SerialPortSessionFailureReason.Busy)
		{
			return Lang.T("cli.session_busy", portName, result.CurrentOwner, result.CurrentState);
		}
		return Lang.T("cli.open_failed");
	}

	private void HandleCliMatchedAck(uint cmd, object data, object data2)
	{
		ResAckInfo resAckInfo = data as ResAckInfo;
		string text = (data2 as string) ?? string.Empty;
		if (cmd != 119 || resAckInfo == null)
		{
			CompleteCliSend(success: false, Lang.T("cli.invalid_response"));
			return;
		}
		if (resAckInfo.Status == 0)
		{
			string text2 = Lang.T("cli.send_success");
			if (!string.IsNullOrWhiteSpace(text))
			{
				text2 += Lang.T("cli.detail_suffix", text);
			}
			CompleteCliSend(success: true, text2);
			return;
		}
		string text3 = Lang.T("cli.send_failed_status", resAckInfo.Status);
		if (!string.IsNullOrWhiteSpace(text))
		{
			text3 += Lang.T("cli.detail_suffix", text);
		}
		CompleteCliSend(success: false, text3);
	}

	private void CompleteCliSend(bool success, string message)
	{
		lock (_cliSendLock)
		{
			if (!_cliSending || _cliCompletionScheduled)
			{
				return;
			}
			_cliCompletionScheduled = true;
		}
		Action action = delegate
		{
			CompleteCliSendOnUi(success, message);
		};
		if (((Control)this).IsDisposed || ((Control)this).Disposing || !((Control)this).IsHandleCreated)
		{
			ReleaseCliFsm(disposeAsync: true);
			return;
		}
		try
		{
			if (((Control)this).InvokeRequired)
			{
				((Control)this).BeginInvoke((Delegate)action);
			}
			else
			{
				action();
			}
		}
		catch (InvalidOperationException)
		{
			ReleaseCliFsm(disposeAsync: true);
		}
	}

	private void CompleteCliSendOnUi(bool success, string message)
	{
		bool flag;
		lock (_cliSendLock)
		{
			flag = _cliSession != null;
		}
		if (!flag && !IsCliReleaseInProgress())
		{
			RestoreDiscoverySessionIfNeededAsync(_cliDiscoveryRestorePort);
		}
		ReleaseCliFsm();
		if (!((Control)this).IsDisposed && !((Control)this).Disposing)
		{
			((IControl)btn_cmdSend).Enabled = true;
			ShowCliMessage(success, message);
		}
	}

	private void ReleaseCliFsm(bool disposeAsync = false)
	{
		SerialPortSessionHandle cliSession;
		lock (_cliSendLock)
		{
			cliSession = _cliSession;
			_cliSession = null;
			_cliSending = false;
			_cliCompletionScheduled = false;
			if (cliSession != null)
			{
				_cliReleaseInProgress = true;
			}
		}
		if (cliSession != null)
		{
			ReleaseCliSessionAsync(cliSession, SerialPortReleaseReason.CommandCompleted);
		}
	}

	private async Task ReleaseCliSessionAsync(SerialPortSessionHandle session, SerialPortReleaseReason reason)
	{
		try
		{
			SerialPortReleaseResult result = await GD.Inst.SerialPortSessions.ReleaseAsync(session, reason);
			if (result.Succeeded)
			{
				await RestoreDiscoverySessionIfNeededAsync(session.PortName);
			}
			else if (result.FailureReason != SerialPortSessionFailureReason.StaleHandle)
			{
				WriteLog.WriteLogFileToUI(Lang.T("cli.log.release_failed", session.PortName, result.FailureReason, result.CurrentOwner, result.CurrentState), Color.DarkOrange);
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			WriteLog.WriteLogFileToUI(Lang.T("cli.log.release_exception", session.PortName, ex2.Message), Color.Red);
		}
		finally
		{
			lock (_cliSendLock)
			{
				_cliReleaseInProgress = false;
			}
		}
	}

	private async Task RestoreDiscoverySessionIfNeededAsync(string portName)
	{
		string normalized = SerialPortSessionText.NormalizePortName(portName);
		string restorePort = null;
		lock (_cliSendLock)
		{
			if (_cliDiscoveryRestorePort != null && string.Equals(_cliDiscoveryRestorePort, normalized, StringComparison.OrdinalIgnoreCase))
			{
				restorePort = _cliDiscoveryRestorePort;
				_cliDiscoveryRestorePort = null;
			}
		}
		if (restorePort == null)
		{
			return;
		}
		try
		{
			FindDeviceFrm findDeviceFrm = GD.Inst.FindDeviceFrm;
			if (findDeviceFrm != null && !((Control)findDeviceFrm).Disposing && !((Control)findDeviceFrm).IsDisposed)
			{
				await findDeviceFrm.RestoreDiscoverySessionByPortAsync(restorePort);
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI(Lang.T("cli.log.restore_exception", restorePort, ex.Message), Color.Red);
		}
	}

	private bool IsCliReleaseInProgress()
	{
		lock (_cliSendLock)
		{
			return _cliReleaseInProgress;
		}
	}

	private void SoftwareSettingFrm_Disposed(object sender, EventArgs e)
	{
		ReleaseCliFsm();
	}

	public void ReleaseCliSession()
	{
		ReleaseCliFsm();
	}

	private void ShowCliMessage(bool success, string message)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		using CommModalFrm commModalFrm = new CommModalFrm();
		commModalFrm.SetAllTxt(success ? Lang.T("common.title_success") : Lang.T("common.title_warning"), message, success ? Color.Green : Color.Red);
		((Form)commModalFrm).ShowDialog();
	}

	private void btn_cmdSend_Click(object sender, EventArgs e)
	{
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
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Expected O, but got Unknown
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Expected O, but got Unknown
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Expected O, but got Unknown
		//IL_077c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Expected O, but got Unknown
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dc: Expected O, but got Unknown
		//IL_0a8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f00: Expected O, but got Unknown
		//IL_0f5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_106e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1263: Unknown result type (might be due to invalid IL or missing references)
		//IL_126d: Expected O, but got Unknown
		//IL_1292: Unknown result type (might be due to invalid IL or missing references)
		//IL_129c: Expected O, but got Unknown
		//IL_12e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1350: Unknown result type (might be due to invalid IL or missing references)
		//IL_135a: Expected O, but got Unknown
		//IL_1379: Unknown result type (might be due to invalid IL or missing references)
		//IL_1383: Expected O, but got Unknown
		//IL_13b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1466: Unknown result type (might be due to invalid IL or missing references)
		//IL_1470: Expected O, but got Unknown
		//IL_1473: Unknown result type (might be due to invalid IL or missing references)
		//IL_148d: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SoftwareSettingFrm));
		button1 = new Button();
		grPan_Main = new GridPanel();
		gridPanel1 = new GridPanel();
		btn_cmdSend = new Button();
		inp_cmd = new Input();
		sel_comm = new Select_Ex();
		label2 = new Label();
		pictureBox4 = new PictureBox();
		sel_cmd = new Select_Ex();
		gridPanel6 = new GridPanel();
		lab_userAgree = new Label();
		pictureBox1 = new PictureBox();
		gridPanel5 = new GridPanel();
		lab_software = new Label();
		pictureBox3 = new PictureBox();
		label1 = new Label();
		gridPanel4 = new GridPanel();
		sel_lang = new Select();
		lab_lang = new Label();
		pictureBox2 = new PictureBox();
		((Control)grPan_Main).SuspendLayout();
		((Control)gridPanel1).SuspendLayout();
		((ISupportInitialize)pictureBox4).BeginInit();
		((Control)gridPanel6).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)gridPanel5).SuspendLayout();
		((ISupportInitialize)pictureBox3).BeginInit();
		((Control)gridPanel4).SuspendLayout();
		((ISupportInitialize)pictureBox2).BeginInit();
		((Control)this).SuspendLayout();
		button1.BackHover = Color.Transparent;
		button1.DefaultBack = Color.Transparent;
		button1.DisplayStyle = (TButtonDisplayStyle)1;
		((Control)button1).Font = new Font("Microsoft Sans Serif", 15.75f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		button1.ForeColor = Color.White;
		button1.Ghost = true;
		((IControl)button1).HandDragFolder = false;
		gridPanel6.SetIndex((Control)(object)button1, 3);
		((Control)button1).Location = new Point(239, 5);
		((Control)button1).Margin = new Padding(5, 5, 20, 5);
		((Control)button1).Name = "button1";
		((Control)button1).Size = new Size(521, 39);
		((Control)button1).TabIndex = 2;
		((Control)button1).Text = " >";
		button1.TextAlign = (ContentAlignment)64;
		button1.WaveSize = 0;
		((Control)button1).MouseClick += new MouseEventHandler(label7_MouseClick);
		((ContainerPanel)grPan_Main).Back = Color.Transparent;
		((Control)grPan_Main).BackColor = Color.Transparent;
		((Control)grPan_Main).Controls.Add((Control)(object)gridPanel1);
		((Control)grPan_Main).Controls.Add((Control)(object)sel_cmd);
		((Control)grPan_Main).Controls.Add((Control)(object)gridPanel6);
		((Control)grPan_Main).Controls.Add((Control)(object)gridPanel5);
		((Control)grPan_Main).Controls.Add((Control)(object)gridPanel4);
		((Control)grPan_Main).Dock = (DockStyle)5;
		((IControl)grPan_Main).HandCursor = Cursors.Default;
		((Control)grPan_Main).Location = new Point(25, 25);
		((Control)grPan_Main).Name = "grPan_Main";
		((Control)grPan_Main).Size = new Size(790, 590);
		grPan_Main.Span = "100%;100%;100%;\r\n100%;100%;100%;\r\n100%;100%;100%;\r\n-10% 10% 10% 10% 10% 10% 10% 10% 20%";
		((Control)grPan_Main).TabIndex = 9;
		((Control)grPan_Main).Text = "gridPanel1";
		((ContainerPanel)gridPanel1).BorderColor = Color.FromArgb(57, 60, 61);
		((ContainerPanel)gridPanel1).BorderWidth = 2f;
		((Control)gridPanel1).Controls.Add((Control)(object)btn_cmdSend);
		((Control)gridPanel1).Controls.Add((Control)(object)inp_cmd);
		((Control)gridPanel1).Controls.Add((Control)(object)sel_comm);
		((Control)gridPanel1).Controls.Add((Control)(object)label2);
		((Control)gridPanel1).Controls.Add((Control)(object)pictureBox4);
		((IControl)gridPanel1).HandCursor = Cursors.Default;
		grPan_Main.SetIndex((Control)(object)gridPanel1, 3);
		((Control)gridPanel1).Location = new Point(5, 182);
		((Control)gridPanel1).Margin = new Padding(5);
		((Control)gridPanel1).Name = "gridPanel1";
		((ContainerPanel)gridPanel1).Radius = 6;
		((Control)gridPanel1).Size = new Size(780, 49);
		gridPanel1.Span = "10% 10% 20% 50% 10%;";
		((Control)gridPanel1).TabIndex = 20;
		((Control)gridPanel1).Text = "gridPanel1";
		((IControl)btn_cmdSend).ColorScheme = (TAMode)2;
		btn_cmdSend.DefaultBack = Color.FromArgb(255, 233, 0);
		btn_cmdSend.ForeColor = Color.FromArgb(35, 35, 35);
		((Control)btn_cmdSend).Location = new Point(705, 3);
		((Control)btn_cmdSend).Name = "btn_cmdSend";
		((Control)btn_cmdSend).Size = new Size(72, 43);
		((Control)btn_cmdSend).TabIndex = 20;
		((Control)btn_cmdSend).Text = "send";
		((Control)btn_cmdSend).MouseClick += new MouseEventHandler(btn_cmdSend_MouseClick);
		inp_cmd.BorderActive = Color.FromArgb(255, 233, 0);
		inp_cmd.BorderHover = Color.FromArgb(255, 233, 0);
		((IControl)inp_cmd).ColorScheme = (TAMode)2;
		((Control)inp_cmd).Location = new Point(315, 3);
		((Control)inp_cmd).Name = "inp_cmd";
		inp_cmd.PlaceholderText = "input cmd";
		((Control)inp_cmd).Size = new Size(384, 43);
		((Control)inp_cmd).TabIndex = 18;
		((Control)inp_cmd).TabStop = false;
		((Input)sel_comm).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_comm).BorderHover = Color.FromArgb(255, 233, 0);
		((IControl)sel_comm).ColorScheme = (TAMode)2;
		((IControl)sel_comm).HandDragFolder = false;
		((Control)sel_comm).Location = new Point(159, 3);
		((Control)sel_comm).Name = "sel_comm";
		((Input)sel_comm).PlaceholderText = "select comm";
		((Control)sel_comm).Size = new Size(150, 43);
		((Control)sel_comm).TabIndex = 17;
		((Control)sel_comm).TabStop = false;
		((Input)sel_comm).WaveSize = 0;
		((Control)sel_comm).MouseDown += new MouseEventHandler(sel_comm_MouseDown);
		((Control)label2).AutoSize = true;
		((Control)label2).ForeColor = Color.White;
		gridPanel1.SetIndex((Control)(object)label2, 2);
		((Control)label2).Location = new Point(78, 10);
		((Control)label2).Margin = new Padding(0, 10, 0, 10);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(78, 29);
		((Control)label2).TabIndex = 14;
		((Control)label2).Text = "CLI";
		label2.TextAlign = (ContentAlignment)16;
		((Control)pictureBox4).Dock = (DockStyle)5;
		pictureBox4.Image = (Image)(object)Resources.CLI_Icon;
		gridPanel1.SetIndex((Control)(object)pictureBox4, 1);
		((Control)pictureBox4).Location = new Point(20, 10);
		((Control)pictureBox4).Margin = new Padding(20, 10, 0, 10);
		((Control)pictureBox4).Name = "pictureBox4";
		((Control)pictureBox4).Size = new Size(58, 29);
		pictureBox4.SizeMode = (PictureBoxSizeMode)4;
		pictureBox4.TabIndex = 10;
		pictureBox4.TabStop = false;
		((Input)sel_cmd).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_cmd).BorderHover = Color.FromArgb(255, 233, 0);
		((IControl)sel_cmd).ColorScheme = (TAMode)2;
		((IControl)sel_cmd).HandDragFolder = false;
		((Control)sel_cmd).Location = new Point(3, 239);
		((Control)sel_cmd).Name = "sel_cmd";
		((Input)sel_cmd).PlaceholderText = "select cmd";
		((Control)sel_cmd).Size = new Size(784, 53);
		((Control)sel_cmd).TabIndex = 19;
		((Control)sel_cmd).TabStop = false;
		((IControl)sel_cmd).Visible = false;
		((Input)sel_cmd).WaveSize = 0;
		((Select)sel_cmd).SelectedIndexChanged += new IntEventHandler(sel_cmd_SelectedIndexChanged);
		((ContainerPanel)gridPanel6).BorderColor = Color.FromArgb(57, 60, 61);
		((ContainerPanel)gridPanel6).BorderWidth = 2f;
		((Control)gridPanel6).Controls.Add((Control)(object)lab_userAgree);
		((Control)gridPanel6).Controls.Add((Control)(object)pictureBox1);
		((Control)gridPanel6).Controls.Add((Control)(object)button1);
		((IControl)gridPanel6).HandCursor = Cursors.Default;
		grPan_Main.SetIndex((Control)(object)gridPanel6, 3);
		((Control)gridPanel6).Location = new Point(5, 123);
		((Control)gridPanel6).Margin = new Padding(5);
		((Control)gridPanel6).Name = "gridPanel6";
		((ContainerPanel)gridPanel6).Radius = 6;
		((Control)gridPanel6).Size = new Size(780, 49);
		gridPanel6.Span = "10% 20% 70%;";
		((Control)gridPanel6).TabIndex = 13;
		((Control)gridPanel6).Text = "gridPanel6";
		((Control)lab_userAgree).AutoSize = true;
		((Control)lab_userAgree).ForeColor = Color.White;
		gridPanel6.SetIndex((Control)(object)lab_userAgree, 2);
		((Control)lab_userAgree).Location = new Point(78, 10);
		((Control)lab_userAgree).Margin = new Padding(0, 10, 0, 10);
		((Control)lab_userAgree).Name = "lab_userAgree";
		((Control)lab_userAgree).Size = new Size(156, 29);
		((Control)lab_userAgree).TabIndex = 14;
		((Control)lab_userAgree).Text = "User Agreement";
		lab_userAgree.TextAlign = (ContentAlignment)16;
		((Control)pictureBox1).Dock = (DockStyle)5;
		pictureBox1.Image = (Image)(object)Resources.用户协议1;
		gridPanel6.SetIndex((Control)(object)pictureBox1, 1);
		((Control)pictureBox1).Location = new Point(20, 10);
		((Control)pictureBox1).Margin = new Padding(20, 10, 0, 10);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(58, 29);
		pictureBox1.SizeMode = (PictureBoxSizeMode)4;
		pictureBox1.TabIndex = 10;
		pictureBox1.TabStop = false;
		((ContainerPanel)gridPanel5).BorderColor = Color.FromArgb(57, 60, 61);
		((ContainerPanel)gridPanel5).BorderWidth = 2f;
		((Control)gridPanel5).Controls.Add((Control)(object)lab_software);
		((Control)gridPanel5).Controls.Add((Control)(object)pictureBox3);
		((Control)gridPanel5).Controls.Add((Control)(object)label1);
		((IControl)gridPanel5).HandCursor = Cursors.Default;
		grPan_Main.SetIndex((Control)(object)gridPanel5, 3);
		((Control)gridPanel5).Location = new Point(5, 64);
		((Control)gridPanel5).Margin = new Padding(5);
		((Control)gridPanel5).Name = "gridPanel5";
		((ContainerPanel)gridPanel5).Radius = 6;
		((Control)gridPanel5).Size = new Size(780, 49);
		gridPanel5.Span = "10% 20% 70%;";
		((Control)gridPanel5).TabIndex = 12;
		((Control)gridPanel5).Text = "gridPanel5";
		((Control)lab_software).AutoSize = true;
		((Control)lab_software).ForeColor = Color.White;
		gridPanel5.SetIndex((Control)(object)lab_software, 2);
		((Control)lab_software).Location = new Point(78, 10);
		((Control)lab_software).Margin = new Padding(0, 10, 0, 10);
		((Control)lab_software).Name = "lab_software";
		((Control)lab_software).Size = new Size(156, 29);
		((Control)lab_software).TabIndex = 15;
		((Control)lab_software).Text = "Software version";
		lab_software.TextAlign = (ContentAlignment)16;
		((Control)pictureBox3).Dock = (DockStyle)5;
		pictureBox3.Image = (Image)(object)Resources.软件版本1;
		gridPanel5.SetIndex((Control)(object)pictureBox3, 1);
		((Control)pictureBox3).Location = new Point(20, 10);
		((Control)pictureBox3).Margin = new Padding(20, 10, 0, 10);
		((Control)pictureBox3).Name = "pictureBox3";
		((Control)pictureBox3).Size = new Size(58, 29);
		pictureBox3.SizeMode = (PictureBoxSizeMode)4;
		pictureBox3.TabIndex = 14;
		pictureBox3.TabStop = false;
		((Control)label1).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		label1.ForeColor = Color.White;
		((IControl)label1).HandCursor = Cursors.Default;
		gridPanel5.SetIndex((Control)(object)label1, 3);
		((Control)label1).Location = new Point(239, 5);
		((Control)label1).Margin = new Padding(5, 5, 20, 5);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(521, 39);
		((Control)label1).TabIndex = 13;
		((Control)label1).Text = "V1.0.8   ";
		label1.TextAlign = (ContentAlignment)64;
		((ContainerPanel)gridPanel4).BorderColor = Color.FromArgb(57, 60, 61);
		((ContainerPanel)gridPanel4).BorderWidth = 2f;
		((Control)gridPanel4).Controls.Add((Control)(object)sel_lang);
		((Control)gridPanel4).Controls.Add((Control)(object)lab_lang);
		((Control)gridPanel4).Controls.Add((Control)(object)pictureBox2);
		((IControl)gridPanel4).HandCursor = Cursors.Default;
		grPan_Main.SetIndex((Control)(object)gridPanel4, 3);
		((Control)gridPanel4).Location = new Point(5, 5);
		((Control)gridPanel4).Margin = new Padding(5);
		((Control)gridPanel4).Name = "gridPanel4";
		((ContainerPanel)gridPanel4).Radius = 6;
		((Control)gridPanel4).Size = new Size(780, 49);
		gridPanel4.Span = "10% 50% 40%;";
		((Control)gridPanel4).TabIndex = 11;
		((Control)gridPanel4).Text = "gridPanel4";
		((Input)sel_lang).BackColor = Color.FromArgb(38, 41, 43);
		((Input)sel_lang).BorderActive = Color.FromArgb(255, 233, 0);
		((Input)sel_lang).BorderColor = Color.FromArgb(0, 0, 0, 0);
		((Input)sel_lang).BorderHover = Color.FromArgb(255, 233, 0);
		((Input)sel_lang).BorderWidth = 2f;
		((IControl)sel_lang).ColorScheme = (TAMode)2;
		sel_lang.EnterDropDown = false;
		((Input)sel_lang).ForeColor = Color.White;
		((IControl)sel_lang).HandDragFolder = false;
		sel_lang.List = true;
		((Control)sel_lang).Location = new Point(471, 3);
		((Control)sel_lang).Name = "sel_lang";
		((Control)sel_lang).RightToLeft = (RightToLeft)0;
		((Input)sel_lang).SelectionColor = Color.Empty;
		((Control)sel_lang).Size = new Size(306, 43);
		((Control)sel_lang).TabIndex = 21;
		((Control)sel_lang).TabStop = false;
		((Control)sel_lang).Text = "none";
		((Input)sel_lang).TextAlign = (HorizontalAlignment)1;
		((Input)sel_lang).WaveSize = 0;
		sel_lang.SelectedIndexChanged += new IntEventHandler(sel_cam1_SelectedIndexChanged);
		((Control)lab_lang).AutoSize = true;
		((Control)lab_lang).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_lang).ForeColor = Color.White;
		gridPanel4.SetIndex((Control)(object)lab_lang, 2);
		((Control)lab_lang).Location = new Point(78, 10);
		((Control)lab_lang).Margin = new Padding(0, 10, 0, 10);
		((Control)lab_lang).Name = "lab_lang";
		((Control)lab_lang).Size = new Size(390, 29);
		((Control)lab_lang).TabIndex = 15;
		((Control)lab_lang).Text = "语言选择/Language";
		lab_lang.TextAlign = (ContentAlignment)16;
		((Control)lab_lang).MouseDoubleClick += new MouseEventHandler(lab_language_MouseDoubleClick);
		((Control)pictureBox2).Dock = (DockStyle)5;
		pictureBox2.Image = (Image)componentResourceManager.GetObject("pictureBox2.Image");
		gridPanel4.SetIndex((Control)(object)pictureBox2, 1);
		((Control)pictureBox2).Location = new Point(20, 10);
		((Control)pictureBox2).Margin = new Padding(20, 10, 0, 10);
		((Control)pictureBox2).Name = "pictureBox2";
		((Control)pictureBox2).Size = new Size(58, 29);
		pictureBox2.SizeMode = (PictureBoxSizeMode)4;
		pictureBox2.TabIndex = 14;
		pictureBox2.TabStop = false;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(96f, 96f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)2;
		((Control)this).BackColor = Color.FromArgb(38, 41, 43);
		((Control)this).Controls.Add((Control)(object)grPan_Main);
		((Control)this).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)this).Margin = new Padding(5);
		((Control)this).Name = "SoftwareSettingFrm";
		((Control)this).Padding = new Padding(25);
		((Control)this).Size = new Size(840, 640);
		((UserControl)this).Load += SoftwareSettingFrm_Load;
		((Control)grPan_Main).ResumeLayout(false);
		((Control)gridPanel1).ResumeLayout(false);
		((Control)gridPanel1).PerformLayout();
		((ISupportInitialize)pictureBox4).EndInit();
		((Control)gridPanel6).ResumeLayout(false);
		((Control)gridPanel6).PerformLayout();
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)gridPanel5).ResumeLayout(false);
		((Control)gridPanel5).PerformLayout();
		((ISupportInitialize)pictureBox3).EndInit();
		((Control)gridPanel4).ResumeLayout(false);
		((Control)gridPanel4).PerformLayout();
		((ISupportInitialize)pictureBox2).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
