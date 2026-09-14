namespace TRXInjectionTool.Format;

// The TRXI format is declared as plain DTOs: public fields in declaration
// order are the binary layout, and a field's CLR type is its binary type
// type (byte=u8, short=s16, ushort=u16, int=s32, uint=u32, float=f32,
// double=f64; string = s32 byte length + UTF-8). The serializer and the
// format documentation are both produced by reflection over these types, so
// neither can drift from the other.

// Marks a type whose fields form a format record.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class FormatRecordAttribute : Attribute
{
    // Free-form remark rendered with the record in the generated docs.
    public string Doc { get; init; }
}

// Free-form remark rendered with the field in the generated docs.
[AttributeUsage(AttributeTargets.Field)]
public sealed class DocAttribute(string text) : Attribute
{
    public string Text { get; } = text;
}

// Array field whose element count is written before the elements, using the
// given scalar type. The count is taken from the array's length.
[AttributeUsage(AttributeTargets.Field)]
public sealed class LengthPrefixAttribute(Type countType) : Attribute
{
    public Type CountType { get; } = countType;
}

// Array field with a fixed element count; nothing is written for the count.
[AttributeUsage(AttributeTargets.Field)]
public sealed class FixedLengthAttribute(int count) : Attribute
{
    public int Count { get; } = count;
}

// Array field whose element count is not stored; the note states how to derive it.
[AttributeUsage(AttributeTargets.Field)]
public sealed class ImpliedLengthAttribute(string note) : Attribute
{
    public string Note { get; } = note;
}

// Marks the abstract base of a discriminated union. The discriminator is
// written first with the given scalar type, then the base's own fields (the
// common header), then the matching case's fields.
[AttributeUsage(AttributeTargets.Class)]
public sealed class UnionAttribute(Type discriminatorType) : Attribute
{
    public Type DiscriminatorType { get; } = discriminatorType;
    public string Doc { get; init; }
}

// A union case: subclass of a [Union] base, selected by this discriminator.
[AttributeUsage(AttributeTargets.Class)]
public sealed class CaseAttribute(int discriminator) : Attribute
{
    public int Discriminator { get; } = discriminator;
}

// Binds a record to a block type within a chunk.
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class BlockAttribute(int chunkType, int blockType, string name) : Attribute
{
    public int ChunkType { get; } = chunkType;
    public int BlockType { get; } = blockType;
    public string Name { get; } = name;
    public string Doc { get; init; }
}

// Binds a record to an applicability test type.
[AttributeUsage(AttributeTargets.Class)]
public sealed class ApplicabilityAttribute(int testType, string name) : Attribute
{
    public int TestType { get; } = testType;
    public string Name { get; } = name;
    public int Version { get; init; } = 1;
    public string Doc { get; init; }
}
