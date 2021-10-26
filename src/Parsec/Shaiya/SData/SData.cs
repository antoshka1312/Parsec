﻿using System;
using System.Text;
using Parsec.Extensions;
using Parsec.External;
using Parsec.Helpers;
using Parsec.Shaiya.Core;

namespace Parsec.Shaiya
{
    public abstract class SData : FileBase
    {
        protected SData(string path) : base(path)
        {
            SetEncryptedBuffer();
            SetDecryptedBuffer();
        }

        /// <summary>
        /// Checks if the file has the ".SData" extension
        /// </summary>
        public bool IsValidSData => Path.Substring(Path.Length - 6, 6) == ".SData";

        /// <summary>
        /// Checks if the file is encrypted with the SEED algorithm
        /// </summary>
        public bool IsEncrypted
        {
            get
            {
                var encryptedFileHeader = "0001CBCEBC5B2784D3FC9A2A9DB84D1C3FEB6E99";

                var sdataHeader = Encoding.ASCII.GetString(Buffer.SubArray(0, encryptedFileHeader.Length));

                return sdataHeader == encryptedFileHeader;
            }
        }

        private byte[] _encryptedBuffer;

        private void SetEncryptedBuffer()
        {
            if (IsEncrypted)
            {
                _encryptedBuffer = new byte[Buffer.Length];
                Array.Copy(Buffer, _encryptedBuffer, Buffer.Length);
                return;
            }

            var tempBuffer = new byte[Buffer.Length];

            Array.Copy(Buffer, tempBuffer, Buffer.Length);

            var newBufferLength = ShaiyaCrypt.encrypt(tempBuffer, (uint)tempBuffer.Length);

            _encryptedBuffer = new byte[newBufferLength];

            Array.Copy(tempBuffer, _encryptedBuffer, _encryptedBuffer.Length <= tempBuffer.Length ? _encryptedBuffer.Length : tempBuffer.Length);
        }

        public byte[] EncryptedBuffer => _encryptedBuffer;

        private byte[] _decryptedBuffer;

        private void SetDecryptedBuffer()
        {
            if (!IsEncrypted)
            {
                _decryptedBuffer = new byte[Buffer.Length];
                Array.Copy(Buffer, _decryptedBuffer, Buffer.Length);
                return;
            }

            var tempBuffer = new byte[Buffer.Length];

            Array.Copy(Buffer, tempBuffer, Buffer.Length);

            var newBufferLength = ShaiyaCrypt.decrypt(tempBuffer, (uint)tempBuffer.Length);

            _decryptedBuffer = new byte[newBufferLength];

            Array.Copy(tempBuffer, _decryptedBuffer, _decryptedBuffer.Length <= tempBuffer.Length ? _decryptedBuffer.Length : tempBuffer.Length);

            Array.Copy(_decryptedBuffer, Buffer, _decryptedBuffer.Length);
        }

        public byte[] DecryptedBuffer => _decryptedBuffer;

        public void SaveEncrypted(string path) =>
            FileHelper.WriteFile(path, EncryptedBuffer);

        public void SaveDecrypted(string path) =>
            FileHelper.WriteFile(path, DecryptedBuffer);
    }
}