using System.ComponentModel;
using Parsec.Helpers;
using Parsec.Shaiya.Data;

namespace Parsec.Tests.Shaiya;

public class SahTests
{
    [Theory]
    [InlineData("new_folder", "new_file.fl")]
    [InlineData(@"new_folder\sub1", "new_file.fl")]
    [InlineData("new_folder/sub1", "new_file.fl")]
    [InlineData(@"new_folder\sub1\sub2", "new_file.fl")]
    [InlineData("new_folder/sub1/sub2", "new_file.fl")]
    [InlineData(@"new_folder\sub1\sub2\sub3", "new_file.fl")]
    [InlineData("new_folder/sub1/sub2/sub3", "new_file.fl")]
    public void SahFileExistenceTest(string folderName, string fileName)
    {
        var sah = ParsecReader.FromFile<Sah>("Shaiya/Data/sample.sah");
        Assert.Equal("sah", sah.Extension);

        // Add folder to sah
        var newFolder = sah.AddFolder(folderName);

        // Add file to created folder
        var newFile1 = new SFile(fileName, 200, 512);

        sah.AddFile(folderName, newFile1);

        Assert.True(sah.HasFolder(folderName));
        Assert.True(sah.HasFile(newFile1.RelativePath));
        Assert.NotNull(newFolder.GetFile(fileName));
        Assert.True(newFolder.HasFile(fileName));

        var newSubfolder = sah.AddFolder($"{folderName}/sub");
        Assert.True(newFolder.HasSubfolder("sub"));
    }

    [Fact]
    [Description("Test that checks if the Sah subclasses can be instanciated with an empty constructor for json deserialization")]
    public void SahJsonCreationTest()
    {
        var sah = new Sah();
        var folder = new SDirectory();
        var file = new SFile();

        Assert.NotNull(sah);
        Assert.NotNull(folder);
        Assert.NotNull(file);
    }

    [Fact]
    public void SahReadWriteTest()
    {
        var outputPath = "Shaiya/Data/sample_out.sah";
        var patchOutputPath = "Shaiya/Data/sample_out_patch.sah";

        var sah = ParsecReader.FromFile<Sah>("Shaiya/Data/sample.sah");
        sah.Write(outputPath);
        var patch = ParsecReader.FromFile<Sah>("Shaiya/Data/patch.sah");
        patch.Write(patchOutputPath);

        var sah2 = ParsecReader.FromFile<Sah>(outputPath);
        var patch2 = ParsecReader.FromFile<Sah>(patchOutputPath);
        Assert.Equal(sah.GetBytes(), sah2.GetBytes());
        Assert.Equal(patch.GetBytes(), patch2.GetBytes());
    }

    [Theory]
    [InlineData("root/sub/sub_sub/sub_sub")]
    [InlineData("root\\sub\\sub_sub\\sub_sub")]
    [InlineData("we/all/live/in/a/yellow/submarine")]
    [InlineData("we\\all\\live\\in\\a\\yellow\\submarine")]
    public void Sah_EnsureFolderExistsTest(string folderPath)
    {
        var sah = new Sah
        {
            RootDirectory = new SDirectory(string.Empty, null!)
        };

        var directory = sah.EnsureFolderExists(folderPath);
        var normalizedPath = PathHelper.Normalize(folderPath);

        Assert.Equal(normalizedPath, directory.RelativePath);
        Assert.True(sah.HasFolder(folderPath));
        Assert.True(sah.DirectoryIndex.ContainsKey(normalizedPath));
    }
}
