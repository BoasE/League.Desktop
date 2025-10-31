using System.Diagnostics;
using System.Text;

namespace BE.League.Desktop.Connection;

public static class ClientHelper
{
    public sealed class LockInfo
    {
        public string Name { get; set; } = "";
        public int Port { get; set; }
        public string Password { get; set; } = "";
        public string Protocol { get; set; } = "https";
    }

    public static string? ResolveLockfilePath(string? overridePath = null)
    {
        if (!string.IsNullOrWhiteSpace(overridePath) && File.Exists(overridePath))
            return overridePath;

        try
        {
            var candidates = new[] { "LeagueClientUx", "LeagueClient" };
            foreach (var p in Process.GetProcesses()
                         .Where(p => candidates.Contains(p.ProcessName, StringComparer.OrdinalIgnoreCase)))
            {
                try
                {
                    var exe = p.MainModule?.FileName;
                    if (string.IsNullOrWhiteSpace(exe)) continue;
                    var dir = Path.GetDirectoryName(exe)!;
                    var lf = Path.Combine(dir, "lockfile");
                    if (File.Exists(lf)) return lf;
                    var up = Directory.GetParent(dir)?.FullName;
                    if (up != null && File.Exists(Path.Combine(up, "lockfile")))
                        return Path.Combine(up, "lockfile");
                }
                catch
                {
                }
            }
        }
        catch
        {
        }

        foreach (var g in new[]
                     { @"C:\Riot Games\League of Legends\lockfile", @"D:\Riot Games\League of Legends\lockfile" })
            if (File.Exists(g))
                return g;

        var env = Environment.GetEnvironmentVariable("LEAGUE_LOCKFILE");
        return (!string.IsNullOrWhiteSpace(env) && File.Exists(env)) ? env : null;
    }

    public static LockInfo? ReadLockfile(string path)
    {
        try
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, Encoding.UTF8, true);
            var t = sr.ReadToEnd().Trim();
            var parts = t.Split(':');
            if (parts.Length < 5) parts = t.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 5) return null;
            return new LockInfo
            {
                Name = parts[0],
                Port = int.Parse(parts[2]),
                Password = parts[3],
                Protocol = parts[4].StartsWith("http") ? parts[4] : "https"
            };
        }
        catch
        {
            return null;
        }
    }
}