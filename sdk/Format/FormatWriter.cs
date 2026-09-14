using System.Reflection;
using System.Text;

namespace TRXInjectionTool.Format;

// Serializes format DTOs (sdk/Format) by reflection, following the same
// attribute rules the doc generator renders — one source of truth.
public static class FormatWriter
{
    public static void Write(BinaryWriter writer, object dto)
    {
        var type = dto.GetType();
        if (type.GetCustomAttribute<CaseAttribute>() is { } caseAttr)
        {
            var union = UnionBaseOf(type);
            WriteScalar(writer, union.GetCustomAttribute<UnionAttribute>(inherit: false).DiscriminatorType,
                caseAttr.Discriminator);
            WriteFields(writer, dto, union); // common header
            WriteFields(writer, dto, type);  // case payload
        }
        else
        {
            WriteFields(writer, dto, type);
        }
    }

    private static Type UnionBaseOf(Type type)
    {
        for (var t = type.BaseType; t != null; t = t.BaseType)
        {
            if (t.GetCustomAttribute<UnionAttribute>(inherit: false) != null)
            {
                return t;
            }
        }
        throw new InvalidDataException($"{type.Name} has [Case] but no [Union] base");
    }

    private static void WriteFields(BinaryWriter writer, object dto, Type declaringLevel)
    {
        var fields = declaringLevel
            .GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .OrderBy(f => f.MetadataToken);
        foreach (var field in fields)
        {
            WriteField(writer, field, field.GetValue(dto));
        }
    }

    private static void WriteField(BinaryWriter writer, FieldInfo field, object value)
    {
        var type = field.FieldType;
        if (type.IsArray)
        {
            var array = (Array)value ?? Array.Empty<object>();
            if (field.GetCustomAttribute<LengthPrefixAttribute>() is { } prefix)
            {
                WriteScalar(writer, prefix.CountType, array.Length);
            }
            else if (field.GetCustomAttribute<FixedLengthAttribute>() is { } fixedLen)
            {
                if (array.Length != fixedLen.Count)
                {
                    throw new InvalidDataException(
                        $"{field.DeclaringType.Name}.{field.Name}: expected {fixedLen.Count} elements, got {array.Length}");
                }
            }
            else if (field.GetCustomAttribute<ImpliedLengthAttribute>() == null)
            {
                throw new InvalidDataException(
                    $"{field.DeclaringType.Name}.{field.Name}: array without a length attribute");
            }
            foreach (var element in array)
            {
                WriteValue(writer, element);
            }
            return;
        }

        WriteValue(writer, value);
    }

    private static void WriteValue(BinaryWriter writer, object value)
    {
        switch (value)
        {
            case byte b: writer.Write(b); return;
            case sbyte sb: writer.Write(sb); return;
            case short s: writer.Write(s); return;
            case ushort us: writer.Write(us); return;
            case int i: writer.Write(i); return;
            case uint ui: writer.Write(ui); return;
            case float f: writer.Write(f); return;
            case double d: writer.Write(d); return;
            case string text:
                var bytes = Encoding.UTF8.GetBytes(text ?? string.Empty);
                writer.Write(bytes.Length);
                writer.Write(bytes);
                return;
            case null:
                throw new InvalidDataException("null value in a non-array format field");
            default:
                Write(writer, value); // nested record, primitive struct, or union case
                return;
        }
    }

    private static void WriteScalar(BinaryWriter writer, Type type, int value)
    {
        if (type == typeof(byte)) writer.Write((byte)value);
        else if (type == typeof(short)) writer.Write((short)value);
        else if (type == typeof(ushort)) writer.Write((ushort)value);
        else if (type == typeof(int)) writer.Write(value);
        else if (type == typeof(uint)) writer.Write((uint)value);
        else throw new InvalidDataException($"unsupported count/discriminator type {type.Name}");
    }
}
