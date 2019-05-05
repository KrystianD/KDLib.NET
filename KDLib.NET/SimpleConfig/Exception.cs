using System;

namespace KDLib.SimpleConfig
{
  public class ConfigInvalidPathException : Exception
  {
    public ConfigInvalidPathException(string message) : base(message)
    {
    }
  }

  public class ConfigOptionNotExistsException : Exception
  {
    public ConfigOptionNotExistsException(string message) : base(message)
    {
    }
  }
}