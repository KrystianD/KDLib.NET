using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace KDLib.SimpleConfig
{
  public class YamlSimpleConfig : BaseSimpleConfig
  {
    private readonly YamlMappingNode _node;

    public override string ConfigDirectory { get; }

    private YamlSimpleConfig(string directoryPath, YamlMappingNode node)
    {
      ConfigDirectory = directoryPath;
      _node = node;
    }

    public static YamlSimpleConfig FromFile(string path)
    {
      var node = CreateYamlMappingNodeFromString(File.ReadAllText(path));
      return new YamlSimpleConfig(Path.GetDirectoryName(Path.GetFullPath(path)), node);
    }

    public static YamlSimpleConfig FromString(string yamlString)
    {
      var node = CreateYamlMappingNodeFromString(yamlString);
      return new YamlSimpleConfig(null, node);
    }

    public static YamlSimpleConfig FromStringDir(string yamlString, string dir)
    {
      var node = CreateYamlMappingNodeFromString(yamlString);
      return new YamlSimpleConfig(dir, node);
    }

    public override bool TryGetOption<T>(string path, out T val)
    {
      val = default;

      YamlNode node = GetConfigNode(path);
      if (node == null)
        return false;

      val = ConvertNodeToObject<T>(node);
      return true;
    }

    private YamlNode GetConfigNode(string path)
    {
      var curNode = (YamlNode) _node;
      foreach (var p in path.Split('.')) {
        if (curNode is YamlMappingNode n) {
          if (!n.Children.TryGetValue(p, out curNode))
            return null;
        }
        else {
          throw new ConfigInvalidPathException($"Unknown node path /{path}/");
        }
      }

      return curNode;
    }

    private static T ConvertNodeToObject<T>(YamlNode node)
    {
      var yamlStr = new YamlDotNet.Serialization.Serializer().Serialize(node);

      using (var tr = new StringReader(yamlStr)) {
        try {
          return new YamlDotNet.Serialization.Deserializer().Deserialize<T>(tr);
        }
        catch (YamlException) {
          throw new InvalidCastException($"Invalid type: /{yamlStr}/ expected: {typeof(T).Name}");
        }
      }
    }

    private static YamlMappingNode CreateYamlMappingNodeFromString(string yamlStr)
    {
      var yaml = new YamlStream();
      using (var reader = new StringReader(yamlStr)) {
        yaml.Load(reader);
        return (YamlMappingNode) yaml.Documents[0].RootNode;
      }
    }
  }
}