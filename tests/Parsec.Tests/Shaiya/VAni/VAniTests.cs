﻿namespace Parsec.Tests.Shaiya.VAni;

public class VAniTests
{
    [Theory]
    [InlineData("A1_butterfly01.VANI")]
    [InlineData("A1_Green_ani00.VANI")]
    [InlineData("AB7_dun2_femalebeeBUD.VANI")]
    [InlineData("AB7_UReggStoNail01b.VANI")]
    [InlineData("B2_grass_Ani01.VANI")]
    [InlineData("D_Deadbody_01.VANI")]
    [InlineData("L_A1_Ferry_Ship.VANI")]
    [InlineData("R1_Green_ani06.VANI")]
    [InlineData("R2_FireStone.VANI")]
    public void VaniMultipleReadWriteTest(string fileName)
    {
        string filePath = $"Shaiya/VAni/{fileName}";
        string jsonPath = $"Shaiya/VAni/{fileName}.json";
        string newObjPath = $"Shaiya/VAni/new_{fileName}";

        var vani = ParsecReader.FromFile<Parsec.Shaiya.Vani.Vani>(filePath);
        vani.WriteJson(jsonPath);
        var vaniFromJson = ParsecReader.FromJsonFile<Parsec.Shaiya.Vani.Vani>(jsonPath);

        // Check bytes
        Assert.Equal(vani.GetBytes(), vaniFromJson.GetBytes());

        vaniFromJson.Write(newObjPath);
        var newVani = ParsecReader.FromFile<Parsec.Shaiya.Vani.Vani>(newObjPath);

        // Check bytes
        Assert.Equal(vani.GetBytes(), newVani.GetBytes());
    }
}
