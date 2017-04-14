/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 23/01/2017
 * Time: 22:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Net;
using System.Web;
using System.Text;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;

namespace PokeMaster.Translator
{
    /// <summary>
    /// Description of GoogleTranslator.
    /// </summary>
    public static class TranslateReferenceCom
    {
        /// <summary>
        /// Translate a string using 
        /// </summary>
        public static string ErrorMessage;
        
        public static string Translate(string text, string fromCulture, string toCulture)
        {

            string url = string.Format(@"http://translate.reference.com/{0}/{1}/{2}", fromCulture, toCulture,HttpUtility.UrlEncode(text));

            string html = null;
            try {
                WebClient web = new WebClient();

                // MUST add a known browser user agent or else response encoding doen't return UTF-8 (WTF Google?)
                web.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
                web.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");

                // Make sure we have response encoding to UTF-8
                web.Encoding = Encoding.UTF8;
                html = web.DownloadString(url);
            } catch (Exception ex) {
                ErrorMessage = "Westwind.Globalization.Resources.Resources.ConnectionFailed" + ": " +
                ex.GetBaseException().Message;
                return null;
            }

            string result = Regex.Match(html, "readonly>(.*?)<\\/textarea>", RegexOptions.IgnoreCase).Groups[1].Value;


            if (string.IsNullOrEmpty(result)) {
                ErrorMessage = "Westwind.Globalization.Resources.Resources.InvalidSearchResult";
                return null;
            }

            return result; 
        }

        
    }
}
