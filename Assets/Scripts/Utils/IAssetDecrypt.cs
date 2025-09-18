// 参考文档
//https://github.com/jamesjlinden/unity-decompiled
using UnityEditor;
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;

class IAssestDecrypt
{
    /*
    [MenuItem("Utility/AssestDecrypt")]
    static void DecryptFile()
    {
        var inputFile = @"D:\b977f6a3-331b-4037-8266-82df3cd81a65";
        var key = "1a61f8162ed57acd416d93af6544b6250b4a390f3cb69564bde21aa310c404ab2eb7d14d50e4721831ecf167509c260f";

        //var unityEditor = typeof(Editor).Assembly;

        //var assetStoreUtils = unityEditor.GetType("UnityEditor.AssetStoreUtils");

        //assetStoreUtils.Invoke("DecryptFile", inputFile, inputFile + ".unitypackage", key);

        DecryptFile(inputFile, inputFile + ".unitypackage", key);
    }

    private static void HexStringToByteArray(string hex, byte[] array, int offset)
    {
        if (offset + array.Length * 2 > hex.Length)
            throw new ArgumentException("Hex string too short");
        for (int index = 0; index < array.Length; ++index)
        {
            string s = hex.Substring(index * 2 + offset, 2);
            array[index] = byte.Parse(s, NumberStyles.HexNumber);
        }
    }

    public static void DecryptFile(string inputFile, string outputFile, string keyIV)
    {
        byte[] array1 = new byte[32];
        byte[] array2 = new byte[16];
        HexStringToByteArray(keyIV, array1, 0);
        HexStringToByteArray(keyIV, array2, 64);
        EditorUtility.DisplayProgressBar("Decrypting", "Decrypting package", 0.0f);
        FileStream fileStream1 = File.Open(inputFile, System.IO.FileMode.Open);
        FileStream fileStream2 = File.Open(outputFile, System.IO.FileMode.CreateNew);
        long length = fileStream1.Length;
        long num = 0;
        AesManaged aesManaged = new AesManaged();
        aesManaged.Key = array1;
        aesManaged.IV = array2;
        CryptoStream cryptoStream = new CryptoStream((Stream)fileStream1, aesManaged.CreateDecryptor(aesManaged.Key, aesManaged.IV), CryptoStreamMode.Read);
        try
        {
            byte[] numArray = new byte[40960];
            int count;
            while ((count = cryptoStream.Read(numArray, 0, numArray.Length)) > 0)
            {
                fileStream2.Write(numArray, 0, count);
                num += (long)count;
                if (EditorUtility.DisplayCancelableProgressBar("Decrypting", "Decrypting package", (float)num / (float)length))
                    throw new Exception("User cancelled decryption");
            }
        }
        finally
        {
            cryptoStream.Close();
            fileStream1.Close();
            fileStream2.Close();
            EditorUtility.ClearProgressBar();
        }
    }
    */
}
