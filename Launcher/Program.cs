using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;

namespace FaceCompare.Launcher;

internal static class Program
{
    private static void Main(string[] args)
    {
        try
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var targetDir = Path.Combine(appData, "FaceCompareApp");
            var exePath = Path.Combine(targetDir, "FaceCompare.exe");

            var assembly = Assembly.GetExecutingAssembly();
            var processPath = Environment.ProcessPath ?? AppContext.BaseDirectory;
            var lastWrite = File.Exists(processPath) ? File.GetLastWriteTimeUtc(processPath).Ticks : 1L;
            var markerFile = Path.Combine(targetDir, ".version");

            var needsExtraction = !File.Exists(exePath) ||
                                  !File.Exists(markerFile) ||
                                  File.ReadAllText(markerFile).Trim() != lastWrite.ToString();

            if (needsExtraction)
            {
                if (Directory.Exists(targetDir))
                {
                    try { Directory.Delete(targetDir, true); } catch { }
                }
                Directory.CreateDirectory(targetDir);

                var resourceNames = assembly.GetManifestResourceNames();
                var resName = resourceNames.FirstOrDefault(n => n.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                              ?? (resourceNames.Length > 0 ? resourceNames[0] : null);

                if (resName != null)
                {
                    using var stream = assembly.GetManifestResourceStream(resName);
                    if (stream != null)
                    {
                        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
                        archive.ExtractToDirectory(targetDir, overwriteFiles: true);
                    }
                }

                File.WriteAllText(markerFile, lastWrite.ToString());
            }

            if (!File.Exists(exePath))
            {
                NativeMessageBox("Error: FaceCompare executable could not be extracted.");
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = targetDir,
                UseShellExecute = true
            };

            foreach (var arg in args)
            {
                startInfo.ArgumentList.Add(arg);
            }

            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            NativeMessageBox("Failed to launch FaceCompare: " + ex.Message);
        }
    }

    [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "MessageBoxW", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    private static void NativeMessageBox(string text)
    {
        MessageBox(IntPtr.Zero, text, "FaceCompare Error", 0x10);
    }
}
