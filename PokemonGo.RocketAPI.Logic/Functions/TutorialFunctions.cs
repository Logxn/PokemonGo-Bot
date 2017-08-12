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

        //PokemonBerry = 5,
        //UseItem = 6,

        //FirstTimeExperienceComplete = 7, --> Implemented
        //PokestopTutorial = 8, --> Implemented

        //GymTutorial = 9 --> Implemented

        public bool MarkTutorialAsDone(TutorialState State, PokemonGo.RocketAPI.Client client)
        {
            EncounterTutorialCompleteResponse TutorialResponse = null;
            SetAvatarResponse AvatarResponse = null;
            bool SuccessFlag = false;

            switch (State) {
                case TutorialState.LegalScreen:
                    TutorialResponse = client.Misc.AceptLegalScreen().Result;
                    SuccessFlag = Convert.ToBoolean(TutorialResponse.Result);
                    break;
                case TutorialState.AvatarSelection:
                    //AvatarSettings.Load();
                    var playerAvatar = new PlayerAvatar();
                    playerAvatar.Avatar = AvatarSettings.Gender == 2 ? RandomHelper.RandomNumber(0, 2) : AvatarSettings.Gender;
                    playerAvatar.Backpack = AvatarSettings.backpack == 3 ? RandomHelper.RandomNumber(0, 3) : AvatarSettings.backpack;
                    playerAvatar.Eyes = AvatarSettings.eyes == 4 ? RandomHelper.RandomNumber(0, 4) : AvatarSettings.eyes;
                    playerAvatar.Hair = AvatarSettings.hair == 6 ? RandomHelper.RandomNumber(0, 6) : AvatarSettings.hair;
                    playerAvatar.Hat = AvatarSettings.hat == 3 ? RandomHelper.RandomNumber(0, 3) : AvatarSettings.hat;
                    playerAvatar.Pants = AvatarSettings.pants == 3 ? RandomHelper.RandomNumber(0, 3) : AvatarSettings.pants;
                    playerAvatar.Shirt = AvatarSettings.shirt == 3 ? RandomHelper.RandomNumber(0, 3) : AvatarSettings.shirt;
                    playerAvatar.Shoes = AvatarSettings.shoes == 3 ? RandomHelper.RandomNumber(0, 3) : AvatarSettings.shoes;
                    playerAvatar.Skin = AvatarSettings.skin == 4 ? RandomHelper.RandomNumber(0, 4) : AvatarSettings.skin;
                    AvatarResponse = client.Player.SetAvatar(playerAvatar).Result;
                    SuccessFlag = Convert.ToBoolean(TutorialResponse.Result);
                    break;
                case TutorialState.AccountCreation:
                    // Need to check how to implement, meanwhile...
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.AccountCreation }).Result;
                    SuccessFlag = Convert.ToBoolean(TutorialResponse.Result);
                    break;
                case TutorialState.PokemonCapture:
                    TutorialResponse = client.Encounter.EncounterTutorialComplete(AvatarSettings.starter);
                    SuccessFlag = Convert.ToBoolean(TutorialResponse.Result);
                    break;
                case TutorialState.NameSelection:
                    SuccessFlag = false;
                    ClaimCodenameResponse.Types.Status status = ClaimCodenameResponse.Types.Status.CodenameNotValid;
                    for (int i = 0; i < 100; i++)
                    {
                        string suggestedName = AvatarSettings.nicknamePrefix + (i == 0 ? "" : i.ToString()) + AvatarSettings.nicknameSufix;
                        status = client.Misc.ClaimCodename(suggestedName).Status;
                        if (status == ClaimCodenameResponse.Types.Status.Success)
                        {
                            TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.NameSelection }).Result;
                            SuccessFlag = Convert.ToBoolean(TutorialResponse.Result);
                            break;
                        }
                        else if (status == ClaimCodenameResponse.Types.Status.CurrentOwner || status == ClaimCodenameResponse.Types.Status.CodenameChangeNotAllowed) break;
                    }
                    break;
                case TutorialState.PokemonBerry:
                    // Need to check how to implement, meanwhile...
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.PokemonBerry }).Result;
                    SuccessFlag = Convert.ToBoolean(TutorialResponse.Result);
                    break;
                case TutorialState.UseItem:
                    // Need to check how to implement, meanwhile...
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.UseItem }).Result;
                    SuccessFlag = Convert.ToBoolean(TutorialResponse.Result);
                    break;
                case TutorialState.FirstTimeExperienceComplete:
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.FirstTimeExperienceComplete }).Result;
                    SuccessFlag = Convert.ToBoolean(TutorialResponse.Result);
                    break;
                case TutorialState.PokestopTutorial:
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.PokestopTutorial }).Result;
                    SuccessFlag = Convert.ToBoolean(TutorialResponse.Result);
                    break;
                case TutorialState.GymTutorial:
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.GymTutorial }).Result;
                    SuccessFlag = Convert.ToBoolean(TutorialResponse.Result);
                    break;
            }

            if (SuccessFlag) Logger.Debug("[TUTORIAL] " + (int)State + "- " + State + " set DONE");
            else Logger.Warning("[TUTORIAL] " + (int)State + "- " + Enum.GetName(typeof(EncounterTutorialCompleteResponse.Types), State) + " set FAILED. Reason: " + TutorialResponse.Result);

            RandomHelper.RandomDelay(5000).Wait();
            return SuccessFlag;
        }

    }
}
