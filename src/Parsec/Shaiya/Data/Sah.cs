﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Parsec.Common;
using Parsec.Extensions;
using Parsec.Helpers;
using Parsec.Readers;
using Parsec.Shaiya.Core;

namespace Parsec.Shaiya.DATA
{
    [DataContract]
    public partial class Sah : FileBase, IJsonReadable
    {
        /// <summary>
        /// Path to the saf file
        /// </summary>
        public string SafPath => string.Concat(Path.Substring(0, Path.Length - 3), "saf");

        /// <summary>
        /// Total amount of files that are present in the data; does not include directories.
        /// </summary>
        [DataMember]
        public int TotalFileCount { get; private set; }

        /// <summary>
        /// The data's root directory.
        /// </summary>
        [DataMember]
        public SahFolder RootFolder { get; private set; }

        /// <summary>
        /// Dictionary of folders that can be accessed by path
        /// </summary>
        public Dictionary<string, SahFolder> FolderIndex = new();

        /// <summary>
        /// Dictionary of files that can be accessed by path
        /// </summary>
        public Dictionary<string, SahFile> FileIndex = new();

        public Sah(string path)
        {
            Path = path;
        }

        [JsonConstructor]
        public Sah()
        {
        }

        /// <summary>
        /// Constructor used when creating a sah file from a directory
        /// </summary>
        /// <param name="path">Path where sah file will be saved</param>
        /// <param name="rootFolder">Shaiya main Folder containing all the sah's data</param>
        /// <param name="fileCount"></param>
        public Sah(string path, SahFolder rootFolder, int fileCount) : this(path)
        {
            RootFolder = rootFolder;
            TotalFileCount = fileCount;
        }

        public override void Read()
        {
            _binaryReader = new ShaiyaBinaryReader(Path);

            // Skip signature (3) and unknown bytes (4)
            _binaryReader.Skip(7);

            // Read total file count
            TotalFileCount = _binaryReader.Read<int>();

            // Index where data starts (after header - skip padding bytes)
            _binaryReader.SetOffset(51);

            // Read root folder and all of its subfolders
            RootFolder = new SahFolder(_binaryReader, null, FolderIndex, FileIndex);
        }

        /// <summary>
        /// Adds a folder to the sah file
        /// </summary>
        /// <param name="path">Folder's path</param>
        public SahFolder AddFolder(string path) => EnsureFolderExists(path);

        /// <summary>
        /// Adds a file to the sah file
        /// </summary>
        /// <param name="directoryPath">Directory where file must be added. MUST NOT INCLUDE FILE NAME.</param>
        /// <param name="file">File to add</param>
        public void AddFile(string directoryPath, SahFile file)
        {
            // Ensure directory exists
            var parentFolder = EnsureFolderExists(directoryPath);

            // Assign parent folder to file
            file.ParentFolder = parentFolder;

            // Add file to file list
            parentFolder.Files.Add(file);
        }

        /// <summary>
        /// Checks if a folder exists based on its path. If it doesn't exist, it will be created
        /// </summary>
        /// <param name="path">Folder path</param>
        public SahFolder EnsureFolderExists(string path)
        {
            // Check if folder is part of the folder index
            if (FolderIndex.TryGetValue(path, out var matchingFolder))
                return matchingFolder;

            // Split path with the '/' separator
            var pathFolders = path.Separate().ToList();

            // Set current folder to root folder
            var currentFolder = RootFolder;

            //  Iterate recursively through subfolders creating the missing one/
            foreach (string folderName in pathFolders)
            {
                if (!currentFolder.HasSubfolder(folderName))
                {
                    // Create new folder if it doesn't exist
                    var newFolder = new SahFolder(folderName, currentFolder);

                    // Create relative path
                    newFolder.RelativePath = System.IO.Path.Combine(currentFolder.RelativePath, newFolder.Name);

                    // Add new folder to current folder's subfolders
                    currentFolder.Subfolders.Add(newFolder);

                    // Add folder to folder index
                    FolderIndex.Add(newFolder.RelativePath, newFolder);

                    currentFolder = newFolder;
                }
                else
                {
                    // Get subfolder with path name
                    currentFolder = currentFolder.GetSubfolder(folderName);
                }
            }

            return currentFolder;
        }

        /// <summary>
        /// Checks if the first 3 bytes in the file match the "SAH" magic number or the provided one
        /// </summary>
        /// <param name="magicNumber">Magic number to check. <a href="https://en.wikipedia.org/wiki/Magic_number_(programming)">Click here</a> for more information.</param>
        public bool CheckMagicNumber(string magicNumber = "SAH")
        {
            var sahMagicNumber = _binaryReader.ReadString(3);
            return sahMagicNumber == magicNumber;
        }

        public override void Write(string path)
        {
            // Create byte list which will have the sah's data
            var buffer = new List<byte>();

            // Write sah signature
            buffer.AddRange(Encoding.ASCII.GetBytes("SAH"));

            // Write 4 unknown 0x00 bytes
            buffer.AddRange(new byte[4]);

            // Write total file count
            buffer.AddRange(BitConverter.GetBytes(TotalFileCount));

            // Write padding
            buffer.AddRange(new byte[40]);

            // Write root folder and all subfolders with files
            buffer.AddRange(RootFolder.GetBytes());

            // Write last 8 empty bytes
            buffer.AddRange(new byte[8]);

            // Create new file and write buffer
            FileHelper.WriteFile(path, buffer.ToArray());
        }
    }
}