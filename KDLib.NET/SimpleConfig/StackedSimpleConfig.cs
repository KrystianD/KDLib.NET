using System;
using System.Collections.Generic;
using System.Linq;

namespace KDLib.SimpleConfig
{
  public class StackedSimpleConfig : BaseSimpleConfig
  {
    private readonly List<(string, ISimpleConfig)> _innerConfigs = new List<(string, ISimpleConfig)>();

    public override string ConfigDirectory => throw new InvalidOperationException();

    public void AddConfig(ISimpleConfig cfg) => AddConfig("", cfg);

    public void AddConfig(string prefix, ISimpleConfig cfg)
    {
      if (cfg is StackedSimpleConfig)
        throw new ArgumentException();
      prefix = prefix ?? "";
      if (prefix.Length > 0)
        prefix += ".";
      _innerConfigs.Add((prefix, cfg));
    }

    public override string GetOptionAsPath(string path)
    {
      if (!TryGetOption<string>(path, out var val, out var cfg))
        throw new ConfigOptionNotExistsException($"No config option {path}");

      return CreateFilePath(cfg.ConfigDirectory, val);
    }

    public override string GetOptionAsPath(string path, string def)
    {
      if (!TryGetOption<string>(path, out var val, out var cfg))
        return def;

      return CreateFilePath(cfg.ConfigDirectory, val);
    }

    public override bool TryGetOption<T>(string path, out T val)
    {
      return TryGetOption(path, out val, out _);
    }

    private bool TryGetOption<T>(string path, out T val, out ISimpleConfig cfg)
    {
      foreach (var (prefix, config) in _innerConfigs.AsEnumerable().Reverse()) {
        if (config.TryGetOption(prefix + path, out val)) {
          cfg = config;
          return true;
        }
      }

      cfg = null;
      val = default;
      return false;
    }
  }
}