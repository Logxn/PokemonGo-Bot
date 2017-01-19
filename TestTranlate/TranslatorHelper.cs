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

namespace TestTranlate
{
    /// <summary>
    /// Description of TranslatorHelper.
    /// </summary>
    public class TranslatorHelper
    {
        public string languageSelected ="default";
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Translations");
        
        public void ExtractTexts(Control ctrl)
        {
            if (!Directory.Exists(path)){
                Directory.CreateDirectory(path);
            }
            var filename = Path.Combine(path, "default.json");
            var  dict = new Dictionary<String, String>();
            insertTexts(ctrl,dict);
            var content = JsonConvert.SerializeObject(dict,Formatting.Indented);
            File.WriteAllText(filename, content );
        }
        private void insertTexts(Control ctrl, Dictionary<String,String> dict){
            dict.Add(ctrl.Name,ctrl.Text);
            foreach (var element in ctrl.Controls) {
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
        public void Translate(Control ctrl)
        {
            Translate(ctrl, languageSelected);
        }
        
        public void Translate(Control ctrl, string lang)
        {
            var filename = Path.Combine(path, $"{lang}.json");
            if (File.Exists(filename)){
                string JSONstring = File.ReadAllText(filename);
                var dict = JsonConvert.DeserializeObject<Dictionary<String, String>>(JSONstring);
                writeTexts(ctrl,dict);
            }else{
                var tmp = lang.Split('-')[0];
                filename = Path.Combine(path, $"{tmp}.json");
                string JSONstring = File.ReadAllText(filename);
                var dict = JsonConvert.DeserializeObject<Dictionary<String, String>>(JSONstring);
                writeTexts(ctrl,dict);
                
            }
        }
        private void writeTexts(Control ctrl ,  Dictionary<String,String> dict)
        {
            if  (dict.ContainsKey(ctrl.Name))
                ctrl.Text = dict[ctrl.Name];
            foreach (Control element  in ctrl.Controls) {
                if  (dict.ContainsKey(element.Name))
                    element.Text = dict[element.Name];
                if (element is GroupBox){
                    writeTexts(element as Control,dict);
                }
            }
         }
     }
}
