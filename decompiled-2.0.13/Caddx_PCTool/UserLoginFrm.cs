using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AntdUI;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class UserLoginFrm : Form
{
	private IContainer components = null;

	private PictureBox pictureBox1;

	private Button btn_logion;

	private Button btn_register;

	private Button btn_VisitorLogin;

	private Checkbox checkbox1;

	private Label label2;

	private Label label3;

	private PageHeader pageHeader1;

	private Panel panel1;

	private LinkLabel linkLabel1;

	private Label label1;

	private LinkLabel linkLabel2;

	private Label label4;

	public UserLoginFrm()
	{
		InitializeComponent();
	}

	private void SplashScreen2_Load(object sender, EventArgs e)
	{
		checkbox1.Checked = false;
		Button obj = btn_logion;
		bool enabled = (((IControl)btn_register).Enabled = false);
		((IControl)obj).Enabled = enabled;
	}

	private void label1_MouseClick(object sender, MouseEventArgs e)
	{
	}

	private void checkbox1_CheckedChanged(object sender, BoolEventArgs e)
	{
		Button obj = btn_logion;
		bool enabled = (((IControl)btn_register).Enabled = ((VEventArgs<bool>)(object)e).Value);
		((IControl)obj).Enabled = enabled;
	}

	private void btn_VisitorLogin_Click(object sender, EventArgs e)
	{
	}

	private void btn_logion_Click(object sender, EventArgs e)
	{
	}

	private void btn_register_Click(object sender, EventArgs e)
	{
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
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
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Expected O, but got Unknown
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Expected O, but got Unknown
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Expected O, but got Unknown
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Expected O, but got Unknown
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Expected O, but got Unknown
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Expected O, but got Unknown
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Expected O, but got Unknown
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Expected O, but got Unknown
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Expected O, but got Unknown
		//IL_0833: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ba: Expected O, but got Unknown
		//IL_08da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0957: Unknown result type (might be due to invalid IL or missing references)
		//IL_0961: Expected O, but got Unknown
		//IL_0984: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fc: Expected O, but got Unknown
		//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a27: Expected O, but got Unknown
		//IL_0a46: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserLoginFrm));
		pictureBox1 = new PictureBox();
		btn_logion = new Button();
		btn_register = new Button();
		btn_VisitorLogin = new Button();
		checkbox1 = new Checkbox();
		label2 = new Label();
		label3 = new Label();
		pageHeader1 = new PageHeader();
		panel1 = new Panel();
		linkLabel2 = new LinkLabel();
		linkLabel1 = new LinkLabel();
		label1 = new Label();
		label4 = new Label();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)panel1).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)pictureBox1).BackColor = Color.Silver;
		pictureBox1.Image = (Image)(object)Resources.固件升级_default;
		((Control)pictureBox1).Location = new Point(288, 37);
		((Control)pictureBox1).Name = "pictureBox1";
		((Control)pictureBox1).Size = new Size(400, 120);
		pictureBox1.SizeMode = (PictureBoxSizeMode)3;
		pictureBox1.TabIndex = 2;
		pictureBox1.TabStop = false;
		btn_logion.BackColor = Color.DarkGray;
		((Control)btn_logion).Font = new Font("Arial", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)btn_logion).Location = new Point(81, 131);
		((Control)btn_logion).Margin = new Padding(0);
		((Control)btn_logion).Name = "btn_logion";
		btn_logion.Radius = 100;
		((Control)btn_logion).Size = new Size(113, 52);
		((Control)btn_logion).TabIndex = 1;
		((Control)btn_logion).Text = "登录";
		((Control)btn_logion).Click += btn_logion_Click;
		btn_register.BackColor = Color.DarkGray;
		((Control)btn_register).Font = new Font("Arial", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)btn_register).Location = new Point(207, 131);
		((Control)btn_register).Margin = new Padding(0);
		((Control)btn_register).Name = "btn_register";
		btn_register.Radius = 100;
		((Control)btn_register).Size = new Size(113, 52);
		((Control)btn_register).TabIndex = 3;
		((Control)btn_register).Text = "注册";
		((Control)btn_register).Click += btn_register_Click;
		((Control)btn_VisitorLogin).Font = new Font("Arial", 12f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)btn_VisitorLogin).Location = new Point(81, 194);
		((Control)btn_VisitorLogin).Margin = new Padding(0);
		((Control)btn_VisitorLogin).Name = "btn_VisitorLogin";
		btn_VisitorLogin.Radius = 100;
		((Control)btn_VisitorLogin).Size = new Size(245, 52);
		((Control)btn_VisitorLogin).TabIndex = 4;
		((Control)btn_VisitorLogin).Text = "游客登录";
		((Control)btn_VisitorLogin).Click += btn_VisitorLogin_Click;
		((Control)checkbox1).BackColor = Color.DarkGray;
		((Control)checkbox1).Font = new Font("Arial", 9f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)checkbox1).Location = new Point(81, 94);
		((Control)checkbox1).Margin = new Padding(0);
		((Control)checkbox1).Name = "checkbox1";
		((Control)checkbox1).Size = new Size(241, 30);
		((Control)checkbox1).TabIndex = 5;
		((Control)checkbox1).Text = "我已阅读并同意";
		checkbox1.CheckedChanged += new BoolEventHandler(checkbox1_CheckedChanged);
		label2.AutoSizeMode = (TAutoSize)1;
		((Control)label2).BackColor = Color.DarkGray;
		((Control)label2).Font = new Font("Arial", 20.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		label2.ForeColor = Color.Black;
		((Control)label2).Location = new Point(137, 40);
		((Control)label2).Margin = new Padding(0);
		((Control)label2).Name = "label2";
		label2.ShadowOpacity = 0f;
		((Control)label2).Size = new Size(119, 32);
		((Control)label2).TabIndex = 6;
		((Control)label2).Text = "开始使用";
		label2.TextAlign = (ContentAlignment)32;
		((Control)label3).BackColor = Color.DarkGray;
		((Control)label3).Font = new Font("Arial", 10.5f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		label3.ForeColor = Color.Black;
		((Control)label3).Location = new Point(76, 260);
		((Control)label3).Margin = new Padding(0);
		((Control)label3).Name = "label3";
		((Control)label3).Size = new Size(247, 17);
		((Control)label3).TabIndex = 7;
		((Control)label3).Text = "选择游客登录将以受限模式访问";
		label3.TextAlign = (ContentAlignment)32;
		((Control)pageHeader1).BackColor = Color.SlateGray;
		((Control)pageHeader1).Dock = (DockStyle)1;
		((Control)pageHeader1).Font = new Font("宋体", 18f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		pageHeader1.Icon = (Image)componentResourceManager.GetObject("pageHeader1.Icon");
		((Control)pageHeader1).Location = new Point(0, 0);
		((Control)pageHeader1).Margin = new Padding(0);
		((Control)pageHeader1).Name = "pageHeader1";
		pageHeader1.ShowButton = true;
		pageHeader1.ShowIcon = true;
		((Control)pageHeader1).Size = new Size(1000, 35);
		((Control)pageHeader1).TabIndex = 8;
		((Control)pageHeader1).Text = "PC TOOL";
		((Control)panel1).BackColor = Color.DarkGray;
		((Control)panel1).Controls.Add((Control)(object)linkLabel2);
		((Control)panel1).Controls.Add((Control)(object)label2);
		((Control)panel1).Controls.Add((Control)(object)label3);
		((Control)panel1).Controls.Add((Control)(object)linkLabel1);
		((Control)panel1).Controls.Add((Control)(object)checkbox1);
		((Control)panel1).Controls.Add((Control)(object)btn_VisitorLogin);
		((Control)panel1).Controls.Add((Control)(object)btn_logion);
		((Control)panel1).Controls.Add((Control)(object)btn_register);
		((Control)panel1).Location = new Point(288, 163);
		((Control)panel1).Name = "panel1";
		((Control)panel1).Size = new Size(400, 341);
		((Control)panel1).TabIndex = 9;
		((Control)linkLabel2).AutoSize = true;
		((Control)linkLabel2).Font = new Font("Arial", 9f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)linkLabel2).Location = new Point(248, 100);
		((Control)linkLabel2).Margin = new Padding(0);
		((Control)linkLabel2).Name = "linkLabel2";
		((Control)linkLabel2).Size = new Size(55, 15);
		((Control)linkLabel2).TabIndex = 11;
		linkLabel2.TabStop = true;
		((Control)linkLabel2).Text = "隐私协议";
		((Control)linkLabel1).AutoSize = true;
		((Control)linkLabel1).Font = new Font("Arial", 9f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)linkLabel1).Location = new Point(195, 100);
		((Control)linkLabel1).Margin = new Padding(0);
		((Control)linkLabel1).Name = "linkLabel1";
		((Control)linkLabel1).Size = new Size(55, 15);
		((Control)linkLabel1).TabIndex = 10;
		linkLabel1.TabStop = true;
		((Control)linkLabel1).Text = "用户协议";
		label1.AutoSizePadding = true;
		((Control)label1).Font = new Font("Arial", 9f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)label1).Location = new Point(727, 525);
		((Control)label1).Margin = new Padding(0);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(273, 23);
		((Control)label1).TabIndex = 12;
		((Control)label1).Text = "© CaddxFPV Technology(Shenzhen) Co., Ltd.";
		label1.TextAlign = (ContentAlignment)32;
		((Control)label1).MouseClick += new MouseEventHandler(label1_MouseClick);
		label4.AutoSizePadding = true;
		((Control)label4).Font = new Font("Arial", 9f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
		((Control)label4).Location = new Point(6, 525);
		((Control)label4).Margin = new Padding(0);
		((Control)label4).Name = "label4";
		((Control)label4).Size = new Size(92, 23);
		((Control)label4).TabIndex = 13;
		((Control)label4).Text = "version:V1.0.0";
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 12f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Color.Silver;
		((Form)this).ClientSize = new Size(1000, 550);
		((Control)this).Controls.Add((Control)(object)label4);
		((Control)this).Controls.Add((Control)(object)label1);
		((Control)this).Controls.Add((Control)(object)panel1);
		((Control)this).Controls.Add((Control)(object)pageHeader1);
		((Control)this).Controls.Add((Control)(object)pictureBox1);
		((Form)this).FormBorderStyle = (FormBorderStyle)0;
		((Control)this).Name = "UserLoginFrm";
		((Form)this).StartPosition = (FormStartPosition)1;
		((Control)this).Text = "SplashScreen2";
		((Form)this).Load += SplashScreen2_Load;
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)panel1).ResumeLayout(false);
		((Control)panel1).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
