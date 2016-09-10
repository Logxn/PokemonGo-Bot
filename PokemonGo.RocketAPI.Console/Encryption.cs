/*
 */
using System;
using System.Text;
using System.Security.Cryptography;

namespace PokemonGo.RocketAPI.Console {
 /// <summary>
 /// Class to encypt/decrypt strings using 3DESCrypto and Base64 to can save it as plain text.
 /// </summary>
 public static class Encryption {

  public static string Encrypt(string input, string key = "Ar1iPokemonGoBot") {
   byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
   var tripleDES = new TripleDESCryptoServiceProvider();
   tripleDES.Key = ASCIIEncoding.ASCII.GetBytes(key);
   tripleDES.Mode = CipherMode.ECB;
   tripleDES.Padding = PaddingMode.PKCS7;
   byte[] resultArray = tripleDES.CreateEncryptor().TransformFinalBlock(inputArray, 0, inputArray.Length);
   tripleDES.Clear();
   return Convert.ToBase64String(resultArray, 0, resultArray.Length);
  }

  public static string Decrypt(string input, string key = "Ar1iPokemonGoBot") {
   byte[] inputArray = Convert.FromBase64String(input);
   var tripleDES = new TripleDESCryptoServiceProvider();
   tripleDES.Key = ASCIIEncoding.ASCII.GetBytes(key);
   tripleDES.Mode = CipherMode.ECB;
   tripleDES.Padding = PaddingMode.PKCS7;
   byte[] resultArray = tripleDES.CreateDecryptor().TransformFinalBlock(inputArray, 0, inputArray.Length);
   tripleDES.Clear();
   return UTF8Encoding.UTF8.GetString(resultArray);
  }

 }
}