using Parsec.Serialization;
using Parsec.Shaiya.Core;

namespace Parsec.Shaiya.Env;

public sealed class EnvRecord : ISerializable
{
    public int Unknown1 { get; set; }

    public int Unknown2 { get; set; }

    public EnvColor Color1 { get; set; }

    public EnvColor Color2 { get; set; }

    public EnvColor Color3 { get; set; }

    public string SkyName1 { get; set; } = "empty.tga";

    public string SkyName2 { get; set; } = "empty.tga";

    public string SkyName3 { get; set; } = "empty.tga";

    public string SkyName4 { get; set; } = "empty.tga";

    public float Unknown12 { get; set; }

    public float Unknown13 { get; set; }

    public float Unknown14 { get; set; }

    public float Unknown15 { get; set; }

    public float Unknown16 { get; set; }

    public float Unknown17 { get; set; }

    public byte UnknownByte18 { get; set; }

    public int Unknown19 { get; set; }

    public void Read(SBinaryReader binaryReader)
    {
        Unknown1 = binaryReader.ReadInt32();
        Unknown2 = binaryReader.ReadInt32();
        Color1 = binaryReader.Read<EnvColor>();
        Color2 = binaryReader.Read<EnvColor>();
        Color3 = binaryReader.Read<EnvColor>();
        SkyName1 = binaryReader.ReadString();
        SkyName2 = binaryReader.ReadString();
        SkyName3 = binaryReader.ReadString();
        SkyName4 = binaryReader.ReadString();
        Unknown12 = binaryReader.ReadSingle();
        Unknown13 = binaryReader.ReadSingle();
        Unknown14 = binaryReader.ReadSingle();
        Unknown15 = binaryReader.ReadSingle();
        Unknown16 = binaryReader.ReadSingle();
        Unknown17 = binaryReader.ReadSingle();
        UnknownByte18 = binaryReader.ReadByte();
        Unknown19 = binaryReader.ReadInt32();
    }

    public void Write(SBinaryWriter binaryWriter)
    {
        binaryWriter.Write(Unknown1);
        binaryWriter.Write(Unknown2);
        binaryWriter.Write(Color1);
        binaryWriter.Write(Color2);
        binaryWriter.Write(Color3);
        binaryWriter.Write(SkyName1, includeStringTerminator: false);
        binaryWriter.Write(SkyName2, includeStringTerminator: false);
        binaryWriter.Write(SkyName3, includeStringTerminator: false);
        binaryWriter.Write(SkyName4, includeStringTerminator: false);
        binaryWriter.Write(Unknown12);
        binaryWriter.Write(Unknown13);
        binaryWriter.Write(Unknown14);
        binaryWriter.Write(Unknown15);
        binaryWriter.Write(Unknown16);
        binaryWriter.Write(Unknown17);
        binaryWriter.Write(UnknownByte18);
        binaryWriter.Write(Unknown19);
    }
}
