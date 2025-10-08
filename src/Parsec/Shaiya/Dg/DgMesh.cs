using Parsec.Extensions;
using Parsec.Serialization;
using Parsec.Shaiya.Common;
using Parsec.Shaiya.Core;

namespace Parsec.Shaiya.Dg;

public class DgMesh : ISerializable
{
    public int LightmapIndex { get; set; }

    public List<DgMeshVertex> Vertices { get; set; } = new();

    public List<MeshFace> Faces { get; set; } = new();

    public void Read(SBinaryReader binaryReader)
    {
        LightmapIndex = binaryReader.ReadInt32();
        Vertices = binaryReader.ReadList<DgMeshVertex>().ToList();
        Faces = binaryReader.ReadList<MeshFace>().ToList();
    }

    public void Write(SBinaryWriter binaryWriter)
    {
        binaryWriter.Write(LightmapIndex);
        binaryWriter.Write(Vertices.ToSerializable());
        binaryWriter.Write(Faces.ToSerializable());
    }
}
