#if NET5_0
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace KDLib
{
  public class SubprocessBuilder
  {
    private readonly ProcessStartInfo _psi = new ProcessStartInfo();

    private Action<string> _standardOutputCallbackLine;
    private Action<string> _standardErrorCallbackLine;

    public SubprocessBuilder(string executable)
    {
      _psi.FileName = executable;
    }

    public SubprocessBuilder WorkingDirectory(string path)
    {
      _psi.WorkingDirectory = path;
      return this;
    }

    public SubprocessBuilder Argument(params string[] values)
    {
      foreach (var value in values)
        _psi.ArgumentList.Add(value);
      return this;
    }

    public SubprocessBuilder Argument(string value)
    {
      _psi.ArgumentList.Add(value);
      return this;
    }

    public SubprocessBuilder Argument(string value1, string value2)
    {
      _psi.ArgumentList.Add(value1);
      _psi.ArgumentList.Add(value2);
      return this;
    }

    public SubprocessBuilder Argument(string value1, int value2)
    {
      _psi.ArgumentList.Add(value1);
      _psi.ArgumentList.Add(value2.ToString());
      return this;
    }

    public SubprocessBuilder RedirectStandardOutputLine(Action<string> callback)
    {
      _standardOutputCallbackLine = callback;
      _psi.RedirectStandardOutput = true;
      return this;
    }

    public SubprocessBuilder RedirectStandardErrorLine(Action<string> callback)
    {
      _standardErrorCallbackLine = callback;
      _psi.RedirectStandardError = true;
      return this;
    }

    private static async void ReadLinesToCallback(StreamReader s, Action<string> callback)
    {
      while (true) {
        var line = await s.ReadLineAsync();
        if (line == null)
          break;
        callback(line.Trim());
      }
    }

    public string GetEscapedCommand()
    {
      return Enumerable.Repeat(_psi.FileName, 1)
                       .Concat(_psi.ArgumentList)
                       .Select(x => ShellUtils.EncodeParameterArgument(x, false)).JoinString(" ");
    }

    public Process Run()
    {
      var pr = Process.Start(_psi);

      if (pr == null)
        throw new InvalidOperationException("Process is null");

      if (_standardOutputCallbackLine != null)
        ReadLinesToCallback(pr.StandardOutput, _standardOutputCallbackLine);
      if (_standardErrorCallbackLine != null)
        ReadLinesToCallback(pr.StandardError, _standardErrorCallbackLine);

      return pr;
    }
  }

  public static class Subprocess
  {
    public static SubprocessBuilder Call(string executable, params string[] arguments)
    {
      return new SubprocessBuilder(executable)
          .Argument(arguments);
    }
  }
}
#endif