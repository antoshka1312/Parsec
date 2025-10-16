using Parsec.Extensions;
using Parsec.Serialization;
using Parsec.Shaiya.Core;

namespace Parsec.Shaiya.Env;

public sealed class Env : FileBase
{
    private const int HeaderLength = 2;

    public string Header { get; set; } = "V2";

    public List<EnvRecord> Records { get; set; } = new();

    public override string Extension { get; } = "env";

    protected override void Read(SBinaryReader binaryReader)
    {
        Header = binaryReader.ReadString(HeaderLength);
        Records = binaryReader.ReadList<EnvRecord>().ToList();
    }

    protected override void Write(SBinaryWriter binaryWriter)
    {
        binaryWriter.Write(Header.Take(HeaderLength).ToString(), isLengthPrefixed: false, includeStringTerminator: false);
        binaryWriter.Write(Records.ToSerializable());
    }
}
