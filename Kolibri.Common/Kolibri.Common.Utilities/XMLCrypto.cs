using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;   
using System;
    using System.Security.Cryptography;
    using System.Text;


namespace Kolibri.Common.Utilities
{
    /// <summary>
    /// Encrypts and decrypts strings using DES3 encryption and the .NET Framework
    /// </summary>
    public  class XMLCrypto
    { 
        static byte[] mabtyBuffer;
        static TripleDESCryptoServiceProvider mobjDES;

        /// <summary>
        /// Create a secret key. The key is used to encrypt and decrypt strings.
        /// Without the key, the encrypted string cannot be decrypted and is just garbage.
        /// You must use the same key to decrypt an encrypted string as the string was originally encrypted with.
        /// </summary>
        private static void Construct()
        {
            string strKey = "";
            MD5CryptoServiceProvider hashmd5;
            byte[] abytKeyHash;

            strKey += "FEDCBA9876543210";

            // Sample keys
            //m_key = m_TextConverter.GetBytes("3.1415926535897932384626 43383279");
            //m_IV = m_TextConverter.GetBytes("FEDCBA9876543210");



            /// <remarks>
            /// Generate an MD5 hash from the key.
            /// A hash is a one way encryption meaning once you generate
            /// the hash, you can't derive the key back from it.
            /// </remarks>
            hashmd5 = new MD5CryptoServiceProvider();
            abytKeyHash = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(strKey));
            hashmd5 = null;

             mobjDES = new TripleDESCryptoServiceProvider(); //implement DES3 encryption
            mobjDES.Key = abytKeyHash; //the key is the secret key hash.

            /// <remarks>
            /// The mode is the block cipher mode which is basically the details of how the encryption will work.
            /// There are several kinds of ciphers available in DES3 and they all have benefits and drawbacks.
            /// Here the Electronic Codebook cipher is used which means that a given bit of text is always encrypted
            /// exactly the same when the same key is used.
            /// </remarks>
            mobjDES.Mode = CipherMode.ECB; //CBC, CFB
        }

        private static void Deconstruct()
        {
            if (mobjDES != null)
            {
                mobjDES = null;
            }
        }

        /// <summary>
        /// To encrypt an unencrypted string
        /// </summary>
        /// <param name="pstrText">Text to be encrypted</param>
        /// <returns>Encrypted text</returns>
        public static string Encrypt(string pstrText)
        {
            try
            {
                Construct();
                //mabtyBuffer = ASCIIEncoding.ASCII.GetBytes(pstrText);
                mabtyBuffer = UTF8Encoding.UTF8.GetBytes(pstrText);
                return
                Convert.ToBase64String(mobjDES.CreateEncryptor().TransformFinalBlock(mabtyBuffer, 0, mabtyBuffer.Length));
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Deconstruct();
            }
        }

        /// <summary>
        /// To decrypt an encrypted string
        /// </summary>
        /// <param name="pstrText">Text to be decrypted</param>
        /// <returns>Decrypted text</returns>
        public static string Decrypt(string pstrText)
        {
            try
            {
                Construct();
                mabtyBuffer = Convert.FromBase64String(pstrText);
                return
                UTF8Encoding.UTF8.GetString(mobjDES.CreateDecryptor().TransformFinalBlock(mabtyBuffer,
                0, mabtyBuffer.Length));
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Deconstruct();
            }
        }
    }
}


