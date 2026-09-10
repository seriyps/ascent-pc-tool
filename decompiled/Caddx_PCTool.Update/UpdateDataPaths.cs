using System;
using System.Collections.Generic;
using System.IO;

namespace Caddx_PCTool.Update;

public sealed class UpdateDataPaths
{
	public string DataDirectory { get; }

	public UpdateDataPaths(string localAppDataRoot, VariantUpdateConfig config)
	{
		if (config == null)
		{
			throw new ArgumentNullException("config");
		}
		if (string.IsNullOrWhiteSpace(localAppDataRoot))
		{
			throw new ArgumentException("localAppDataRoot");
		}
		DataDirectory = Path.Combine(localAppDataRoot, "Caddx", config.DataFolderName, "Data");
	}

	public int RestoreMissingSeedFiles(string seedRootDir, IEnumerable<string> relativeFiles, Action<string> log)
	{
		if (relativeFiles == null)
		{
			throw new ArgumentNullException("relativeFiles");
		}
		log = log ?? ((Action<string>)delegate
		{
		});
		int num = 0;
		foreach (string relativeFile in relativeFiles)
		{
			string text = Path.Combine(DataDirectory, relativeFile);
			if (!File.Exists(text))
			{
				string text2 = Path.Combine(seedRootDir, relativeFile);
				if (!File.Exists(text2))
				{
					log("种子文件缺失，跳过恢复：" + relativeFile);
					continue;
				}
				Directory.CreateDirectory(Path.GetDirectoryName(text));
				File.Copy(text2, text, overwrite: false);
				num++;
				log("已恢复缺失文件：" + relativeFile);
			}
		}
		return num;
	}
}
