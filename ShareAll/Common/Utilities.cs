using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Streams;
using nsoftware.IPWorksSSL;
using System.Text.RegularExpressions;

namespace ShareAll.Common
{
    public class Utilities
    {
        public const string MatchEmailPattern = @"\w+([-+.]\w+)*@(yahoo|gmail|hotmail|msn|live)\.com";
        public const string MatchEmailToPattern = @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*[.][a-zA-Z]{2,10}$";

        public string AES_Encrypt(string input, string pass)
        {
            SymmetricKeyAlgorithmProvider SAP = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
            CryptographicKey AES;
            HashAlgorithmProvider HAP = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            CryptographicHash Hash_AES = HAP.CreateHash();

            string encrypted = "";
            try
            {
                byte[] hash = new byte[32];
                Hash_AES.Append(CryptographicBuffer.CreateFromByteArray(System.Text.Encoding.UTF8.GetBytes(pass)));
                byte[] temp;
                CryptographicBuffer.CopyToByteArray(Hash_AES.GetValueAndReset(), out temp);

                Array.Copy(temp, 0, hash, 0, 16);
                Array.Copy(temp, 0, hash, 15, 16);

                AES = SAP.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(hash));

                IBuffer Buffer = CryptographicBuffer.CreateFromByteArray(System.Text.Encoding.UTF8.GetBytes(input));
                encrypted = CryptographicBuffer.EncodeToBase64String(CryptographicEngine.Encrypt(AES, Buffer, null));

                return encrypted;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string AES_Decrypt(string input, string pass)
        {
            SymmetricKeyAlgorithmProvider SAP = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
            CryptographicKey AES;
            HashAlgorithmProvider HAP = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            CryptographicHash Hash_AES = HAP.CreateHash();

            string decrypted = "";
            try
            {
                byte[] hash = new byte[32];
                Hash_AES.Append(CryptographicBuffer.CreateFromByteArray(System.Text.Encoding.UTF8.GetBytes(pass)));
                byte[] temp;
                CryptographicBuffer.CopyToByteArray(Hash_AES.GetValueAndReset(), out temp);

                Array.Copy(temp, 0, hash, 0, 16);
                Array.Copy(temp, 0, hash, 15, 16);

                AES = SAP.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(hash));

                IBuffer Buffer = CryptographicBuffer.DecodeFromBase64String(input);
                byte[] Decrypted;
                CryptographicBuffer.CopyToByteArray(CryptographicEngine.Decrypt(AES, Buffer, null), out Decrypted);
                decrypted = System.Text.Encoding.UTF8.GetString(Decrypted, 0, Decrypted.Length);

                return decrypted;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string SendEmail(Htmlmailers htmlmailer, string serverDomain, string serverType, string UserEmail, string password, string fromEmail, string toEmail, string subject, string message)
        {
            htmlmailer.RuntimeLicense = "31534E3952413153554252414E5852465350454E434552544553540000000000000000000000000038344E394A483841000052424B414A3739394D3354440000";
            htmlmailer.MailServer = serverDomain;

            if (serverType != "Yahoo")
            {
                htmlmailer.SSLStartMode = HtmlmailersSSLStartModes.sslExplicit;
                htmlmailer.MailPort = 587;
            }
            else
            {
                htmlmailer.MailPort = 465;
            }
            htmlmailer.User = UserEmail;
            htmlmailer.Password = password;
            htmlmailer.From = fromEmail;
            htmlmailer.SendTo = toEmail;
            htmlmailer.Subject = subject;
            htmlmailer.MessageHTML = message;
            htmlmailer.Config("Hello=Metro"); //This can not be automatically determined in WinRT due to access restrictions.

            string returnmessage = "";
            try
            {
                htmlmailer.SendAsync();
                returnmessage = "Success";
            }
            catch (Exception ex)
            {
                returnmessage = "Error: " + ex.Message;
            }

            return returnmessage;
        }

        public string ExtractLinkFromText(string sText)
        {

            string extractedURL = "";
            Regex linkRegex = new Regex(@"(http|ftp|https)://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            MatchCollection linkCollection = linkRegex.Matches(sText);
            if (linkCollection.Count > 0)
            {
                extractedURL = linkCollection[0].ToString();

            }

            return extractedURL;

        }

        public string createCSVEmailList(string[] mailToEmailsArray, string delimiter)
        {
            string returnString = "";

            for (int i = 0; i < mailToEmailsArray.Length; i++)
            {
                returnString += mailToEmailsArray[i].ToString().Trim();

                if (i != (mailToEmailsArray.Length - 1))
                    returnString += delimiter;

            }

            return returnString;
        }

        /// <summary>
        /// Checks whether the given Email-Parameter is a valid E-Mail address.
        /// </summary>
        /// <param name="email">Parameter-string that contains an E-Mail address.</param>
        /// <returns>True, when Parameter-string is not null and 
        /// contains a valid E-Mail address;
        /// otherwise false.</returns>
        public bool IsEmail(string email)
        {
            if (email != null) 
                return Regex.IsMatch(email, MatchEmailPattern);
            else 
                return false;
        }
        
        public bool IsValidEmailTo(string email)
        {
            if (email != null)
                return Regex.IsMatch(email, MatchEmailToPattern);
            else
                return false;

        }
    }
}
