using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI;
using POGOProtos.Enums;
using POGOProtos.Networking.Responses;
using PokeMaster.Logic.Shared;
using POGOProtos.Data.Player;
using PokemonGo.RocketAPI.Helpers;
using Google.Protobuf.Collections;

namespace PokeMaster.Logic.Functions
{
    public class TutorialFunctions
    {
        //LegalScreen = 0, --> Implemented
        //AvatarSelection = 1, --> Implemented
        //AccountCreation = 2, --> Implemented
        //PokemonCapture = 3, --> Implemented
        //NameSelection = 4, --> Implemented

        //PokemonBerry = 5, --> Implemented
        //UseItem = 6,

        //FirstTimeExperienceComplete = 7, --> Implemented
        //PokestopTutorial = 8, --> Implemented

        //GymTutorial = 9 --> Implemented

        public bool MarkTutorialAsDone(TutorialState State, PokemonGo.RocketAPI.Client client, PokemonId firstPokemon = PokemonId.Charmander) // A Charmander
        {
            MarkTutorialCompleteResponse TutorialResponse = null;
            SetAvatarResponse AvatarResponse = null;
            bool SuccessFlag = false;

            switch (State) {
                case TutorialState.LegalScreen:
                    TutorialResponse = client.Misc.AceptLegalScreen().Result;
                    SuccessFlag = TutorialResponse.Success;
                    break;
                case TutorialState.AvatarSelection:
                    // All MAX values will be used to get a random value 
                    // RandormNumber never returns max value.
                    var playerAvatar = new PlayerAvatar();
                    playerAvatar.Skin = AvatarSettings.skin == 4 ? RandomHelper.RandomNumber(0, 4) : AvatarSettings.skin;
                    playerAvatar.Backpack = AvatarSettings.backpack == 3 ? RandomHelper.RandomNumber(0, 3) : AvatarSettings.backpack;
                    playerAvatar.Eyes = AvatarSettings.eyes == 4 ? RandomHelper.RandomNumber(0, 4) : AvatarSettings.eyes;
                    playerAvatar.Shoes = AvatarSettings.shoes == 3 ? RandomHelper.RandomNumber(0, 3) : AvatarSettings.shoes;
                    playerAvatar.Hat = AvatarSettings.hat == 3 ? RandomHelper.RandomNumber(0, 3) : AvatarSettings.hat;
                    playerAvatar.Pants = AvatarSettings.pants == 3 ? RandomHelper.RandomNumber(0, 3) : AvatarSettings.pants;
                    playerAvatar.Hair = AvatarSettings.hair == 6 ? RandomHelper.RandomNumber(0, 6) : AvatarSettings.hair;
                    playerAvatar.Shirt = AvatarSettings.shirt == 3 ? RandomHelper.RandomNumber(0, 3) : AvatarSettings.shirt;
                    playerAvatar.Avatar = AvatarSettings.Gender == 2 ? RandomHelper.RandomNumber(0, 2) : AvatarSettings.Gender;
                    
                    // TODO Add this new configurable avatar options to tutorial configuration window
                    // currently will use 0 for all value not loaded
                    //playerAvatar.AvatarNecklace = AvatarSettings.necklace = ...
                    //playerAvatar.AvatarBelt = AvatarSettings.belt == ...
                    //playerAvatar.AvatarSocks = AvatarSettings.socks == ...
                    //playerAvatar.AvatarGloves = AvatarSettings.gloves == ...
                    //playerAvatar.AvatarGlasses = AvatarSettings.glasses == ...

                    AvatarResponse = client.Player.SetAvatar(playerAvatar).Result;
                    SuccessFlag = (AvatarResponse.Status ==SetAvatarResponse.Types.Status.Success);
                    // TODO: check if this is really needed or the "TutorialState.PokemonCapture" flag is done by the above call.
                    if (!client.Player.PlayerResponse.PlayerData.TutorialState.Contains(TutorialState.AvatarSelection)) {
                        TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.AvatarSelection }).Result;
                        SuccessFlag = TutorialResponse.Success;
                    }

                    break;
                case TutorialState.AccountCreation:
                    // Need to check how to implement, meanwhile...
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.AccountCreation }).Result;
                    SuccessFlag = TutorialResponse.Success;
                    break;
                case TutorialState.PokemonCapture:
                    if (AvatarSettings.starter == PokemonId.Missingno ){
                        // Selected random pokemon
                        var rnd = new Random().Next(0,4);
                        switch (rnd) {
                            case 0:
                                AvatarSettings.starter = PokemonId.Bulbasaur;
                                break;
                            case 1:
                                AvatarSettings.starter =PokemonId.Charmander;
                                break;
                            case 2:
                                AvatarSettings.starter = PokemonId.Squirtle;
                                break;
                            default:
                                AvatarSettings.starter = PokemonId.Pikachu;
                                break;
                        }
                    }
                    var encounterResponse = client.Encounter.EncounterTutorialComplete(AvatarSettings.starter);
                    if (encounterResponse.Result == EncounterTutorialCompleteResponse.Types.Result.ErrorInvalidPokemon) 
                        encounterResponse = client.Encounter.EncounterTutorialComplete(PokemonId.Charmander);
                    SuccessFlag = (encounterResponse.Result == EncounterTutorialCompleteResponse.Types.Result.Success);
                    
                    // TODO: check if this is really needed or the "TutorialState.PokemonCapture" flag is done by the above call.
                    if (!client.Player.PlayerResponse.PlayerData.TutorialState.Contains(TutorialState.PokemonCapture)) {
                        TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.PokemonCapture }).Result;
                        SuccessFlag = TutorialResponse.Success;
                    }
                    break;
                case TutorialState.NameSelection:
                    SuccessFlag = false;
                    string suggestedName = client.Username;
                    ClaimCodenameResponse.Types.Status status = ClaimCodenameResponse.Types.Status.CodenameNotValid;
                    for (int i = 0; i < 100; i++)
                    {
                        suggestedName = AvatarSettings.nicknamePrefix + suggestedName + (i == 0 ? "" : i.ToString()) + AvatarSettings.nicknameSufix;
                        status = client.Misc.ClaimCodename(suggestedName).Status;
                        if (status == ClaimCodenameResponse.Types.Status.Success)
                        {
                            TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.NameSelection }).Result;
                            SuccessFlag = TutorialResponse.Success;
                            break;
                        }
                        else if (status == ClaimCodenameResponse.Types.Status.CurrentOwner || status == ClaimCodenameResponse.Types.Status.CodenameChangeNotAllowed) break;
                    }
                    // TODO: check if this is really needed or the "TutorialState.PokemonCapture" flag is done by the above call.
                    if (!client.Player.PlayerResponse.PlayerData.TutorialState.Contains(TutorialState.NameSelection)) {
                        TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.NameSelection }).Result;
                        SuccessFlag = TutorialResponse.Success;
                    }

                    break;
                case TutorialState.PokemonBerry:
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.PokemonBerry }).Result;
                    SuccessFlag = TutorialResponse.Success;
                    break;
                case TutorialState.UseItem:
                    // Need to check how to implement, meanwhile...
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.UseItem }).Result;
                    SuccessFlag = TutorialResponse.Success;
                    break;
                case TutorialState.FirstTimeExperienceComplete:
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.FirstTimeExperienceComplete }).Result;
                    SuccessFlag = TutorialResponse.Success;
                    break;
                case TutorialState.PokestopTutorial:
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.PokestopTutorial }).Result;
                    SuccessFlag = TutorialResponse.Success;
                    break;
                case TutorialState.GymTutorial:
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.GymTutorial }).Result;
                    SuccessFlag = TutorialResponse.Success;
                    break;
            }

            if (SuccessFlag) Logger.Info("[TUTORIAL] " + (int)State + "- " + State + " set DONE");
            else Logger.Warning("[TUTORIAL] " + (int)State + "- " + Enum.GetName(typeof(EncounterTutorialCompleteResponse.Types), State) + " set FAILED. Reason: " + TutorialResponse.Success);

            RandomHelper.RandomDelay(5000).Wait();
            return SuccessFlag;
        }

    }
}
