namespace KDLib.SimpleConfig
{
  public interface ISimpleConfig
  {
    string ConfigDirectory { get; }

    T GetOption<T>(string path);
    T GetOption<T>(string path, T def);
    string GetOptionAsPath(string path);
    string GetOptionAsPath(string path, string def);
    bool TryGetOption<T>(string path, out T val);
  }
}