using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntdUI_Ex;
using Caddx_PCTool.Properties;
using Caddx_PCTool.Update;

namespace Caddx_PCTool;

public class SidebarCtrl : UserControl, IDisposable
{
	private string _className = "SidebarCtrl";

	private List<PictureBox> picList;

	private List<Label> labList;

	private List<Panel> panList;

	private List<GridPanel> grPanList;

	private List<Label> helpBtnList;

	private List<Panel> helpPanList;

	private float btnFontSize = 12f;

	private float helpBtnFontSize = 10f;

	private ushort _rightBtnClickCount = 0;

	private CancellationTokenSource _clearClickCount;

	private bool _isCheckingApplicationUpdate;

	private IContainer components = null;

	private PictureBox pic_firmware;

	private Label lab_prodSer;

	private Label lab_customerService;

	private Label lab_firmware;

	private Label lab_setting;

	private PictureBox pic_setting;

	private Label lab_HelpCenter;

	private PictureBox pic_HelpCenter;

	private GridPanel grpan_use;

	private Panel pan_firmware;

	private GridPanel grpan_firmware;

	private Panel pan_setting;

	private GridPanel grpan_setting;

	private Panel pan_HelpCenter;

	private GridPanel grpan_HelpCenter;

	private GridPanel grpan_main;

	private Label label1;

	private Panel pan_prodSer;

	private Panel pan_CamHub;

	private GridPanel grpan_CamHub;

	private PictureBox pic_CamHub;

	private Label lab_CamHub;

	private Panel pan_customerService;

	private Panel pan_RCMode;

	private Label lab_RCMode;

	private PictureBox pic_RCMode;

	private GridPanel grpan_RCMode;

	private Panel panel1;

	private Label lab_vers;

	private Panel pan_Channel;

	private GridPanel grPan_Channel;

	private PictureBox pic_Channel;

	private Label lab_Channel;

	public event EventHandler<SidebarFrmEventArgs> OnSidebarFrmEvnet;

	public SidebarCtrl()
	{
		InitializeComponent();
	}

	private void Lang_OnLanguageChanged(object sender, EventArgs e)
	{
		ReloadLang(isReloadFont: true);
	}

	private void SidebarCtrl_Load(object sender, EventArgs e)
	{
		picList = new List<PictureBox> { pic_firmware, pic_setting, pic_HelpCenter, pic_CamHub, pic_RCMode, pic_Channel };
		labList = new List<Label> { lab_firmware, lab_setting, lab_HelpCenter, lab_CamHub, lab_RCMode, lab_Channel };
		panList = new List<Panel> { pan_firmware, pan_setting, pan_HelpCenter, pan_CamHub, pan_RCMode, pan_Channel };
		grPanList = new List<GridPanel> { grpan_firmware, grpan_setting, grpan_HelpCenter, grpan_CamHub, grpan_RCMode, grPan_Channel };
		helpBtnList = new List<Label> { lab_prodSer, lab_customerService };
		helpPanList = new List<Panel> { pan_prodSer, pan_customerService };
		Label obj = lab_prodSer;
		bool visible = (((Control)lab_customerService).Visible = false);
		((Control)obj).Visible = visible;
		((Control)label1).BackColor = Color.FromArgb(1776670);
		ReloadLang();
		ReLoadFont();
	}

	private async Task ClearRightBtnClickCount(CancellationToken cts)
	{
		while (!cts.IsCancellationRequested)
		{
			try
			{
				WriteLog.WriteLogFileToUI($"ClearRightBtnClickCount, current count={_rightBtnClickCount}", Color.Black);
				_rightBtnClickCount = 0;
				await Task.Delay(10000, cts);
			}
			catch (OperationCanceledException)
			{
				break;
			}
		}
	}

	private async void lab_vers_MouseClick(object sender, MouseEventArgs e)
	{
		if ((int)e.Button != 2097152 || _isCheckingApplicationUpdate)
		{
			return;
		}
		Form val = ((Control)this).FindForm();
		if (!(val is MainFrm main) || !main.IsFindDeviceActive)
		{
			ShowUpdateMessage("sidebar.update.device_required", false, null, true, false);
			return;
		}
		new CommModalFrm();
		_isCheckingApplicationUpdate = true;
		try
		{
			VariantUpdateConfig config = VariantUpdateConfig.FromAppConfig(ConfigurationManager.AppSettings);
			VelopackApplicationUpdater updater = new VelopackApplicationUpdater(config, delegate(string message)
			{
				WriteLog.WriteLogFileToUI(message, Color.Gray);
			});
			if (!updater.IsInstalled)
			{
				ShowUpdateMessage("sidebar.update.not_installed", false, null, true, false);
				return;
			}
			ApplicationUpdateInfo update;
			using (CommModalFrm searchDialog = new CommModalFrm())
			{
				string title = Lang.T("sidebar.update.title");
				string description = Lang.T("sidebar.update.searching");
				searchDialog.SetAllTxt(title, description, isshowBtnOK: true, isshowBtnCan: false);
				((Control)searchDialog).Show();
				WriteLog.WriteLogFileToUI("开始联网搜索软件更新", Color.Gray);
				update = await updater.CheckForUpdatesAsync(CancellationToken.None);
			}
			if (update == null)
			{
				string currentVersion = (string.IsNullOrWhiteSpace(updater.CurrentVersion) ? Program.SoftwareVersion : updater.CurrentVersion);
				WriteLog.WriteLogFileToUI("当前已是最新版本：" + currentVersion, Color.Gray);
				ShowUpdateMessage("sidebar.update.latest", false, null, true, false, currentVersion);
			}
			else
			{
				WriteLog.WriteLogFileToUI("发现新版本：" + update.TargetVersion, Color.Gray);
				if (!ConfirmApplicationUpdate(update))
				{
					WriteLog.WriteLogFileToUI("已取消软件更新", Color.Gray);
				}
				else
				{
					await ApplyApplicationUpdateAsync(updater, update);
				}
			}
		}
		catch (UpdateServiceException ex)
		{
			UpdateServiceException ex2 = ex;
			WriteLog.WriteLogFileToUI($"检查软件更新失败：{ex2.Kind}", Color.Red);
			ShowApplicationUpdateError(ex2.Kind);
		}
		catch (Exception ex3)
		{
			Exception ex4 = ex3;
			WriteLog.WriteLogFileToUI("检查软件更新失败，desc=" + ex4.Message, Color.Red);
			ShowApplicationUpdateMessage("sidebar.update.error_generic", true, null);
		}
		finally
		{
			_isCheckingApplicationUpdate = false;
		}
	}

	private void ShowApplicationUpdateError(UpdateFailureKind kind, bool isApply = false)
	{
		string messageKey;
		switch (kind)
		{
		case UpdateFailureKind.NotInstalled:
			messageKey = "sidebar.update.error_not_installed";
			break;
		case UpdateFailureKind.NotConfigured:
			messageKey = "sidebar.update.error_not_configured";
			break;
		case UpdateFailureKind.Cancelled:
			messageKey = "sidebar.update.error_cancelled";
			break;
		case UpdateFailureKind.RateLimited:
		case UpdateFailureKind.Forbidden:
			messageKey = "sidebar.update.error_unavailable";
			break;
		case UpdateFailureKind.Network:
		case UpdateFailureKind.Server:
			messageKey = "sidebar.update.error_network";
			break;
		default:
			messageKey = "sidebar.update.error_generic";
			break;
		}
		ShowApplicationUpdateMessage(messageKey, true, isApply ? "sidebar.update.title_failed" : null);
	}

	private void ShowApplicationUpdateMessage(string messageKey, bool isError = false, string titleKey = null, params object[] args)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		using CommModalFrm commModalFrm = new CommModalFrm();
		string title = Lang.T(titleKey ?? (isError ? "sidebar.update.title_check_failed" : "sidebar.update.title"));
		string desc = ((args == null || args.Length == 0) ? Lang.T(messageKey) : Lang.T(messageKey, args));
		if (isError)
		{
			commModalFrm.SetAllTxt(title, desc, Color.Red);
		}
		else
		{
			commModalFrm.SetAllTxt(title, desc);
		}
		((Form)commModalFrm).ShowDialog();
	}

	private void ShowUpdateMessage(string messageKey, bool isError = false, string titleKey = null, bool isShowOK = true, bool isShowCanel = false, params object[] args)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		using CommModalFrm commModalFrm = new CommModalFrm();
		string title = Lang.T(titleKey ?? (isError ? "sidebar.update.title_check_failed" : "sidebar.update.title"));
		string desc = ((args == null || args.Length == 0) ? Lang.T(messageKey) : Lang.T(messageKey, args));
		if (isError)
		{
			commModalFrm.SetAllTxt(title, desc, Color.Red);
		}
		else
		{
			commModalFrm.SetAllTxt(title, desc, isShowOK, isShowCanel);
		}
		((Form)commModalFrm).ShowDialog();
	}

	private bool ConfirmApplicationUpdate(ApplicationUpdateInfo update)
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Invalid comparison between Unknown and I4
		using CommModalFrm commModalFrm = new CommModalFrm();
		string title = Lang.T("sidebar.update.title");
		string text = Lang.T("sidebar.update.confirm").Replace("\r\n", "\n");
		string[] array = text.Split(new char[1] { '\n' });
		List<string> list = new List<string>();
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			if (text2.IndexOf("{1}", StringComparison.Ordinal) < 0)
			{
				list.Add(text2);
			}
		}
		string desc = string.Format(string.Join(Environment.NewLine, list), update.TargetVersion);
		commModalFrm.SetAllTxt(title, desc);
		commModalFrm.SetReleaseNotes(update.ReleaseNotes);
		((Form)commModalFrm).ShowDialog();
		return (int)commModalFrm.FrmResult == 1;
	}

	private async Task ApplyApplicationUpdateAsync(IApplicationUpdater updater, ApplicationUpdateInfo update)
	{
		CommModalFrm progressDialog = new CommModalFrm();
		try
		{
			string title = Lang.T("sidebar.update.title");
			string description = Lang.T("sidebar.update.downloading");
			progressDialog.SetAllTxt(title, description, isshowBtnOK: false, isshowBtnCan: false);
			progressDialog.ShowProgressBar();
			((Control)progressDialog).Show();
			WriteLog.WriteLogFileToUI("用户确认更新，开始下载更新包", Color.Gray);
			int lastProgress = -1;
			Progress<int> progress = new Progress<int>(delegate(int p)
			{
				progressDialog.SetProgress(p);
				if (p != lastProgress)
				{
					lastProgress = p;
					WriteLog.WriteLogFileToUI($"更新下载进度：{p}%", Color.Gray);
				}
			});
			await updater.DownloadUpdatesAsync(update, progress, CancellationToken.None);
			progressDialog.SetProgress(100);
			WriteLog.WriteLogFileToUI("更新下载完成", Color.Green);
			((Form)progressDialog).Close();
			if (!ConfirmRestartApplication())
			{
				WriteLog.WriteLogFileToUI("用户暂不重启，更新已下载完成", Color.Gray);
				return;
			}
			WriteLog.WriteLogFileToUI("准备重启并安装更新", Color.Gray);
			updater.WaitExitThenApplyUpdates(update);
		}
		catch (UpdateServiceException ex)
		{
			UpdateServiceException ex2 = ex;
			if (((Control)progressDialog).Visible)
			{
				((Form)progressDialog).Close();
			}
			WriteLog.WriteLogFileToUI($"软件更新失败：{ex2.Kind}", Color.Red);
			ShowApplicationUpdateError(ex2.Kind, isApply: true);
		}
		catch (Exception ex3)
		{
			Exception ex4 = ex3;
			if (((Control)progressDialog).Visible)
			{
				((Form)progressDialog).Close();
			}
			WriteLog.WriteLogFileToUI("软件更新失败，desc=" + ex4.Message, Color.Red);
			ShowApplicationUpdateMessage("sidebar.update.failed", true, "sidebar.update.title_failed");
		}
		finally
		{
			if (((Control)progressDialog).Visible)
			{
				((Form)progressDialog).Close();
			}
			progressDialog.Dispose();
		}
	}

	private bool ConfirmRestartApplication()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Invalid comparison between Unknown and I4
		using CommModalFrm commModalFrm = new CommModalFrm();
		string title = Lang.T("sidebar.update.title");
		string desc = Lang.T("sidebar.update.restart");
		commModalFrm.SetAllTxt(title, desc, isshowBtnOK: true, isshowBtnCan: false);
		((Form)commModalFrm).ShowDialog();
		return (int)commModalFrm.FrmResult == 1;
	}

	private void lab_Channel_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		RefreshButtonStyle();
		((Control)lab_Channel).ForeColor = Color.FromArgb(35, 35, 35);
		pan_Channel.Back = Color.FromArgb(255, 233, 0);
		pic_Channel.Image = (Image)(object)Resources.wifi_selected;
		Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], btnFontSize);
		((Control)lab_Channel).Font = font;
		SidebarFrmEventArgs e2 = new SidebarFrmEventArgs
		{
			Text = ((Control)lab_Channel).Text,
			Name = ((Control)lab_Channel).Name
		};
		OnSidebarFrmEvnet?.Invoke(null, e2);
	}

	private void CamHub_MouseClick(object sender, MouseEventArgs e)
	{
		RefreshButtonStyle();
		SidebarFrmEventArgs se = new SidebarFrmEventArgs();
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Expected O, but got Unknown
			((Control)lab_CamHub).ForeColor = Color.FromArgb(35, 35, 35);
			pan_CamHub.Back = Color.FromArgb(255, 233, 0);
			pic_CamHub.Image = (Image)(object)Resources.Hub_selected;
			Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], btnFontSize);
			((Control)lab_CamHub).Font = font;
			se = new SidebarFrmEventArgs
			{
				Text = ((Control)lab_CamHub).Text,
				Name = ((Control)lab_CamHub).Name
			};
		});
		OnSidebarFrmEvnet?.Invoke(null, se);
	}

	private void RCMode_MouseClick(object sender, MouseEventArgs e)
	{
		RefreshButtonStyle();
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			((Control)lab_RCMode).ForeColor = Color.FromArgb(35, 35, 35);
			pic_RCMode.Image = (Image)(object)Resources.RCMode_selected;
			pan_RCMode.Back = Color.FromArgb(255, 233, 0);
			Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], btnFontSize);
			((Control)lab_RCMode).Font = font;
		});
		SidebarFrmEventArgs e2 = new SidebarFrmEventArgs
		{
			Text = ((Control)lab_RCMode).Text,
			Name = ((Control)lab_RCMode).Name
		};
		OnSidebarFrmEvnet?.Invoke(null, e2);
	}

	private void FirmwareUpgrade_MouseClick(object sender, MouseEventArgs e)
	{
		RefreshButtonStyle();
		SidebarFrmEventArgs se = new SidebarFrmEventArgs();
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Expected O, but got Unknown
			((Control)lab_firmware).ForeColor = Color.FromArgb(35, 35, 35);
			pan_firmware.Back = Color.FromArgb(255, 233, 0);
			pic_firmware.Image = (Image)(object)Resources.固件升级_selected;
			se = new SidebarFrmEventArgs
			{
				Text = ((Control)lab_firmware).Text,
				Name = ((Control)lab_firmware).Name
			};
			Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], btnFontSize);
			((Control)lab_firmware).Font = font;
		});
		OnSidebarFrmEvnet?.Invoke(null, se);
	}

	private void Setting_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		try
		{
			RefreshButtonStyle();
			SidebarFrmEventArgs e2 = new SidebarFrmEventArgs();
			((Control)lab_setting).ForeColor = Color.FromArgb(35, 35, 35);
			pan_setting.Back = Color.FromArgb(255, 233, 0);
			pic_setting.Image = (Image)(object)Resources.软件设置_selected;
			e2 = new SidebarFrmEventArgs();
			e2.Text = ((Control)lab_setting).Text;
			e2.Name = ((Control)lab_setting).Name;
			Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], btnFontSize);
			((Control)lab_setting).Font = font;
			OnSidebarFrmEvnet?.Invoke(null, e2);
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("sidebar.SettingClick error,desc=" + ex.Message, Color.Red);
		}
	}

	private void HelpCenter_MouseClick(object sender, MouseEventArgs e)
	{
		RefreshButtonStyle(isShow: true);
		SidebarFrmEventArgs se = new SidebarFrmEventArgs();
		((Control)this).Invoke((Delegate)(Action)delegate
		{
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Expected O, but got Unknown
			((Control)lab_HelpCenter).ForeColor = Color.FromArgb(35, 35, 35);
			pan_HelpCenter.Back = Color.FromArgb(255, 233, 0);
			pic_HelpCenter.Image = (Image)(object)Resources.帮助中心_selected;
			se = new SidebarFrmEventArgs
			{
				Text = ((Control)lab_HelpCenter).Text,
				Name = ((Control)lab_HelpCenter).Name
			};
			Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], btnFontSize);
			((Control)lab_HelpCenter).Font = font;
		});
		OnSidebarFrmEvnet?.Invoke(null, se);
	}

	private void RefreshButtonStyle(bool isShow = false)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		Font font = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], btnFontSize);
		for (int i = 0; i < picList.Count; i++)
		{
			if (((Control)labList[i]).ForeColor != Color.FromArgb(255, 255, 255))
			{
				((ContainerPanel)grPanList[i]).Back = Color.FromArgb(0, 241, 247, 251);
				((Control)picList[i]).BackColor = Color.FromArgb(0, 241, 247, 251);
				((Control)labList[i]).BackColor = Color.FromArgb(0, 241, 247, 251);
				((Control)labList[i]).ForeColor = Color.FromArgb(255, 255, 255);
				panList[i].Back = Color.FromArgb(0, 255, 233, 0);
				((Control)labList[i]).Font = font;
			}
		}
		for (int j = 0; j < helpPanList.Count; j++)
		{
			if (helpPanList[j].Back != Color.FromName("Transparent"))
			{
				helpPanList[j].Back = Color.FromName("Transparent");
			}
		}
		if ((object)pic_firmware.Image != Resources.固件升级_default)
		{
			pic_firmware.Image = (Image)(object)Resources.固件升级_default;
		}
		if ((object)pic_setting.Image != Resources.软件设置_default)
		{
			pic_setting.Image = (Image)(object)Resources.软件设置_default;
		}
		if ((object)pic_HelpCenter.Image != Resources.帮助中心_default)
		{
			pic_HelpCenter.Image = (Image)(object)Resources.帮助中心_default;
		}
		if ((object)pic_RCMode.Image != Resources.RCMode_default)
		{
			pic_RCMode.Image = (Image)(object)Resources.RCMode_default;
		}
		if ((object)pic_CamHub.Image != Resources.Hub_default)
		{
			pic_CamHub.Image = (Image)(object)Resources.Hub_default;
		}
		if ((object)pic_Channel.Image != Resources.wifi)
		{
			pic_Channel.Image = (Image)(object)Resources.wifi;
		}
		Label obj = lab_prodSer;
		bool visible = (((Control)lab_customerService).Visible = isShow);
		((Control)obj).Visible = visible;
	}

	private void lab_prodSer_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		RefreshHelpButtonStyle();
		Label val = (Label)sender;
		((Control)val).BackColor = Color.FromArgb(0, 0, 0, 0);
		((Control)val).ForeColor = Color.FromArgb(255, 233, 0);
		SidebarFrmEventArgs e2 = new SidebarFrmEventArgs
		{
			Text = ((Control)val).Text,
			Name = ((Control)val).Name
		};
		Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], helpBtnFontSize);
		((Control)val).Font = font;
		OnSidebarFrmEvnet?.Invoke(null, e2);
	}

	private void lab_customerService_MouseClick(object sender, MouseEventArgs e)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		RefreshHelpButtonStyle();
		Label val = (Label)sender;
		((Control)val).BackColor = Color.FromArgb(0, 0, 0, 0);
		((Control)val).ForeColor = Color.FromArgb(255, 233, 0);
		SidebarFrmEventArgs e2 = new SidebarFrmEventArgs
		{
			Text = ((Control)val).Text,
			Name = ((Control)val).Name
		};
		Font font = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], helpBtnFontSize);
		((Control)val).Font = font;
		OnSidebarFrmEvnet?.Invoke(null, e2);
	}

	private void RefreshHelpButtonStyle()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		Font font = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], 10f);
		for (int i = 0; i < helpBtnList.Count; i++)
		{
			helpPanList[i].Back = Color.FromName("Transparent");
			((Control)helpBtnList[i]).ForeColor = Color.FromArgb(153, 255, 255, 255);
			((Control)helpBtnList[i]).Font = font;
		}
	}

	public void ReLoadFont()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		try
		{
			Font font = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], btnFontSize);
			for (int i = 0; i < labList.Count; i++)
			{
				((Control)labList[i]).Font = font;
			}
			Font font2 = new Font(((FontCollection)GD.Inst.TextPFC).Families[0], helpBtnFontSize);
			for (int j = 0; j < helpBtnList.Count; j++)
			{
				((Control)helpBtnList[j]).Font = font2;
			}
			((Control)lab_vers).Font = font2;
			Font font3 = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], btnFontSize);
			((Control)lab_firmware).Font = font3;
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI(_className + ex.Message, Color.Red);
		}
	}

	public void ReloadLang(bool isReloadFont = false)
	{
		try
		{
			((Control)lab_firmware).Text = Lang.T("sidebar_firmware");
			((Control)lab_setting).Text = Lang.T("sidebar_setting");
			((Control)lab_HelpCenter).Text = Lang.T("sidebar_help");
			((Control)lab_prodSer).Text = Lang.T("sidebar_product_series");
			((Control)lab_customerService).Text = Lang.T("sidebar_customer_service");
			((Control)lab_vers).Text = Lang.T("version_label", Program.SoftwareVersion);
			if (isReloadFont)
			{
				ReLoadFont();
			}
		}
		catch (Exception ex)
		{
			WriteLog.WriteLogFileToUI("sidebarCtrl.ReloadLang error,desc=" + ex.Message, Color.Red);
		}
	}

	public void SelectedBtn(string btnname)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		RefreshButtonStyle();
		Font fon = new Font(((FontCollection)GD.Inst.TitlePFC).Families[0], btnFontSize);
		if (!(btnname == "firmware"))
		{
			if (btnname == "updataChannel")
			{
				((Control)lab_Channel).ForeColor = Color.FromArgb(35, 35, 35);
				pan_Channel.Back = Color.FromArgb(255, 233, 0);
				pic_Channel.Image = (Image)(object)Resources.wifi_selected;
				((Control)lab_Channel).Font = fon;
			}
		}
		else
		{
			((Control)this).Invoke((Delegate)(Action)delegate
			{
				((Control)lab_firmware).ForeColor = Color.FromArgb(35, 35, 35);
				pan_firmware.Back = Color.FromArgb(255, 233, 0);
				pic_firmware.Image = (Image)(object)Resources.固件升级_selected;
				((Control)lab_firmware).Font = fon;
			});
		}
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Expected O, but got Unknown
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Expected O, but got Unknown
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Expected O, but got Unknown
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Expected O, but got Unknown
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Expected O, but got Unknown
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Expected O, but got Unknown
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Expected O, but got Unknown
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Expected O, but got Unknown
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Expected O, but got Unknown
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0767: Expected O, but got Unknown
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0986: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a18: Expected O, but got Unknown
		//IL_0a9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b23: Expected O, but got Unknown
		//IL_0b88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf7: Expected O, but got Unknown
		//IL_0c2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c37: Expected O, but got Unknown
		//IL_0c7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cef: Expected O, but got Unknown
		//IL_0d78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dfd: Expected O, but got Unknown
		//IL_0e80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0efe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f08: Expected O, but got Unknown
		//IL_0f3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f48: Expected O, but got Unknown
		//IL_0f78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fed: Expected O, but got Unknown
		//IL_103f: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ae: Expected O, but got Unknown
		//IL_112d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1150: Unknown result type (might be due to invalid IL or missing references)
		//IL_1225: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b7: Expected O, but got Unknown
		//IL_133a: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_13c2: Expected O, but got Unknown
		//IL_1427: Unknown result type (might be due to invalid IL or missing references)
		//IL_148c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1496: Expected O, but got Unknown
		//IL_14cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d6: Expected O, but got Unknown
		//IL_1519: Unknown result type (might be due to invalid IL or missing references)
		//IL_1584: Unknown result type (might be due to invalid IL or missing references)
		//IL_158e: Expected O, but got Unknown
		//IL_160d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1630: Unknown result type (might be due to invalid IL or missing references)
		//IL_1708: Unknown result type (might be due to invalid IL or missing references)
		//IL_1775: Unknown result type (might be due to invalid IL or missing references)
		//IL_177f: Expected O, but got Unknown
		//IL_1802: Unknown result type (might be due to invalid IL or missing references)
		//IL_1880: Unknown result type (might be due to invalid IL or missing references)
		//IL_188a: Expected O, but got Unknown
		//IL_18dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1941: Unknown result type (might be due to invalid IL or missing references)
		//IL_194b: Expected O, but got Unknown
		//IL_19d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a41: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a4b: Expected O, but got Unknown
		//IL_1ace: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b56: Expected O, but got Unknown
		//IL_1ba8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c17: Expected O, but got Unknown
		//IL_1ca1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d25: Expected O, but got Unknown
		//IL_1d9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e25: Expected O, but got Unknown
		//IL_1e95: Unknown result type (might be due to invalid IL or missing references)
		//IL_1efa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f04: Expected O, but got Unknown
		//IL_1f8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2035: Unknown result type (might be due to invalid IL or missing references)
		//IL_2084: Unknown result type (might be due to invalid IL or missing references)
		//IL_208e: Expected O, but got Unknown
		//IL_20ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_20d4: Expected O, but got Unknown
		//IL_210f: Unknown result type (might be due to invalid IL or missing references)
		//IL_217c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2186: Expected O, but got Unknown
		//IL_21ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_21b5: Expected O, but got Unknown
		//IL_21f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_2288: Unknown result type (might be due to invalid IL or missing references)
		lab_firmware = new Label();
		lab_customerService = new Label();
		lab_prodSer = new Label();
		lab_setting = new Label();
		lab_HelpCenter = new Label();
		grpan_use = new GridPanel();
		pan_Channel = new Panel();
		grPan_Channel = new GridPanel();
		pic_Channel = new PictureBox();
		lab_Channel = new Label();
		pan_RCMode = new Panel();
		grpan_RCMode = new GridPanel();
		lab_RCMode = new Label();
		pic_RCMode = new PictureBox();
		pan_customerService = new Panel();
		pan_CamHub = new Panel();
		grpan_CamHub = new GridPanel();
		pic_CamHub = new PictureBox();
		lab_CamHub = new Label();
		pan_prodSer = new Panel();
		pan_HelpCenter = new Panel();
		grpan_HelpCenter = new GridPanel();
		pic_HelpCenter = new PictureBox();
		pan_setting = new Panel();
		grpan_setting = new GridPanel();
		pic_setting = new PictureBox();
		pan_firmware = new Panel();
		grpan_firmware = new GridPanel();
		pic_firmware = new PictureBox();
		grpan_main = new GridPanel();
		panel1 = new Panel();
		lab_vers = new Label();
		label1 = new Label();
		((Control)grpan_use).SuspendLayout();
		((Control)pan_Channel).SuspendLayout();
		((Control)grPan_Channel).SuspendLayout();
		((ISupportInitialize)pic_Channel).BeginInit();
		((Control)pan_RCMode).SuspendLayout();
		((Control)grpan_RCMode).SuspendLayout();
		((ISupportInitialize)pic_RCMode).BeginInit();
		((Control)pan_customerService).SuspendLayout();
		((Control)pan_CamHub).SuspendLayout();
		((Control)grpan_CamHub).SuspendLayout();
		((ISupportInitialize)pic_CamHub).BeginInit();
		((Control)pan_prodSer).SuspendLayout();
		((Control)pan_HelpCenter).SuspendLayout();
		((Control)grpan_HelpCenter).SuspendLayout();
		((ISupportInitialize)pic_HelpCenter).BeginInit();
		((Control)pan_setting).SuspendLayout();
		((Control)grpan_setting).SuspendLayout();
		((ISupportInitialize)pic_setting).BeginInit();
		((Control)pan_firmware).SuspendLayout();
		((Control)grpan_firmware).SuspendLayout();
		((ISupportInitialize)pic_firmware).BeginInit();
		((Control)grpan_main).SuspendLayout();
		((Control)panel1).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)lab_firmware).BackColor = Color.FromArgb(255, 233, 0);
		((Control)lab_firmware).Cursor = Cursors.Hand;
		((Control)lab_firmware).Dock = (DockStyle)5;
		((Control)lab_firmware).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_firmware).ForeColor = Color.FromArgb(35, 35, 35);
		lab_firmware.ImageAlign = (ContentAlignment)16;
		grpan_firmware.SetIndex((Control)(object)lab_firmware, 2);
		((Control)lab_firmware).Location = new Point(54, 5);
		((Control)lab_firmware).Margin = new Padding(0, 5, 5, 5);
		((Control)lab_firmware).Name = "lab_firmware";
		((Control)lab_firmware).Size = new Size(76, 48);
		((Control)lab_firmware).TabIndex = 19;
		((Control)lab_firmware).Text = "固件升级";
		lab_firmware.TextAlign = (ContentAlignment)16;
		((Control)lab_firmware).MouseClick += new MouseEventHandler(FirmwareUpgrade_MouseClick);
		((Control)lab_customerService).BackColor = Color.FromArgb(27, 28, 30);
		((Control)lab_customerService).Dock = (DockStyle)5;
		((Control)lab_customerService).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_customerService).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_customerService).Location = new Point(5, 5);
		((Control)lab_customerService).Margin = new Padding(0);
		((Control)lab_customerService).Name = "lab_customerService";
		((Control)lab_customerService).Size = new Size(125, 28);
		((Control)lab_customerService).TabIndex = 15;
		((Control)lab_customerService).Text = "联系客服";
		lab_customerService.TextAlign = (ContentAlignment)32;
		((Control)lab_customerService).MouseClick += new MouseEventHandler(lab_customerService_MouseClick);
		((Control)lab_prodSer).BackColor = Color.Transparent;
		((Control)lab_prodSer).Dock = (DockStyle)5;
		((Control)lab_prodSer).Font = new Font("Microsoft Sans Serif", 9.749999f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		((Control)lab_prodSer).ForeColor = Color.FromArgb(255, 233, 0);
		((Control)lab_prodSer).Location = new Point(5, 5);
		((Control)lab_prodSer).Margin = new Padding(0);
		((Control)lab_prodSer).Name = "lab_prodSer";
		((Control)lab_prodSer).Size = new Size(125, 28);
		((Control)lab_prodSer).TabIndex = 18;
		((Control)lab_prodSer).Text = "产品系列";
		lab_prodSer.TextAlign = (ContentAlignment)32;
		((Control)lab_prodSer).MouseClick += new MouseEventHandler(lab_prodSer_MouseClick);
		((Control)lab_setting).BackColor = Color.Transparent;
		((Control)lab_setting).Dock = (DockStyle)5;
		((Control)lab_setting).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_setting).ForeColor = Color.White;
		((Control)lab_setting).Location = new Point(54, 5);
		((Control)lab_setting).Margin = new Padding(0, 5, 5, 5);
		((Control)lab_setting).Name = "lab_setting";
		((Control)lab_setting).Size = new Size(76, 48);
		((Control)lab_setting).TabIndex = 19;
		((Control)lab_setting).Text = "软件设置";
		lab_setting.TextAlign = (ContentAlignment)16;
		((Control)lab_setting).MouseClick += new MouseEventHandler(Setting_MouseClick);
		((Control)lab_HelpCenter).AutoSize = true;
		((Control)lab_HelpCenter).BackColor = Color.Transparent;
		((Control)lab_HelpCenter).Dock = (DockStyle)5;
		((Control)lab_HelpCenter).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_HelpCenter).ForeColor = Color.White;
		((Control)lab_HelpCenter).Location = new Point(54, 5);
		((Control)lab_HelpCenter).Margin = new Padding(0, 5, 5, 5);
		((Control)lab_HelpCenter).Name = "lab_HelpCenter";
		((Control)lab_HelpCenter).Size = new Size(76, 48);
		((Control)lab_HelpCenter).TabIndex = 19;
		((Control)lab_HelpCenter).Text = "帮助中心";
		lab_HelpCenter.TextAlign = (ContentAlignment)16;
		((Control)lab_HelpCenter).MouseClick += new MouseEventHandler(HelpCenter_MouseClick);
		((ContainerPanel)grpan_use).Back = Color.FromArgb(27, 28, 30);
		((Control)grpan_use).BackColor = Color.FromArgb(27, 28, 30);
		((Control)grpan_use).Controls.Add((Control)(object)pan_Channel);
		((Control)grpan_use).Controls.Add((Control)(object)pan_RCMode);
		((Control)grpan_use).Controls.Add((Control)(object)pan_customerService);
		((Control)grpan_use).Controls.Add((Control)(object)pan_CamHub);
		((Control)grpan_use).Controls.Add((Control)(object)pan_prodSer);
		((Control)grpan_use).Controls.Add((Control)(object)pan_HelpCenter);
		((Control)grpan_use).Controls.Add((Control)(object)pan_setting);
		((Control)grpan_use).Controls.Add((Control)(object)pan_firmware);
		((Control)grpan_use).Dock = (DockStyle)5;
		grpan_main.SetIndex((Control)(object)grpan_use, 2);
		((Control)grpan_use).Location = new Point(0, 34);
		((Control)grpan_use).Margin = new Padding(0);
		((Control)grpan_use).Name = "grpan_use";
		((ContainerPanel)grpan_use).Radius = 6;
		((Control)grpan_use).Size = new Size(155, 578);
		grpan_use.Span = "100%;100%;100%;100%;\r\n100%;100%;100%;100%;\r\n100%;100%;";
		((Control)grpan_use).TabIndex = 8;
		((Control)grpan_use).Text = "gridPanel1";
		pan_Channel.Back = Color.FromArgb(27, 28, 30);
		((Control)pan_Channel).BackColor = Color.Transparent;
		pan_Channel.Controls.Add((Control)(object)grPan_Channel);
		((Control)pan_Channel).Dock = (DockStyle)5;
		grpan_use.SetIndex((Control)(object)pan_Channel, 2);
		((Control)pan_Channel).Location = new Point(10, 116);
		((Control)pan_Channel).Margin = new Padding(10, 0, 10, 0);
		((Control)pan_Channel).Name = "pan_Channel";
		pan_Channel.Radius = 10;
		((Control)pan_Channel).Size = new Size(135, 58);
		((Control)pan_Channel).TabIndex = 23;
		((Control)pan_Channel).TabStop = false;
		((Control)pan_Channel).Text = "panel1";
		((IControl)pan_Channel).Visible = false;
		((Control)pan_Channel).MouseClick += new MouseEventHandler(lab_Channel_MouseClick);
		((ContainerPanel)grPan_Channel).Back = Color.FromArgb(27, 28, 30);
		((Control)grPan_Channel).BackColor = Color.Transparent;
		((Control)grPan_Channel).Controls.Add((Control)(object)pic_Channel);
		((Control)grPan_Channel).Controls.Add((Control)(object)lab_Channel);
		((Control)grPan_Channel).Dock = (DockStyle)5;
		((Control)grPan_Channel).Location = new Point(0, 0);
		((Control)grPan_Channel).Margin = new Padding(0);
		((Control)grPan_Channel).Name = "grPan_Channel";
		((ContainerPanel)grPan_Channel).Radius = 10;
		((Control)grPan_Channel).Size = new Size(135, 58);
		grPan_Channel.Span = "40% 60%;";
		((Control)grPan_Channel).TabIndex = 1;
		((Control)grPan_Channel).Text = "gridPanel2";
		((Control)grPan_Channel).MouseClick += new MouseEventHandler(lab_Channel_MouseClick);
		((Control)pic_Channel).BackColor = Color.Transparent;
		((Control)pic_Channel).Dock = (DockStyle)5;
		pic_Channel.Image = (Image)(object)Resources.wifi;
		grPan_Channel.SetIndex((Control)(object)pic_Channel, 1);
		((Control)pic_Channel).Location = new Point(20, 17);
		((Control)pic_Channel).Margin = new Padding(20, 17, 0, 17);
		((Control)pic_Channel).Name = "pic_Channel";
		((Control)pic_Channel).Size = new Size(34, 24);
		pic_Channel.SizeMode = (PictureBoxSizeMode)4;
		pic_Channel.TabIndex = 0;
		pic_Channel.TabStop = false;
		((Control)pic_Channel).MouseClick += new MouseEventHandler(lab_Channel_MouseClick);
		((Control)lab_Channel).BackColor = Color.Transparent;
		((Control)lab_Channel).Dock = (DockStyle)5;
		((Control)lab_Channel).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_Channel).ForeColor = Color.White;
		grPan_Channel.SetIndex((Control)(object)lab_Channel, 2);
		((Control)lab_Channel).Location = new Point(54, 5);
		((Control)lab_Channel).Margin = new Padding(0, 5, 5, 5);
		((Control)lab_Channel).Name = "lab_Channel";
		((Control)lab_Channel).Size = new Size(76, 48);
		((Control)lab_Channel).TabIndex = 19;
		((Control)lab_Channel).Text = "Channel";
		lab_Channel.TextAlign = (ContentAlignment)16;
		((Control)lab_Channel).MouseClick += new MouseEventHandler(lab_Channel_MouseClick);
		pan_RCMode.Back = Color.FromArgb(27, 28, 30);
		((Control)pan_RCMode).BackColor = Color.Transparent;
		pan_RCMode.Controls.Add((Control)(object)grpan_RCMode);
		((Control)pan_RCMode).Dock = (DockStyle)5;
		grpan_use.SetIndex((Control)(object)pan_RCMode, 3);
		((Control)pan_RCMode).Location = new Point(10, 173);
		((Control)pan_RCMode).Margin = new Padding(10, 0, 10, 0);
		((Control)pan_RCMode).Name = "pan_RCMode";
		pan_RCMode.Radius = 10;
		((Control)pan_RCMode).Size = new Size(135, 58);
		((Control)pan_RCMode).TabIndex = 22;
		((Control)pan_RCMode).Text = "panel1";
		((IControl)pan_RCMode).Visible = false;
		((Control)pan_RCMode).MouseClick += new MouseEventHandler(RCMode_MouseClick);
		((ContainerPanel)grpan_RCMode).Back = Color.FromArgb(27, 28, 30);
		((Control)grpan_RCMode).BackColor = Color.Transparent;
		((Control)grpan_RCMode).Controls.Add((Control)(object)lab_RCMode);
		((Control)grpan_RCMode).Controls.Add((Control)(object)pic_RCMode);
		((Control)grpan_RCMode).Dock = (DockStyle)5;
		((Control)grpan_RCMode).Location = new Point(0, 0);
		((Control)grpan_RCMode).Margin = new Padding(0);
		((Control)grpan_RCMode).Name = "grpan_RCMode";
		((ContainerPanel)grpan_RCMode).Radius = 10;
		((Control)grpan_RCMode).Size = new Size(135, 58);
		grpan_RCMode.Span = "40% 60%;";
		((Control)grpan_RCMode).TabIndex = 2;
		((Control)grpan_RCMode).Text = "gridPanel2";
		((Control)grpan_RCMode).MouseClick += new MouseEventHandler(RCMode_MouseClick);
		((Control)lab_RCMode).BackColor = Color.Transparent;
		((Control)lab_RCMode).Dock = (DockStyle)5;
		((Control)lab_RCMode).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_RCMode).ForeColor = Color.White;
		((Control)lab_RCMode).Location = new Point(54, 5);
		((Control)lab_RCMode).Margin = new Padding(0, 5, 5, 5);
		((Control)lab_RCMode).Name = "lab_RCMode";
		((Control)lab_RCMode).Size = new Size(76, 48);
		((Control)lab_RCMode).TabIndex = 20;
		((Control)lab_RCMode).Text = "RC Mode";
		lab_RCMode.TextAlign = (ContentAlignment)16;
		((Control)lab_RCMode).MouseClick += new MouseEventHandler(RCMode_MouseClick);
		((Control)pic_RCMode).BackColor = Color.Transparent;
		((Control)pic_RCMode).Dock = (DockStyle)5;
		pic_RCMode.Image = (Image)(object)Resources.RCMode_default;
		((Control)pic_RCMode).Location = new Point(20, 17);
		((Control)pic_RCMode).Margin = new Padding(20, 17, 0, 17);
		((Control)pic_RCMode).Name = "pic_RCMode";
		((Control)pic_RCMode).Size = new Size(34, 24);
		pic_RCMode.SizeMode = (PictureBoxSizeMode)4;
		pic_RCMode.TabIndex = 1;
		pic_RCMode.TabStop = false;
		((Control)pic_RCMode).MouseClick += new MouseEventHandler(RCMode_MouseClick);
		pan_customerService.Back = Color.Transparent;
		((Control)pan_customerService).BackColor = Color.Transparent;
		pan_customerService.Controls.Add((Control)(object)lab_customerService);
		((Control)pan_customerService).Dock = (DockStyle)5;
		grpan_use.SetIndex((Control)(object)pan_customerService, 6);
		((Control)pan_customerService).Location = new Point(10, 415);
		((Control)pan_customerService).Margin = new Padding(10);
		((Control)pan_customerService).Name = "pan_customerService";
		((Control)pan_customerService).Padding = new Padding(5);
		pan_customerService.Radius = 10;
		((Control)pan_customerService).Size = new Size(135, 38);
		((Control)pan_customerService).TabIndex = 21;
		((Control)pan_customerService).Text = "panel4";
		pan_CamHub.Back = Color.FromArgb(27, 28, 30);
		((Control)pan_CamHub).BackColor = Color.Transparent;
		pan_CamHub.Controls.Add((Control)(object)grpan_CamHub);
		((Control)pan_CamHub).Dock = (DockStyle)5;
		grpan_use.SetIndex((Control)(object)pan_CamHub, 2);
		((Control)pan_CamHub).Location = new Point(10, 58);
		((Control)pan_CamHub).Margin = new Padding(10, 0, 10, 0);
		((Control)pan_CamHub).Name = "pan_CamHub";
		pan_CamHub.Radius = 10;
		((Control)pan_CamHub).Size = new Size(135, 58);
		((Control)pan_CamHub).TabIndex = 20;
		((Control)pan_CamHub).TabStop = false;
		((Control)pan_CamHub).Text = "panel1";
		((IControl)pan_CamHub).Visible = false;
		((Control)pan_CamHub).MouseClick += new MouseEventHandler(CamHub_MouseClick);
		((ContainerPanel)grpan_CamHub).Back = Color.FromArgb(27, 28, 30);
		((Control)grpan_CamHub).BackColor = Color.Transparent;
		((Control)grpan_CamHub).Controls.Add((Control)(object)pic_CamHub);
		((Control)grpan_CamHub).Controls.Add((Control)(object)lab_CamHub);
		((Control)grpan_CamHub).Dock = (DockStyle)5;
		((Control)grpan_CamHub).Location = new Point(0, 0);
		((Control)grpan_CamHub).Margin = new Padding(0);
		((Control)grpan_CamHub).Name = "grpan_CamHub";
		((ContainerPanel)grpan_CamHub).Radius = 10;
		((Control)grpan_CamHub).Size = new Size(135, 58);
		grpan_CamHub.Span = "40% 60%;";
		((Control)grpan_CamHub).TabIndex = 1;
		((Control)grpan_CamHub).Text = "gridPanel2";
		((Control)grpan_CamHub).MouseClick += new MouseEventHandler(CamHub_MouseClick);
		((Control)pic_CamHub).BackColor = Color.Transparent;
		((Control)pic_CamHub).Dock = (DockStyle)5;
		pic_CamHub.Image = (Image)(object)Resources.Hub_default;
		grpan_CamHub.SetIndex((Control)(object)pic_CamHub, 1);
		((Control)pic_CamHub).Location = new Point(20, 17);
		((Control)pic_CamHub).Margin = new Padding(20, 17, 0, 17);
		((Control)pic_CamHub).Name = "pic_CamHub";
		((Control)pic_CamHub).Size = new Size(34, 24);
		pic_CamHub.SizeMode = (PictureBoxSizeMode)4;
		pic_CamHub.TabIndex = 0;
		pic_CamHub.TabStop = false;
		((Control)pic_CamHub).MouseClick += new MouseEventHandler(CamHub_MouseClick);
		((Control)lab_CamHub).BackColor = Color.Transparent;
		((Control)lab_CamHub).Dock = (DockStyle)5;
		((Control)lab_CamHub).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_CamHub).ForeColor = Color.White;
		grpan_CamHub.SetIndex((Control)(object)lab_CamHub, 2);
		((Control)lab_CamHub).Location = new Point(54, 5);
		((Control)lab_CamHub).Margin = new Padding(0, 5, 5, 5);
		((Control)lab_CamHub).Name = "lab_CamHub";
		((Control)lab_CamHub).Size = new Size(76, 48);
		((Control)lab_CamHub).TabIndex = 19;
		((Control)lab_CamHub).Text = "Hub";
		lab_CamHub.TextAlign = (ContentAlignment)16;
		((Control)lab_CamHub).MouseClick += new MouseEventHandler(CamHub_MouseClick);
		pan_prodSer.Back = Color.Transparent;
		((Control)pan_prodSer).BackColor = Color.Transparent;
		pan_prodSer.Controls.Add((Control)(object)lab_prodSer);
		((Control)pan_prodSer).Dock = (DockStyle)5;
		grpan_use.SetIndex((Control)(object)pan_prodSer, 5);
		((Control)pan_prodSer).Location = new Point(10, 357);
		((Control)pan_prodSer).Margin = new Padding(10);
		((Control)pan_prodSer).Name = "pan_prodSer";
		((Control)pan_prodSer).Padding = new Padding(5);
		pan_prodSer.Radius = 10;
		((Control)pan_prodSer).Size = new Size(135, 38);
		((Control)pan_prodSer).TabIndex = 19;
		((Control)pan_prodSer).Text = "panel2";
		pan_HelpCenter.Back = Color.FromArgb(27, 28, 30);
		((Control)pan_HelpCenter).BackColor = Color.Transparent;
		pan_HelpCenter.Controls.Add((Control)(object)grpan_HelpCenter);
		((Control)pan_HelpCenter).Dock = (DockStyle)5;
		grpan_use.SetIndex((Control)(object)pan_HelpCenter, 4);
		((Control)pan_HelpCenter).Location = new Point(10, 289);
		((Control)pan_HelpCenter).Margin = new Padding(10, 0, 10, 0);
		((Control)pan_HelpCenter).Name = "pan_HelpCenter";
		pan_HelpCenter.Radius = 10;
		((Control)pan_HelpCenter).Size = new Size(135, 58);
		((Control)pan_HelpCenter).TabIndex = 4;
		((Control)pan_HelpCenter).Text = "panel1";
		((Control)pan_HelpCenter).MouseClick += new MouseEventHandler(HelpCenter_MouseClick);
		((ContainerPanel)grpan_HelpCenter).Back = Color.FromArgb(27, 28, 30);
		((Control)grpan_HelpCenter).BackColor = Color.Transparent;
		((Control)grpan_HelpCenter).Controls.Add((Control)(object)lab_HelpCenter);
		((Control)grpan_HelpCenter).Controls.Add((Control)(object)pic_HelpCenter);
		((Control)grpan_HelpCenter).Dock = (DockStyle)5;
		((Control)grpan_HelpCenter).Location = new Point(0, 0);
		((Control)grpan_HelpCenter).Margin = new Padding(0);
		((Control)grpan_HelpCenter).Name = "grpan_HelpCenter";
		((ContainerPanel)grpan_HelpCenter).Radius = 10;
		((Control)grpan_HelpCenter).Size = new Size(135, 58);
		grpan_HelpCenter.Span = "40% 60%;";
		((Control)grpan_HelpCenter).TabIndex = 1;
		((Control)grpan_HelpCenter).Text = "gridPanel1";
		((Control)grpan_HelpCenter).MouseClick += new MouseEventHandler(HelpCenter_MouseClick);
		((Control)pic_HelpCenter).BackColor = Color.Transparent;
		((Control)pic_HelpCenter).Dock = (DockStyle)5;
		pic_HelpCenter.Image = (Image)(object)Resources.帮助中心_default;
		((Control)pic_HelpCenter).Location = new Point(20, 17);
		((Control)pic_HelpCenter).Margin = new Padding(20, 17, 0, 17);
		((Control)pic_HelpCenter).Name = "pic_HelpCenter";
		((Control)pic_HelpCenter).Size = new Size(34, 24);
		pic_HelpCenter.SizeMode = (PictureBoxSizeMode)4;
		pic_HelpCenter.TabIndex = 0;
		pic_HelpCenter.TabStop = false;
		((Control)pic_HelpCenter).MouseClick += new MouseEventHandler(HelpCenter_MouseClick);
		pan_setting.Back = Color.FromArgb(27, 28, 30);
		((Control)pan_setting).BackColor = Color.Transparent;
		pan_setting.Controls.Add((Control)(object)grpan_setting);
		((Control)pan_setting).Dock = (DockStyle)5;
		grpan_use.SetIndex((Control)(object)pan_setting, 4);
		((Control)pan_setting).Location = new Point(10, 231);
		((Control)pan_setting).Margin = new Padding(10, 0, 10, 0);
		((Control)pan_setting).Name = "pan_setting";
		pan_setting.Radius = 10;
		((Control)pan_setting).Size = new Size(135, 58);
		((Control)pan_setting).TabIndex = 3;
		((Control)pan_setting).Text = "panel1";
		((Control)pan_setting).MouseClick += new MouseEventHandler(Setting_MouseClick);
		((ContainerPanel)grpan_setting).Back = Color.FromArgb(27, 28, 30);
		((Control)grpan_setting).BackColor = Color.Transparent;
		((Control)grpan_setting).Controls.Add((Control)(object)lab_setting);
		((Control)grpan_setting).Controls.Add((Control)(object)pic_setting);
		((Control)grpan_setting).Dock = (DockStyle)5;
		((Control)grpan_setting).Location = new Point(0, 0);
		((Control)grpan_setting).Margin = new Padding(0);
		((Control)grpan_setting).Name = "grpan_setting";
		((ContainerPanel)grpan_setting).Radius = 10;
		((Control)grpan_setting).Size = new Size(135, 58);
		grpan_setting.Span = "40% 60%;";
		((Control)grpan_setting).TabIndex = 1;
		((Control)grpan_setting).Text = "gridPanel1";
		((Control)grpan_setting).MouseClick += new MouseEventHandler(Setting_MouseClick);
		((Control)pic_setting).BackColor = Color.Transparent;
		((Control)pic_setting).Dock = (DockStyle)5;
		pic_setting.Image = (Image)(object)Resources.软件设置_default;
		((Control)pic_setting).Location = new Point(20, 17);
		((Control)pic_setting).Margin = new Padding(20, 17, 0, 17);
		((Control)pic_setting).Name = "pic_setting";
		((Control)pic_setting).Size = new Size(34, 24);
		pic_setting.SizeMode = (PictureBoxSizeMode)4;
		pic_setting.TabIndex = 0;
		pic_setting.TabStop = false;
		((Control)pic_setting).MouseClick += new MouseEventHandler(Setting_MouseClick);
		pan_firmware.Back = Color.FromArgb(255, 233, 0);
		((Control)pan_firmware).BackColor = Color.Transparent;
		pan_firmware.Controls.Add((Control)(object)grpan_firmware);
		((Control)pan_firmware).Dock = (DockStyle)5;
		grpan_use.SetIndex((Control)(object)pan_firmware, 2);
		((Control)pan_firmware).Location = new Point(10, 0);
		((Control)pan_firmware).Margin = new Padding(10, 0, 10, 0);
		((Control)pan_firmware).Name = "pan_firmware";
		pan_firmware.Radius = 10;
		((Control)pan_firmware).Size = new Size(135, 58);
		((Control)pan_firmware).TabIndex = 2;
		((Control)pan_firmware).TabStop = false;
		((Control)pan_firmware).Text = "panel1";
		((Control)pan_firmware).MouseClick += new MouseEventHandler(CamHub_MouseClick);
		((Control)grpan_firmware).BackColor = Color.Transparent;
		((Control)grpan_firmware).Controls.Add((Control)(object)pic_firmware);
		((Control)grpan_firmware).Controls.Add((Control)(object)lab_firmware);
		((Control)grpan_firmware).Cursor = Cursors.Hand;
		((Control)grpan_firmware).Dock = (DockStyle)5;
		((Control)grpan_firmware).Location = new Point(0, 0);
		((Control)grpan_firmware).Margin = new Padding(0);
		((Control)grpan_firmware).Name = "grpan_firmware";
		((ContainerPanel)grpan_firmware).Radius = 10;
		((Control)grpan_firmware).Size = new Size(135, 58);
		grpan_firmware.Span = "40% 60%;";
		((Control)grpan_firmware).TabIndex = 1;
		((Control)grpan_firmware).Text = "gridPanel4";
		((Control)grpan_firmware).MouseClick += new MouseEventHandler(FirmwareUpgrade_MouseClick);
		((Control)pic_firmware).BackColor = Color.FromArgb(255, 233, 0);
		((Control)pic_firmware).Dock = (DockStyle)5;
		pic_firmware.Image = (Image)(object)Resources.固件升级_selected;
		grpan_firmware.SetIndex((Control)(object)pic_firmware, 1);
		((Control)pic_firmware).Location = new Point(20, 17);
		((Control)pic_firmware).Margin = new Padding(20, 17, 0, 17);
		((Control)pic_firmware).Name = "pic_firmware";
		((Control)pic_firmware).Size = new Size(34, 24);
		pic_firmware.SizeMode = (PictureBoxSizeMode)4;
		pic_firmware.TabIndex = 0;
		pic_firmware.TabStop = false;
		((Control)pic_firmware).MouseClick += new MouseEventHandler(FirmwareUpgrade_MouseClick);
		((ContainerPanel)grpan_main).Back = Color.FromArgb(27, 28, 30);
		((Control)grpan_main).Controls.Add((Control)(object)panel1);
		((Control)grpan_main).Controls.Add((Control)(object)label1);
		((Control)grpan_main).Controls.Add((Control)(object)grpan_use);
		((Control)grpan_main).Dock = (DockStyle)5;
		((Control)grpan_main).Location = new Point(0, 0);
		((Control)grpan_main).Margin = new Padding(0);
		((Control)grpan_main).Name = "grpan_main";
		((Control)grpan_main).Size = new Size(155, 680);
		grpan_main.Span = "100%;100%;100%;\r\n-5% 85% 10%";
		((Control)grpan_main).TabIndex = 0;
		((Control)grpan_main).Text = "gridPanel2";
		((Control)panel1).Controls.Add((Control)(object)lab_vers);
		((Control)panel1).Dock = (DockStyle)5;
		((Control)panel1).Location = new Point(5, 617);
		((Control)panel1).Margin = new Padding(5);
		((Control)panel1).Name = "panel1";
		((Control)panel1).Size = new Size(145, 58);
		((Control)panel1).TabIndex = 11;
		((Control)panel1).MouseClick += new MouseEventHandler(lab_vers_MouseClick);
		((Control)lab_vers).BackColor = Color.FromArgb(27, 28, 30);
		((Control)lab_vers).Dock = (DockStyle)5;
		((Control)lab_vers).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)lab_vers).ForeColor = Color.FromArgb(164, 164, 165);
		((Control)lab_vers).Location = new Point(0, 0);
		((Control)lab_vers).Margin = new Padding(0);
		((Control)lab_vers).Name = "lab_vers";
		((Control)lab_vers).Size = new Size(145, 58);
		((Control)lab_vers).TabIndex = 1;
		((Control)lab_vers).Text = "版本号";
		lab_vers.TextAlign = (ContentAlignment)32;
		((Control)lab_vers).MouseClick += new MouseEventHandler(lab_vers_MouseClick);
		((Control)label1).Dock = (DockStyle)5;
		((Control)label1).Font = new Font("Microsoft Sans Serif", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label1).ForeColor = Color.White;
		grpan_main.SetIndex((Control)(object)label1, 1);
		((Control)label1).Location = new Point(5, 5);
		((Control)label1).Margin = new Padding(5);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(145, 24);
		((Control)label1).TabIndex = 10;
		label1.TextAlign = (ContentAlignment)32;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.FromArgb(27, 28, 30);
		((Control)this).Controls.Add((Control)(object)grpan_main);
		((Control)this).Margin = new Padding(0);
		((Control)this).Name = "SidebarCtrl";
		((Control)this).Size = new Size(155, 680);
		((UserControl)this).Load += SidebarCtrl_Load;
		((Control)grpan_use).ResumeLayout(false);
		((Control)pan_Channel).ResumeLayout(false);
		((Control)grPan_Channel).ResumeLayout(false);
		((ISupportInitialize)pic_Channel).EndInit();
		((Control)pan_RCMode).ResumeLayout(false);
		((Control)grpan_RCMode).ResumeLayout(false);
		((ISupportInitialize)pic_RCMode).EndInit();
		((Control)pan_customerService).ResumeLayout(false);
		((Control)pan_CamHub).ResumeLayout(false);
		((Control)grpan_CamHub).ResumeLayout(false);
		((ISupportInitialize)pic_CamHub).EndInit();
		((Control)pan_prodSer).ResumeLayout(false);
		((Control)pan_HelpCenter).ResumeLayout(false);
		((Control)grpan_HelpCenter).ResumeLayout(false);
		((Control)grpan_HelpCenter).PerformLayout();
		((ISupportInitialize)pic_HelpCenter).EndInit();
		((Control)pan_setting).ResumeLayout(false);
		((Control)grpan_setting).ResumeLayout(false);
		((ISupportInitialize)pic_setting).EndInit();
		((Control)pan_firmware).ResumeLayout(false);
		((Control)grpan_firmware).ResumeLayout(false);
		((ISupportInitialize)pic_firmware).EndInit();
		((Control)grpan_main).ResumeLayout(false);
		((Control)panel1).ResumeLayout(false);
		((Control)this).ResumeLayout(false);
	}
}
