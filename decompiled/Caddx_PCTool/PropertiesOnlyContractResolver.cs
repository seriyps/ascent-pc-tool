using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Caddx_PCTool;

public class PropertiesOnlyContractResolver : DefaultContractResolver
{
	protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (member is PropertyInfo)
		{
			return ((DefaultContractResolver)this).CreateProperty(member, memberSerialization);
		}
		return null;
	}
}
