using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

internal class AutoSizeFormClass
{
	public struct controlRect
	{
		public int Left;

		public int Top;

		public int Width;

		public int Height;
	}

	public List<controlRect> oldCtrl = new List<controlRect>();

	private int ctrlNo = 0;

	public void controllInitializeSize(Control mForm)
	{
		controlRect item = default(controlRect);
		item.Left = mForm.Left;
		item.Top = mForm.Top;
		item.Width = mForm.Width;
		item.Height = mForm.Height;
		oldCtrl.Add(item);
		AddControl(mForm);
	}

	private void AddControl(Control ctl)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		controlRect item = default(controlRect);
		foreach (Control item2 in (ArrangedElementCollection)ctl.Controls)
		{
			Control val = item2;
			item.Left = val.Left;
			item.Top = val.Top;
			item.Width = val.Width;
			item.Height = val.Height;
			oldCtrl.Add(item);
			if (((ArrangedElementCollection)val.Controls).Count > 0)
			{
				AddControl(val);
			}
		}
	}

	public void controlAutoSize(Control mForm)
	{
		if (ctrlNo == 0)
		{
			controlRect item = default(controlRect);
			item.Left = 0;
			item.Top = 0;
			item.Width = mForm.PreferredSize.Width;
			item.Height = mForm.PreferredSize.Height;
			oldCtrl.Add(item);
			AddControl(mForm);
		}
		float wScale = (float)mForm.Width / (float)oldCtrl[0].Width;
		float hScale = (float)mForm.Height / (float)oldCtrl[0].Height;
		ctrlNo = 1;
		AutoScaleControl(mForm, wScale, hScale);
	}

	private void AutoScaleControl(Control ctl, float wScale, float hScale)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		foreach (Control item in (ArrangedElementCollection)ctl.Controls)
		{
			Control val = item;
			int left = oldCtrl[ctrlNo].Left;
			int top = oldCtrl[ctrlNo].Top;
			int width = oldCtrl[ctrlNo].Width;
			int height = oldCtrl[ctrlNo].Height;
			val.Left = (int)((float)left * wScale);
			val.Top = (int)((float)top * hScale);
			val.Width = (int)((float)width * wScale);
			val.Height = (int)((float)height * hScale);
			ctrlNo++;
			if (((ArrangedElementCollection)val.Controls).Count > 0)
			{
				AutoScaleControl(val, wScale, hScale);
			}
			if (ctl is DataGridView)
			{
				DataGridView val2 = (DataGridView)(object)((ctl is DataGridView) ? ctl : null);
				Cursor.Current = Cursors.WaitCursor;
				int num = 0;
				for (int i = 0; i < ((BaseCollection)val2.Columns).Count; i++)
				{
					val2.AutoResizeColumn(i, (DataGridViewAutoSizeColumnMode)6);
					num += val2.Columns[i].Width;
				}
				if (num >= ctl.Size.Width)
				{
					val2.AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)10;
				}
				else
				{
					val2.AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)16;
				}
				Cursor.Current = Cursors.Default;
			}
		}
	}
}
