using Parsec.Serialization;
using Parsec.Shaiya.Core;

namespace Parsec.Shaiya.Env;

public struct EnvColor : ISerializable
{
    public int Red { get; set; }

    public int Green { get; set; }

    public int Blue { get; set; }

    public void Read(SBinaryReader binaryReader)
    {
        Red = binaryReader.ReadInt32();
        Green = binaryReader.ReadInt32();
        Blue = binaryReader.ReadInt32();
    }

    public void Write(SBinaryWriter binaryWriter)
    {
        binaryWriter.Write(Red);
        binaryWriter.Write(Green);
        binaryWriter.Write(Blue);
    }
}
