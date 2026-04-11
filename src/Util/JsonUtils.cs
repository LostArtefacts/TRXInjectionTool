using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace TRXInjectionTool.Util;

public static class JsonUtils
{
    public static T DeserializeFile<T>(string path)
    {
        return Deserialize<T>(File.ReadAllText(path));
    }

    public static T Deserialize<T>(string data)
    {
        return JsonConvert.DeserializeObject<T>(data);
    }

    public static void Serialize(object data, string filePath)
    {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        var formatted = Regex.Replace(json,
            @"\{\s+([^{}]+?)\s+\}",
            m => "{ " + Regex.Replace(m.Groups[1].Value, @"\s+", " ") + " }");
        formatted += Environment.NewLine;
        File.WriteAllText(filePath, formatted);
    }
}
