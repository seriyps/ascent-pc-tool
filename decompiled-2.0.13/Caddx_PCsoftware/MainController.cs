using System;

namespace Caddx_PCsoftware;

public class MainController
{
	public bool InitMainCtrl()
	{
		try
		{
			InitAllModel();
			InitComponents();
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private void InitAllModel()
	{
	}

	private void InitComponents()
	{
	}
}
