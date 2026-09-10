using System.Collections.Generic;
using Newtonsoft.Json;

namespace Caddx_PCTool;

public class CamHubData
{
	public NodeInfo node0 { get; set; }

	public NodeInfo node1 { get; set; }

	public NodeInfo node2 { get; set; }

	public NodeInfo node3 { get; set; }

	public string fly_ctrl { get; set; }

	[JsonIgnore]
	public List<NodeInfo> NodeList { get; set; }

	public CamHubData()
	{
		node0 = new NodeInfo();
		node1 = new NodeInfo();
		node2 = new NodeInfo();
		node3 = new NodeInfo();
		fly_ctrl = "betaFlight";
		NodeList = new List<NodeInfo> { node0, node1, node2, node3 };
	}
}
