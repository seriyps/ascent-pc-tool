using System.Drawing;
using AntdUI;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class FirmwareInfo : NotifyProperty
{
	private CellLink[] _btns;

	private string _name;

	public string Version { get; set; }

	public string ReleaseDate { get; set; }

	public string FirmwareName { get; set; }

	public CellLink[] btns
	{
		get
		{
			return _btns;
		}
		set
		{
			_btns = value;
			((NotifyProperty)this).OnPropertyChanged("btns");
		}
	}

	public string name
	{
		get
		{
			return _name;
		}
		set
		{
			if (!(_name == value))
			{
				_name = value;
				((NotifyProperty)this).OnPropertyChanged("name");
			}
		}
	}

	public FirmwareInfo()
	{
		Version = "1.1.1";
		ReleaseDate = "1-2-3";
		FirmwareName = "abc";
	}

	public FirmwareInfo(string vers, string date, string name)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		((NotifyProperty)this)._002Ector();
		Version = vers;
		ReleaseDate = date;
		FirmwareName = name;
		_btns = (CellLink[])(object)new CellLink[3]
		{
			(CellLink)new CellButton("id", (string)null, (TTypeMini)1).SetIconHover((Image)(object)Resources.CADDXFPV_Discord, 200),
			(CellLink)new CellButton("id", (string)null, (TTypeMini)4),
			(CellLink)new CellButton("id", (string)null, (TTypeMini)3).SetArrow(true)
		};
	}
}
