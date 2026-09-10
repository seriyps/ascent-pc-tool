namespace Caddx_PCTool;

public class ApiVersionResult
{
	public byte ProtocolVersion { get; set; }

	public byte MajorVersion { get; set; }

	public byte MinorVersion { get; set; }

	public override string ToString()
	{
		return $"Protocol: {ProtocolVersion}, Version: {MajorVersion}.{MinorVersion}";
	}
}
