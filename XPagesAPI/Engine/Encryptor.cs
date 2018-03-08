using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;
using System.IO.Compression;

internal class Encryptor {

    #region Variables

    private string IV = "GeNar@tEdIv_K3y!"; // "IV_VALUE_16_BYTE"                    
    private string PASSWORD = "JPITeam@XPages!|"; // "PASSWORD_VALUE"
    private string SALT = "S@1tS@lt_Valu3@JPI_XP@ges"; //"SALT_VALUE"

    protected internal string EncodedContent;
    protected internal bool isInitialized = false;
    protected internal string DecodedContent;
    protected internal bool Encode = false;

    #endregion

    #region Constructor

    protected internal Encryptor(string content, bool encode, ref Connector Connector) {
        this.Encode = encode;
        if (this.Encode) {
            //we need to encode so set the decodedcontent
            this.DecodedContent = content;
        } else {
            //we need to decode/decrypt so set the encodecontent
            this.EncodedContent = content;
        }
        if (!string.IsNullOrEmpty(Connector.EncryptionIV)) {
            IV = Connector.EncryptionIV;
        }

        if (!string.IsNullOrEmpty(Connector.EncryptionPASSWORD)) {
            PASSWORD = Connector.EncryptionPASSWORD;
        }

        if (!string.IsNullOrEmpty(Connector.EncryptionSALT)) {
            SALT = Connector.EncryptionSALT;
        }
    }

    #endregion

    #region Public Methods

    protected internal bool Initialize() {
        string Str;
        if (this.Encode) {
            Str = EncryptAndEncode(this.DecodedContent);
            if (Str != null) {
                this.EncodedContent = Str;
                this.isInitialized = true;
                return true;
            } else {
                this.isInitialized = false;
                return false;
            }
        } else {
            Str = DecodeAndDecrypt(this.EncodedContent);
            if (Str != null) {
                this.DecodedContent = Str;
                this.isInitialized = true;
                return true;
            } else {
                this.isInitialized = false;
                return false;
            }
        }

    }
    
    //public string EncryptAndEncode(string raw) {
    //    AesCryptoServiceProvider csp = new AesCryptoServiceProvider();
    //    using (csp) {
    //        ICryptoTransform e = GetCryptoTransform(csp, true);
    //        byte [] inputBuffer = Encoding.UTF8.GetBytes(raw);
    //        byte [] output = e.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
    //        string encrypted = Convert.ToBase64String(output);
    //        return encrypted;
    //    }
    //}

    //public string DecodeAndDecrypt(string encrypted) {
    //    AesCryptoServiceProvider csp = new AesCryptoServiceProvider();
    //    using (csp) {
    //        ICryptoTransform d = GetCryptoTransform(csp, false);
    //        byte [] output = Convert.FromBase64String(encrypted);
    //        byte [] decryptedOutput = d.TransformFinalBlock(output, 0, output.Length);
    //        string decypted = Encoding.UTF8.GetString(decryptedOutput);
    //        return decypted;
    //    }
    //}


    public string EncryptAndEncode(string raw) {
        AesCryptoServiceProvider csp = new AesCryptoServiceProvider();
        using (csp) {
            ICryptoTransform e = GetCryptoTransform(csp, true);
            byte[] inputBuffer = Encoding.UTF8.GetBytes(raw);
            byte[] output = e.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            output = DeflaterCompress(output);
            string encrypted = Convert.ToBase64String(output);
            return encrypted;
        }
    }

    private byte[] DeflaterCompress(byte[] toCompress) {
        //compression
        using (MemoryStream compressedStream = new MemoryStream()) {
            compressedStream.Position = 0;
            using (DeflateStream deflater = new DeflateStream(compressedStream, CompressionMode.Compress)) {
                deflater.Write(toCompress, 0, toCompress.Length);
            }

            return compressedStream.ToArray();
        }
    }

    private byte[] DeflaterDecompress(byte[] toDecompress) {
        //decompression
        using (MemoryStream decompressedStream = new MemoryStream()) {
            using (MemoryStream compressedStream = new MemoryStream(toDecompress)) {
                using (DeflateStream deflater = new DeflateStream(compressedStream, CompressionMode.Decompress)) {
                    int c = 0;
                    while ((c = deflater.ReadByte()) != -1) {
                        decompressedStream.WriteByte((byte)c);
                    }
                }
            }
            return decompressedStream.ToArray();
        }
    }

    public string DecodeAndDecrypt(string encrypted) {
        //this will be in base64 - base64 is zipped
        AesCryptoServiceProvider csp = new AesCryptoServiceProvider();
        using (csp) {
            ICryptoTransform d = GetCryptoTransform(csp, false);
            byte[] output = Convert.FromBase64String(encrypted);
            output = DeflaterDecompress(output);
            byte[] decryptedOutput = d.TransformFinalBlock(output, 0, output.Length);
            string decypted = Encoding.UTF8.GetString(decryptedOutput);
            return decypted;
        }
    }

    #endregion

    #region Private Methods

    private ICryptoTransform GetCryptoTransform(AesCryptoServiceProvider csp, bool encrypting) {
        csp.Mode = CipherMode.CBC;
        csp.Padding = PaddingMode.PKCS7;
        Rfc2898DeriveBytes spec = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(PASSWORD), Encoding.UTF8.GetBytes(SALT), 65536);
        byte [] key = spec.GetBytes(16);
        csp.IV = Encoding.UTF8.GetBytes(IV);
        csp.Key = key;
        if (encrypting) {
            return csp.CreateEncryptor();
        }
        return csp.CreateDecryptor();
    }

    #endregion

}
