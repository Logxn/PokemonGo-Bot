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

namespace TestTranlate
{
    /// <summary>
    /// Description of TranslatorHelper.
    /// </summary>
    public class TranslatorHelper
    {
        private string language ="default";
        
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Translations");
        private Dictionary<String, String> dictionary = new Dictionary<String, String> ();
        
        public TranslatorHelper(){
            SelectLanguage(CultureInfo.CurrentCulture.Name);
        }
        
        public void SelectLanguage(string lang){
        	language = lang;
        	dictionary = loadDictionary(lang);
        }
       
        public void Translate(Control ctrl)
        {
            writeTexts(ctrl,dictionary);
        }
        
        public string Translate(string str)
        {
        	if  (dictionary.ContainsKey(str))
        		return dictionary[str];
        	return "";
        }
        
        public static void Translate(Control ctrl, string lang)
        {
        	var dict = loadDictionary(lang);
            writeTexts(ctrl,dict);
        }

        public static string Translate(string str, string lang)
        {
        	var dict = loadDictionary(lang);
        	if  (dict.ContainsKey(str))
        		return dict[str];
        	return "";
        }

        public void ExtractTexts(Control ctrl)
        {
            if (!Directory.Exists(path)){
                Directory.CreateDirectory(path);
            }
            var filename = Path.Combine(path, "default.json");
            var dict = new Dictionary<String, String>();
            insertTexts(ctrl,dict);
            var content = JsonConvert.SerializeObject(dict,Formatting.Indented);
            File.WriteAllText(filename, content );
        }
        
        private void insertTexts(Control ctrl, Dictionary<String,String> dict){
        	var tagVal = ctrl.Tag as string;
          	if (  tagVal != "NO TRANSLATE" )
	            dict.Add(ctrl.Name,ctrl.Text);
            foreach (var element in ctrl.Controls) {
            	tagVal = (element as Control).Tag as string;
            	if (  tagVal != "NO TRANSLATE" ){
	                if (element is Label)
	                        dict.Add((element as Label).Name,(element as Label).Text);
	                if (element is CheckBox)
	                        dict.Add((element as CheckBox).Name,(element as CheckBox).Text);
	                if (element is RadioButton)
	                        dict.Add((element as RadioButton).Name,(element as RadioButton).Text);
	                if (element is Button)
	                        dict.Add((element as Button).Name,(element as Button).Text);
	                if (element is GroupBox){
	                        insertTexts(element as Control,dict);
	                }
            	}
            }
        }
        
        
        private static Dictionary<String, String> loadDictionary( string lang)
        {
            var filename = Path.Combine(path, $"{lang}.json");
            var dict = new Dictionary<String, String> ();
            if (File.Exists(filename)){
                string JSONstring = File.ReadAllText(filename);
                dict = JsonConvert.DeserializeObject<Dictionary<String, String>>(JSONstring);
            }else{
                var tmp = lang.Split('-')[0];
                filename = Path.Combine(path, $"{tmp}.json");
                if (File.Exists(filename)){
	                string JSONstring = File.ReadAllText(filename);
	                dict = JsonConvert.DeserializeObject<Dictionary<String, String>>(JSONstring);
                }                
            }
            return dict;
        }
        
        
        private static void writeTexts(Control ctrl ,  Dictionary<String,String> dict)
        {
        	var tagVal = ctrl.Tag as string;
          	if (  tagVal != "NO TRANSLATE" )
	            if  (dict.ContainsKey(ctrl.Name))
	                ctrl.Text = dict[ctrl.Name];
            foreach (Control element  in ctrl.Controls) {
          		tagVal = (element as Control).Tag as string;
            	if (  tagVal != "NO TRANSLATE" ){
	                if  (dict.ContainsKey(element.Name))
	                    element.Text = dict[element.Name];
	                if (element is GroupBox){
	                    writeTexts(element as Control,dict);
	                }
            	}
            }
        }
     }
}
