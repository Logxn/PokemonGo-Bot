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

        public bool MarkTutorialAsDone(TutorialState State, PokemonGo.RocketAPI.Client client, int firstPokemon = 4) // A Charmander
        {
            MarkTutorialCompleteResponse TutorialResponse = null;
            EncounterTutorialCompleteResponse EncounterResponse = null;
            SetAvatarResponse AvatarResponse = null;
            bool SuccessFlag = false;

            switch (State) {
                case TutorialState.LegalScreen:
                    TutorialResponse = client.Misc.AceptLegalScreen().Result;
                    SuccessFlag = TutorialResponse.Success;
                    break;
                case TutorialState.AvatarSelection:
                    /* TODO THIS NEEDS MORE WORK
                    //AvatarSettings.Load();
                    var playerAvatar = new PlayerAvatar();
                    playerAvatar.Skin = AvatarSettings.skin == 0 ? RandomHelper.RandomNumber(0, 4) : AvatarSettings.skin;
                    //playerAvatar.AvatarNecklace = AvatarSettings.necklace = ...
                    //playerAvatar.AvatarBelt = AvatarSettings.belt == ...
                    //playerAvatar.AvatarSocks = AvatarSettings.socks == ...
                    //playerAvatar.AvatarGloves = AvatarSettings.gloves == ...
                    playerAvatar.Backpack = AvatarSettings.backpack == 0 ? RandomHelper.RandomNumber(0, 3) : AvatarSettings.backpack;
                    playerAvatar.Eyes = AvatarSettings.eyes == 0 ? RandomHelper.RandomNumber(0, 4) : AvatarSettings.eyes;
                    playerAvatar.Shoes = AvatarSettings.shoes == 0 ? RandomHelper.RandomNumber(0, 3) : AvatarSettings.shoes;
                    playerAvatar.Hat = AvatarSettings.hat == 0 ? RandomHelper.RandomNumber(0, 3) : AvatarSettings.hat;
                    playerAvatar.Pants = AvatarSettings.pants == 0 ? RandomHelper.RandomNumber(0, 3) : AvatarSettings.pants;
                    //playerAvatar.AvatarGlasses = AvatarSettings.glasses == ...
                    playerAvatar.Hair = AvatarSettings.hair == 0 ? RandomHelper.RandomNumber(0, 6) : AvatarSettings.hair;
                    playerAvatar.Shirt = AvatarSettings.shirt == 0 ? RandomHelper.RandomNumber(0, 3) : AvatarSettings.shirt;
                    playerAvatar.Avatar = AvatarSettings.Gender == 0 ? RandomHelper.RandomNumber(1, 3) : AvatarSettings.Gender;

                    AvatarResponse = client.Player.SetAvatar(playerAvatar).Result;
                    SuccessFlag = Convert.ToBoolean(AvatarResponse.Status.ToString() == "Success" ? 1:0);
                    */
                    SuccessFlag = true;
                    break;
                case TutorialState.AccountCreation:
                    // Need to check how to implement, meanwhile...
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.AccountCreation }).Result;
                    SuccessFlag = TutorialResponse.Success;
                    break;
                case TutorialState.PokemonCapture:
                    if (AvatarSettings.starter == (POGOProtos.Enums.PokemonId)0) AvatarSettings.starter = (POGOProtos.Enums.PokemonId)firstPokemon;
                    EncounterResponse = client.Encounter.EncounterTutorialComplete(AvatarSettings.starter);
                    if (EncounterResponse.Result == EncounterTutorialCompleteResponse.Types.Result.ErrorInvalidPokemon) EncounterResponse = client.Encounter.EncounterTutorialComplete(POGOProtos.Enums.PokemonId.Charmander);
                    SuccessFlag = Convert.ToBoolean(EncounterResponse.Result);
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
