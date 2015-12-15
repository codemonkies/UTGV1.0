using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Security.Cryptography;

namespace UTGHelper
{
   public static class Cryotor
   {
      private static readonly byte[] SALT = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };

      public static byte[] Encrypt(byte[] plain, string password)
      {
         MemoryStream memoryStream;
         CryptoStream cryptoStream;
         Rijndael rijndael = Rijndael.Create();
         Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT);
         rijndael.Key = pdb.GetBytes(32);
         rijndael.IV = pdb.GetBytes(16);
         memoryStream = new MemoryStream();
         cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
         cryptoStream.Write(plain, 0, plain.Length);
         cryptoStream.Close();
         return memoryStream.ToArray();
      }

      public static byte[] Decrypt(byte[] cipher, string password)
      {
         MemoryStream memoryStream;
         CryptoStream cryptoStream;
         Rijndael rijndael = Rijndael.Create();
         Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT);
         rijndael.Key = pdb.GetBytes(32);
         rijndael.IV = pdb.GetBytes(16);
         memoryStream = new MemoryStream();
         cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
         cryptoStream.Write(cipher, 0, cipher.Length);
         cryptoStream.Close();
         return memoryStream.ToArray();
      }

      public static string EncryptString(string InputText, string Password)
      {
         RijndaelManaged RijndaelCipher = new RijndaelManaged();

         byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(InputText);

         byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());

         PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

         ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

         MemoryStream memoryStream = new MemoryStream();

         CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);

         cryptoStream.Write(PlainText, 0, PlainText.Length);

         cryptoStream.FlushFinalBlock();

         byte[] CipherBytes = memoryStream.ToArray();

         memoryStream.Close();

         cryptoStream.Close();

         string EncryptedData = Convert.ToBase64String(CipherBytes);

         // Return encrypted string.
         return EncryptedData;
      }

      public static string DecryptString(string InputText, string Password)
      {
         RijndaelManaged RijndaelCipher = new RijndaelManaged();

         byte[] EncryptedData = Convert.FromBase64String(InputText);

         byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());

         PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

         ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

         MemoryStream memoryStream = new MemoryStream(EncryptedData);

         CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

         byte[] PlainText = new byte[EncryptedData.Length];

         int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

         memoryStream.Close();

         cryptoStream.Close();

         string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);

         return DecryptedData;

      }
   }
}
