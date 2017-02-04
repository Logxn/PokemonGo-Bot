/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 15/01/2017
 * Time: 14:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PokemonGo.RocketAPI.Logic.Shared;

namespace PokemonGo.RocketAPI.Console.Panels
{
    /// <summary>
    /// Description of TutorialPanel.
    /// </summary>
    public partial class TutorialPanel : UserControl
    {
        Helper.TranslatorHelper th =  Helper.TranslatorHelper.getInstance();
        public TutorialPanel()
        {
            InitializeComponent();
           th.Translate(this);
        }
        public void LoadData()
        {
            if (AvatarSettings.Load()){
                ltNickPrefix.Value = AvatarSettings.nicknamePrefix;
                ltNickSufix.Value = AvatarSettings.nicknameSufix;
                lcSkin.SelectedIndex = AvatarSettings.skin;
                lcHair.SelectedIndex = AvatarSettings.hair;
                lcEyes.SelectedIndex = AvatarSettings.eyes;
                lcHat.SelectedIndex = AvatarSettings.hat;
                lcShirt.SelectedIndex = AvatarSettings.shirt;
                lcPants.SelectedIndex = AvatarSettings.pants;
                lcShoes.SelectedIndex = AvatarSettings.shoes;            
                lcBackpack.SelectedIndex = AvatarSettings.backpack;
                lcGender.SelectedIndex = AvatarSettings.Gender;
    
                lcPokemon.SelectedIndex = 4;
                switch (AvatarSettings.starter) {
                    case POGOProtos.Enums.PokemonId.Bulbasaur: 
                        lcPokemon.SelectedIndex= 0;
                        break;
                    case POGOProtos.Enums.PokemonId.Charmander: 
                        lcPokemon.SelectedIndex= 1;
                        break;
                    case POGOProtos.Enums.PokemonId.Squirtle: 
                        lcPokemon.SelectedIndex= 2;
                        break;
                    case POGOProtos.Enums.PokemonId.Pikachu: 
                        lcPokemon.SelectedIndex= 3;
                        break;
                }
            }
        }
        public void SaveData()
        {
            AvatarSettings.skin = lcSkin.SelectedIndex;
            AvatarSettings.hair = lcHair.SelectedIndex;
            AvatarSettings.eyes = lcEyes.SelectedIndex;
            AvatarSettings.hat = lcHat.SelectedIndex;
            AvatarSettings.shirt = lcShirt.SelectedIndex;
            AvatarSettings.pants = lcPants.SelectedIndex;
            AvatarSettings.shoes = lcShoes.SelectedIndex;
            AvatarSettings.backpack = lcBackpack.SelectedIndex;
            AvatarSettings.nicknamePrefix = ltNickPrefix.Value;
            AvatarSettings.nicknameSufix = ltNickSufix.Value;
             AvatarSettings.Gender = lcGender.SelectedIndex;

            AvatarSettings.starter= POGOProtos.Enums.PokemonId.Missingno;
            switch (lcPokemon.SelectedIndex) {
                case 0: 
                    AvatarSettings.starter= POGOProtos.Enums.PokemonId.Bulbasaur;
                    break;
                case 1: 
                    AvatarSettings.starter= POGOProtos.Enums.PokemonId.Charmander;
                    break;
                case 2: 
                    AvatarSettings.starter= POGOProtos.Enums.PokemonId.Squirtle;
                    break;
                case 3: 
                    AvatarSettings.starter= POGOProtos.Enums.PokemonId.Pikachu;
                    break;
            }
            AvatarSettings.Save();
        }

    }
}
