namespace Caddx_PCTool;

public class SlotData
{
	public int slot0 { get; set; }

	public int slot0_min { get; set; }

	public int slot0_max { get; set; }

	public int slot1 { get; set; }

	public int slot1_min { get; set; }

	public int slot1_max { get; set; }

	public int slot2 { get; set; }

	public int slot2_min { get; set; }

	public int slot2_max { get; set; }

	public int slot3 { get; set; }

	public int slot3_min { get; set; }

	public int slot3_max { get; set; }

	public SlotData()
	{
		slot0 = 0;
		slot0_min = 0;
		slot0_max = 0;
		slot1 = 0;
		slot1_min = 0;
		slot1_max = 0;
	}

	public SlotData(SlotData obj)
	{
		if (obj != null)
		{
			slot0 = obj.slot0;
			slot0_min = obj.slot0_min;
			slot0_max = obj.slot0_max;
			slot1 = obj.slot1;
			slot1_min = obj.slot1_min;
			slot1_max = obj.slot1_max;
			slot2 = obj.slot2;
			slot2_min = obj.slot2_min;
			slot2_max = obj.slot2_max;
			slot3 = obj.slot3;
			slot3_min = obj.slot3_min;
			slot3_max = obj.slot3_max;
		}
	}
}
