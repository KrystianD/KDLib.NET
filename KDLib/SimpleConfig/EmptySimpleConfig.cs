using System;

namespace KDLib.SimpleConfig
{
  public class EmptySimpleConfig : BaseSimpleConfig
  {
    public override string ConfigDirectory => throw new InvalidOperationException();

    internal EmptySimpleConfig() { }

    public override bool TryGetOption<T>(string path, out T val)
    {
      val = default;
      return false;
    }
  }
}