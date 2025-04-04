using System.IO;
using System.Linq;
using Parsec.Common;
using Parsec.Extensions;
using Parsec.Serialization;
using Parsec.Shaiya.Cash;

namespace Parsec.Tests.Shaiya.Cash;

public class CashTests
{
    [Fact]
    public void CashTest()
    {
        const string filePath = "Shaiya/Cash/Cash.SData";
        const string outputPath = "Shaiya/Cash/Cash_new.SData";

        var cash = ParsecReader.FromFile<Parsec.Shaiya.Cash.Cash>(filePath);
        cash.Write(outputPath);
        var newCash = ParsecReader.FromFile<Parsec.Shaiya.Cash.Cash>(outputPath);
        Assert.Equal(cash.GetBytes(), newCash.GetBytes());
        Assert.Equal(FileHash.Checksum(filePath), FileHash.Checksum(outputPath));
    }

    [Fact]
    public void DbItemSellTest()
    {
        const string filePath = "Shaiya/Cash/DBItemSellData.SData";
        const string outputPath = "Shaiya/Cash/output_DBItemSellData.SData";
        const string jsonPath = "Shaiya/Cash/DBItemSellData.SData.json";
        const string csvPath = "Shaiya/Cash/DBItemSellData.SData.csv";

        var dbItemSell = ParsecReader.FromFile<DBItemSellData>(filePath);
        dbItemSell.Write(outputPath);
        dbItemSell.WriteJson(jsonPath);
        dbItemSell.WriteCsv(csvPath);

        var outputDbItemSell = ParsecReader.FromFile<DBItemSellData>(outputPath);
        var jsonDbItemSell = ParsecReader.FromJsonFile<DBItemSellData>(jsonPath);
        var csvItemSell = DBItemSellData.FromCsv<DBItemSellData>(csvPath);

        var expected = dbItemSell.GetBytes().ToList();
        Assert.Equal(expected, outputDbItemSell.GetBytes());
        Assert.Equal(expected, jsonDbItemSell.GetBytes());

        // DbItemSell doesn't seem to follow the casing conventions of the other formats so, since the field names get assigned based on the
        // property names, the csv field names may not be the same as the original SData field names.
        // This is why only record values are tested here.
        var itemSellMemoryStream = new MemoryStream();
        var csvItemSellMemoryStream = new MemoryStream();
        var serializationOptions = new BinarySerializationOptions
        {
            Episode = Episode.EP8
        };

        var dbItemSellBinaryWriter = new SBinaryWriter(itemSellMemoryStream, serializationOptions);
        var csvItemSellBinaryWriter = new SBinaryWriter(csvItemSellMemoryStream, serializationOptions);

        foreach (var record in dbItemSell.Records.ToSerializable())
        {
            record.Write(dbItemSellBinaryWriter);
        }

        foreach (var record in csvItemSell.Records.ToSerializable())
        {
            record.Write(csvItemSellBinaryWriter);
        }

        Assert.Equal(itemSellMemoryStream.GetBuffer(), csvItemSellMemoryStream.GetBuffer());
    }
}
