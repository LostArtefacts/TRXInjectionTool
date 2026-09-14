namespace TRXInjectionTool.Control;

public interface IInjectionExporter
{
    byte[] Serialize(InjectionData data);

    void Export(InjectionData data, string file)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(file)));
        File.WriteAllBytes(file, Serialize(data));
    }
}
