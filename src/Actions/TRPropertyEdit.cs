using System.Text;
using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Actions;

public enum TRPropertyTarget
{
    Object,
    Item,
}

public enum TRPropertyType
{
    Int,
    Float,
    Double,
    Bool,
    XYZ,
}

public abstract class TRPropertyEdit
{
    public abstract TRPropertyTarget Type { get; }
    public List<TRProperty> Properties { get; set; } = [];

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write((int)Type);
        SerializeImpl(writer, version);
        writer.Write(Properties.Count);
        Properties.ForEach(p => p.Serialize(writer));
    }

    protected abstract void SerializeImpl(TRLevelWriter writer, TRGameVersion version);
}

public class TRObjectPropertyEdit : TRPropertyEdit
{
    public override TRPropertyTarget Type => TRPropertyTarget.Object;
    public int ObjectId { get; set; }
    public TRObjectType ObjectType { get; set; } = TRObjectType.Game;

    protected override void SerializeImpl(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write(ObjectId, ObjectType, version);
    }
}

public class TRItemPropertyEdit : TRPropertyEdit
{
    public override TRPropertyTarget Type => TRPropertyTarget.Item;
    public int ItemIndex { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write(ItemIndex);
    }
}

public abstract class TRProperty
{
    public abstract TRPropertyType Type { get; }
    public required string Name { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        var name = Encoding.UTF8.GetBytes(Name);
        writer.Write(name.Length);
        writer.Write(name);
        writer.Write((int)Type);
        SerializeImpl(writer);
    }

    protected abstract void SerializeImpl(TRLevelWriter writer);
}

public class TRBoolProperty : TRProperty
{
    public override TRPropertyType Type => TRPropertyType.Bool;
    public bool Value { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        writer.Write(Value ? 1 : 0);
    }
}

public class TRDoubleProperty : TRProperty
{
    public override TRPropertyType Type => TRPropertyType.Double;
    public double Value { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        writer.Write(Value);
    }
}

public class TRFloatProperty : TRProperty
{
    public override TRPropertyType Type => TRPropertyType.Float;
    public float Value { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        writer.Write(Value);
    }
}

public class TRIntProperty : TRProperty
{
    public override TRPropertyType Type => TRPropertyType.Int;
    public int Value { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        writer.Write(Value);
    }
}

public class TRXYZProperty : TRProperty
{
    public override TRPropertyType Type => TRPropertyType.XYZ;
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    protected override void SerializeImpl(TRLevelWriter writer)
    {
        writer.Write(X);
        writer.Write(Y);
        writer.Write(Z);
    }
}
