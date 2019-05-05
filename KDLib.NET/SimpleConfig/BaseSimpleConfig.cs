using System;
using System.IO;

namespace KDLib.SimpleConfig
{
  public abstract class BaseSimpleConfig : ISimpleConfig
  {
    public abstract string ConfigDirectory { get; }

    public abstract bool TryGetOption<T>(string path, out T val);

    public static readonly ISimpleConfig Empty = new EmptySimpleConfig();

    public T GetOption<T>(string path)
    {
      if (!TryGetOption(path, out T val))
        throw new ConfigOptionNotExistsException($"No config option {path}");
      return val;
    }

    public T GetOption<T>(string path, T def)
    {
      return TryGetOption(path, out T val) ? val : def;
    }

    public virtual string GetOptionAsPath(string path)
    {
      return CreateFilePath(ConfigDirectory, GetOption<string>(path));
    }

    public virtual string GetOptionAsPath(string path, string def)
    {
      if (!TryGetOption<string>(path, out var val))
        return def;

      return CreateFilePath(ConfigDirectory, val);
    }

    protected static string CreateFilePath(string dir, string relativePath)
    {
      if (dir == null)
        throw new InvalidOperationException("cannot use CreateFilePath on non-file based config");

      if (relativePath.StartsWith("/"))
        return relativePath;
      else
        return Path.GetFullPath(Path.Combine(dir, relativePath));
    }
  }
}