using System;
using System.Collections.Specialized;

namespace Caddx_PCTool.Update;

public sealed class VariantUpdateConfig
{
	public ProductVariant Variant { get; }

	public string PackId { get; }

	public string Channel { get; }

	public string UpdateUrl { get; }

	public string DataFolderName { get; }

	public string Runtime { get; }

	private VariantUpdateConfig(ProductVariant variant, string packId, string channel, string dataFolderName, string updateUrl)
	{
		Variant = variant;
		PackId = packId;
		Channel = channel;
		DataFolderName = dataFolderName;
		UpdateUrl = updateUrl;
		Runtime = "win-x64";
	}

	public static VariantUpdateConfig FromValues(string variant, string updateUrl)
	{
		if (string.IsNullOrWhiteSpace(updateUrl))
		{
			throw new ArgumentException("更新 URL 不能为空", "updateUrl");
		}
		string text = (variant ?? string.Empty).Trim().ToUpperInvariant();
		string text2 = text;
		if (!(text2 == "B"))
		{
			if (text2 == "C")
			{
				return new VariantUpdateConfig(ProductVariant.C, "CaddxPCTool.C", "win-c", "PC Tool_C", updateUrl);
			}
			throw new ArgumentException("未知产品变体：" + variant, "variant");
		}
		return new VariantUpdateConfig(ProductVariant.B, "CaddxPCTool.B", "win-b", "PC Tool_B", updateUrl);
	}

	public static VariantUpdateConfig FromAppConfig(NameValueCollection appSettings)
	{
		if (appSettings == null)
		{
			throw new ArgumentNullException("appSettings");
		}
		return FromValues(appSettings["UpdateProductVariant"], appSettings["UpdateUrl"]);
	}
}
