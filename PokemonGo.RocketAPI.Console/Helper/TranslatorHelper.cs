/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 17/01/2017
 * Time: 16:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;

namespace PokemonGo.RocketAPI.Console.Helper
{
    /// <summary>
    /// Class to Help translation of forms.
    /// * Extract all text from a form in a default.json file using ExtractTexts(ctrl)
    ///   ** you can add new text calling same function again.
    /// * Translate .json, rename the file to your CultureInfo Name (example Spanish : es.json)
    /// * You can translate a form with that file using 
    ///   ** Translate(ctrl): if you want use current cultureinfo in the system.
    ///   ** Translate(ctrl,CultureInfo.Name): if you want translate it to a given cultureinfo.
    /// * You can put "NO TRANSLATE" text in ".tag" field if you wannot translate that control
    /// </summary>
    public class TranslatorHelper
    {
        private string language = "default";
        
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Translations");
        private Dictionary<String, String> dictionary = new Dictionary<String, String>();
        
        public TranslatorHelper()
        {
            SelectLanguage(CultureInfo.CurrentCulture.Name);
        }
        
        public void SelectLanguage(string lang)
        {
            language = lang;
            dictionary = loadDictionary(lang);
        }
       
        public void Translate(Control ctrl)
        {
            writeTexts("", ctrl, dictionary);
        }
        
        public string Translate(string str)
        {
            if (dictionary.ContainsKey(str))
                return dictionary[str];
            return "";
        }
        
        public static void Translate(Control ctrl, string lang)
        {
            var dict = loadDictionary(lang);
            writeTexts("", ctrl, dict);
        }

        public static string Translate(string str, string lang)
        {
            var dict = loadDictionary(lang);
            if (dict.ContainsKey(str))
                return dict[str];
            return "";
        }

        public void ExtractTexts(Control ctrl)
        {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            var filename = Path.Combine(path, "default.json");
            var dict = new Dictionary<String, String>();
            if (File.Exists(filename)) {
                string JSONstring = File.ReadAllText(filename);
                dict = JsonConvert.DeserializeObject<Dictionary<String, String>>(JSONstring);
            }
            insertTexts("", ctrl, dict);
            var content = JsonConvert.SerializeObject(dict, Formatting.Indented);
            File.WriteAllText(filename, content);
        }
        
        
        private void insertTexts(string prefix, Control ctrl, Dictionary<String,String> dict)
        {
            var tagVal = ctrl.Tag as string;
            if (ctrl.Parent != null)
                prefix += ctrl.Parent.Name + ".";
            if (tagVal != "NO TRANSLATE") {
                SafeAddToDict(dict, prefix + ctrl.Name, ctrl.Text);
            }
            var ctrlPrefix = prefix + ctrl.Name + ".";
            foreach (Control element  in ctrl.Controls) {
                tagVal = element.Tag as string;
                if (tagVal != "NO TRANSLATE") {
                    if (element is Label)
                        SafeAddToDict(dict, ctrlPrefix + (element as Label).Name, (element as Label).Text);
                    if (element is CheckBox)
                        SafeAddToDict(dict, ctrlPrefix + (element as CheckBox).Name, (element as CheckBox).Text);
                    if (element is RadioButton)
                        SafeAddToDict(dict, ctrlPrefix + (element as RadioButton).Name, (element as RadioButton).Text);
                    if (element is Button)
                        SafeAddToDict(dict, ctrlPrefix + (element as Button).Name, (element as Button).Text);
                    if (element is GroupBox) {
                        insertTexts(prefix, element as Control, dict);
                    }
                    if (element is TabControl) {
                        insertTexts(prefix, element as Control, dict);
                    }
                    if (element is TabPage) {
                        insertTexts(prefix, element as Control, dict);
                    }
                }
            }
        }
        
        private void SafeAddToDict(IDictionary<string, string> dict, string key, string value)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, value);
        }
        
        
        private static Dictionary<String, String> loadDictionary(string lang)
        {
            var filename = Path.Combine(path, $"{lang}.json");
            var dict = new Dictionary<String, String>();
            if (File.Exists(filename)) {
                string JSONstring = File.ReadAllText(filename);
                dict = JsonConvert.DeserializeObject<Dictionary<String, String>>(JSONstring);
            } else {
                var baseLang = lang.Split('-');
                if (baseLang.Length > 1)
                {
                    filename = Path.Combine(path, $"{baseLang[0].ToLower()}.json");
                    if (File.Exists(filename))
                    {
                        string JSONstring = File.ReadAllText(filename);
                        dict = JsonConvert.DeserializeObject<Dictionary<String, String>>(JSONstring);
                    }
                    else
                    {
                        filename = Path.Combine(path, $"{baseLang[1].ToLower()}.json");
                        if (File.Exists(filename))
                        {
                            string JSONstring = File.ReadAllText(filename);
                            dict = JsonConvert.DeserializeObject<Dictionary<String, String>>(JSONstring);
                        }
                    }
                }
            }
            return dict;
        }
        
        
        private static void writeTexts(string prefix, Control ctrl, Dictionary<String,String> dict)
        {
            if (dict == null)
                return;
            var tagVal = ctrl.Tag as string;        	
            if (ctrl.Parent != null)
                prefix += ctrl.Parent.Name + ".";
            if (tagVal != "NO TRANSLATE") {
                if (dict.ContainsKey(prefix + ctrl.Name))
                    ctrl.Text = dict[prefix + ctrl.Name];
            }
            var ctrlPrefix = prefix + ctrl.Name + ".";
            foreach (Control element  in ctrl.Controls) {
                tagVal = (element as Control).Tag as string;
                if (tagVal != "NO TRANSLATE") {
                    if (dict.ContainsKey(ctrlPrefix + element.Name))
                        element.Text = dict[ctrlPrefix + element.Name];
                    if (element is GroupBox) {
                        writeTexts(prefix, element as Control, dict);
                    }
                    if (element is TabControl) {
                        writeTexts(prefix, element as Control, dict);
                    }
                    if (element is TabPage) {
                        writeTexts(prefix, element as Control, dict);
                    }
                }
            }
        }
    }
}
