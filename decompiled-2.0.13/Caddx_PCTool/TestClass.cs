using System;
using System.Drawing;
using AntdUI;
using Caddx_PCTool.Properties;

namespace Caddx_PCTool;

public class TestClass : NotifyProperty
{
	private bool _check;

	private bool _radio;

	private bool _checkTitle;

	private string _name;

	private CellBadge _online;

	private bool _enable;

	private int _age;

	private DateTime _date;

	private string _address;

	private CellTag[] _tag;

	private CellImage[] _imgs;

	private CellLink[] _btns;

	public int id { get; set; }

	public bool check
	{
		get
		{
			return _check;
		}
		set
		{
			if (_check != value)
			{
				_check = value;
				((NotifyProperty)this).OnPropertyChanged("check");
			}
		}
	}

	public bool radio
	{
		get
		{
			return _radio;
		}
		set
		{
			if (_radio != value)
			{
				_radio = value;
				((NotifyProperty)this).OnPropertyChanged("radio");
			}
		}
	}

	public bool checkTitle
	{
		get
		{
			return _checkTitle;
		}
		set
		{
			if (_checkTitle != value)
			{
				_checkTitle = value;
				((NotifyProperty)this).OnPropertyChanged("checkTitle");
			}
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

	public CellBadge online
	{
		get
		{
			return _online;
		}
		set
		{
			_online = value;
			((NotifyProperty)this).OnPropertyChanged("online");
		}
	}

	public bool enable
	{
		get
		{
			return _enable;
		}
		set
		{
			if (_enable != value)
			{
				_enable = value;
				((NotifyProperty)this).OnPropertyChanged("enable");
			}
		}
	}

	public int age
	{
		get
		{
			return _age;
		}
		set
		{
			if (_age != value)
			{
				_age = value;
				((NotifyProperty)this).OnPropertyChanged("age");
			}
		}
	}

	public DateTime date
	{
		get
		{
			return _date;
		}
		set
		{
			if (!(_date == value))
			{
				_date = value;
				((NotifyProperty)this).OnPropertyChanged("date");
			}
		}
	}

	public string address
	{
		get
		{
			return _address;
		}
		set
		{
			if (!(_address == value))
			{
				_address = value;
				((NotifyProperty)this).OnPropertyChanged("address");
			}
		}
	}

	public CellTag[] tag
	{
		get
		{
			return _tag;
		}
		set
		{
			_tag = value;
			((NotifyProperty)this).OnPropertyChanged("tag");
		}
	}

	public CellImage[] imgs
	{
		get
		{
			return _imgs;
		}
		set
		{
			_imgs = value;
			((NotifyProperty)this).OnPropertyChanged("imgs");
		}
	}

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

	public TestClass(int index, int start, string name, int age)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Expected O, but got Unknown
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Expected O, but got Unknown
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Expected O, but got Unknown
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		_check = false;
		_radio = false;
		_checkTitle = false;
		_enable = false;
		((NotifyProperty)this)._002Ector();
		id = index + 1;
		switch (start)
		{
		case 1:
			_online = new CellBadge((TState)2, Localization.Get("Table.Data.Online", "在线"));
			break;
		case 2:
			_online = new CellBadge((TState)4, Localization.Get("Table.Data.Online.Processing", "处置"));
			break;
		case 3:
			_online = new CellBadge((TState)3, Localization.Get("Table.Data.Online.Error", "离线"));
			break;
		case 4:
			_online = new CellBadge((TState)5, Localization.Get("Table.Data.Online.Warn", "离线"));
			break;
		default:
			_online = new CellBadge((TState)1, Localization.Get("Table.Data.Online.Default", "常规"));
			break;
		}
		_name = name;
		_age = age;
		_date = DateTime.Now.Date.AddYears(-age);
		_address = Localization.GetLangI("Table.Data.Address" + id, (string)null);
		if (_address == null)
		{
			_address = Localization.GetLangI("Table.Data.AddressNum", (string)null);
		}
		if (_address == null)
		{
			_address = ((new Random().Next(DateTime.Now.Second) > 5) ? "东湖" : "西湖") + "区湖底公园" + id + "号";
		}
		else
		{
			_address += id;
		}
		_enable = start % 2 == 0;
		if (start == 1)
		{
		}
		switch (start)
		{
		case 1:
			_btns = (CellLink[])(object)new CellLink[3]
			{
				(CellLink)new CellButton("id", (string)null, (TTypeMini)1).SetIcon("SearchOutlined").SetIconHover((Image)(object)Resources.CADDXFPV_Discord, 200),
				(CellLink)new CellButton("id", (string)null, (TTypeMini)4).SetIcon("ArrowDownOutlined"),
				(CellLink)new CellButton("id", (string)null, (TTypeMini)3).SetArrow(true)
			};
			break;
		case 2:
			_btns = (CellLink[])(object)new CellLink[3]
			{
				(CellLink)new CellButton("id").SetBorder(1f).SetIcon("SearchOutlined").SetIconHover((Image)(object)Resources.Ascent_CamHub, 200),
				(CellLink)new CellButton("id").SetBorder(1f).SetIcon("ArrowDownOutlined"),
				(CellLink)new CellButton("id").SetBorder(1f).SetArrow(true)
			};
			break;
		case 3:
			_btns = (CellLink[])(object)new CellLink[3]
			{
				(CellLink)new CellButton("id").SetBorder(1f).SetGhost(true).SetIcon("SearchOutlined")
					.SetIconHover((Image)(object)Resources.Ascent_CamHub, 200),
				(CellLink)new CellButton("id").SetBorder(1f).SetGhost(true).SetIcon("ArrowDownOutlined"),
				(CellLink)new CellButton("id").SetBorder(1f).SetGhost(true).SetArrow(true)
			};
			break;
		case 4:
			_btns = (CellLink[])(object)new CellLink[2]
			{
				(CellLink)new CellButton("edit", "Edit", (TTypeMini)1),
				(CellLink)new CellButton("delete", "Delete", (TTypeMini)3)
			};
			break;
		case 5:
			_btns = (CellLink[])(object)new CellLink[2]
			{
				(CellLink)new CellButton("edit", "Edit", (TTypeMini)1).SetBorder(1f).SetGhost(true),
				(CellLink)new CellButton("delete", "Delete", (TTypeMini)3).SetBorder(1f).SetGhost(true)
			};
			break;
		case 6:
			_btns = (CellLink[])(object)new CellLink[1] { (CellLink)new CellButton("download", "Download", (TTypeMini)2).SetIcon("DownloadOutlined") };
			break;
		default:
			_btns = (CellLink[])(object)new CellLink[1]
			{
				new CellLink("delete", "Delete")
			};
			break;
		}
	}
}
