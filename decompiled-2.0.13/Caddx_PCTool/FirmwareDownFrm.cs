using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Caddx_PCTool;

public class FirmwareDownFrm : Form
{
	private IContainer components = null;

	public FirmwareDownFrm()
	{
		InitializeComponent();
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
		components = new Container();
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(800, 450);
		((Control)this).Text = "FirmwareDownFrm";
	}
}
