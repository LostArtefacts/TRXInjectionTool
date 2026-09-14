namespace TRXInjectionTool.Control;

public static class InjectionIO
{
    private static readonly InjectionExporter _exporter = new();

    public static void Export(InjectionData data, string file)
    {
        ((IInjectionExporter)_exporter).Export(data, file);
    }

    public static byte[] Serialize(InjectionData data)
    {
        return _exporter.Serialize(data);
    }
}
