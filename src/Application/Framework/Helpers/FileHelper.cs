﻿using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FractalViewer.Application.Helpers;
public static class FileHelper {

	public static string GetUniqueFileName(string path, string fileName, string fileExtension) {
		if (fileExtension[0] != '.') {
			fileExtension = "." + fileExtension;
		}

		string uniqueName = fileName;
		int index = 0;

		while (File.Exists(Path.Combine(path, uniqueName + fileExtension))) {
			index++;
			uniqueName = fileName + index;
		}
		return uniqueName + fileExtension;
	}

	public static string GetResourcePath(params string[] localPath) {
		return Path.Combine(Directory.GetCurrentDirectory(), "resources", Path.Combine(localPath));
	}

	public static string ReadResourceFile(string localPath) {
		return File.ReadAllText(GetResourcePath(localPath));
	}

	public static void WriteResourcePath(string localPath, string data) {
		WriteResourcePath(localPath, data.Split("\n"));
	}
	public static void WriteResourcePath(string localPath, string[] data) {
		using (StreamWriter writer = new StreamWriter(FileHelper.GetResourcePath(localPath))) {
			foreach (string line in data) {
				writer.WriteLine(line);
			}
		}
	}

	// Thanks to https://github.com/dotnet/runtime/issues/17938
	public static void OpenUrl(string url) {
		try {
			Process.Start(url);
		}
		catch {
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
				url = url.Replace("&", "^&");
				Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
				Process.Start("xdg-open", url);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
				Process.Start("open", url);
			}
			else {
				throw;
			}
		}
	}
}