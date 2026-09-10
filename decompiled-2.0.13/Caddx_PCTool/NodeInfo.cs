namespace Caddx_PCTool;

public class NodeInfo
{
	public string dev_name { get; set; }

	public string res_info { get; set; }

	public int selected { get; set; }

	public int high_priority { get; set; }

	public NodeInfo()
	{
		dev_name = "none";
		res_info = "1920x1080P60";
		selected = 0;
		high_priority = 0;
	}
}
