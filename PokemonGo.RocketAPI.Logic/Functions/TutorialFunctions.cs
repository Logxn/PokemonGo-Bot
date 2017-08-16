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
using POGOProtos.Networking.Requests;
using PokemonGo.RocketAPI.Rpc;

namespace PokeMaster.Logic.Functions
{
    public class TutorialFunctions
    {
        //LegalScreen = 0,
        //AvatarSelection = 1,
        //AccountCreation = 2,
        //PokemonCapture = 3,
        //NameSelection = 4,
        //PokemonBerry = 5,
        //UseItem = 6,
        //FirstTimeExperienceComplete = 7,
        //PokestopTutorial = 8,
        //GymTutorial = 9
        /*
         * On first connection before any move you must do 0,1,3,4 & 7
         *
         */

        public bool MarkTutorialAsDone(TutorialState State, PokemonGo.RocketAPI.Client client, PokemonId firstPokemon = PokemonId.Charmander)
        {
            MarkTutorialCompleteResponse TutorialResponse = null;
            SetAvatarResponse AvatarResponse = null;
            bool SuccessFlag = false;

            switch (State) {
                /* 0 */
                case TutorialState.LegalScreen:
                    RandomHelper.RandomSleep(4000, 7000);
                    TutorialResponse = client.Misc.AceptLegalScreen().Result;
                    client.Player.GetPlayer();
                    SuccessFlag = TutorialResponse.Success;
                    break;
                /* 1 */
                case TutorialState.AvatarSelection:
                    // All MAX values will be used to get a random value 
                    // RandormNumber never returns max value.
                    AvatarSettings.Load();
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

                    RandomHelper.RandomSleep(8000, 14500);
                    AvatarResponse = client.Player.SetAvatar(playerAvatar).Result;

                    client.Player.ListAvatarCustomizations(playerAvatar.Avatar == 0 ? PlayerAvatarType.PlayerAvatarMale:PlayerAvatarType.PlayerAvatarFemale, null, null , 0, 0);

                    RandomHelper.RandomSleep(1000, 1700);
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.AvatarSelection }).Result;

                    client.Player.GetPlayerProfile();
                    SuccessFlag = TutorialResponse.Success;
                    break;
                /* 2 */
                case TutorialState.AccountCreation:
                    // Need to check how to implement, meanwhile...
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.AccountCreation }).Result;
                    SuccessFlag = TutorialResponse.Success;
                    break;
                /* 3 */
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

                    /*
                    batch.getDownloadURLs([
                        this.state.assets.getFullIdFromId('1a3c2816-65fa-4b97-90eb-0b301c064b7a'),
                        this.state.assets.getFullIdFromId('aa8f7687-a022-4773-b900-3a8c170e9aea'),
                        this.state.assets.getFullIdFromId('e89109b0-9a54-40fe-8431-12f7826c8194'),
                    ]);*/
                    CommonRequest.GetDownloadUrlsRequest(client); // Not fully implemented, you need to send the ID's

                    RandomHelper.RandomSleep(10000, 13000);
                    var encounterResponse = client.Encounter.EncounterTutorialComplete(AvatarSettings.starter);
                    if (encounterResponse.Result == EncounterTutorialCompleteResponse.Types.Result.ErrorInvalidPokemon) 
                        encounterResponse = client.Encounter.EncounterTutorialComplete(PokemonId.Charmander);
                    SuccessFlag = (encounterResponse.Result == EncounterTutorialCompleteResponse.Types.Result.Success);

                    client.Player.GetPlayerProfile();

                    // TODO: check if this is really needed or the "TutorialState.PokemonCapture" flag is done by the above call.
                    if (!client.Player.PlayerResponse.PlayerData.TutorialState.Contains(TutorialState.PokemonCapture)) {
                        TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.PokemonCapture }).Result;
                        SuccessFlag = TutorialResponse.Success;
                    }
                    break;
            /* 4 */
                case TutorialState.NameSelection:
                    SuccessFlag = false;
                    string suggestedName = client.Username;
                    ClaimCodenameResponse.Types.Status status = ClaimCodenameResponse.Types.Status.CodenameNotValid;
                    for (int i = 0; i < 100; i++)
                    {
                        suggestedName = AvatarSettings.nicknamePrefix + suggestedName + (i == 0 ? "" : i.ToString()) + AvatarSettings.nicknameSufix;
                        RandomHelper.RandomSleep(7000, 13500);
                        status = client.Misc.ClaimCodename(suggestedName).Status;
                        if (status == ClaimCodenameResponse.Types.Status.Success) break;
                        else if (status == ClaimCodenameResponse.Types.Status.CurrentOwner || status == ClaimCodenameResponse.Types.Status.CodenameChangeNotAllowed) break;
                    }

                    client.Player.GetPlayer();

                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.NameSelection }).Result;
                    SuccessFlag = TutorialResponse.Success;

                    break;
                /* 5 */
                case TutorialState.PokemonBerry:
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.PokemonBerry }).Result;
                    SuccessFlag = TutorialResponse.Success;
                    break;
                /* 6 */
                case TutorialState.UseItem:
                    // Need to check how to implement, meanwhile...
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.UseItem }).Result;
                    SuccessFlag = TutorialResponse.Success;
                    break;
                /* 7 */
                case TutorialState.FirstTimeExperienceComplete:
                    RandomHelper.RandomSleep(3500, 6000);
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.FirstTimeExperienceComplete }).Result;
                    SuccessFlag = TutorialResponse.Success;
                    break;
                /* 8 */
                case TutorialState.PokestopTutorial:
                    TutorialResponse = client.Misc.MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.PokestopTutorial }).Result;
                    SuccessFlag = TutorialResponse.Success;
                    break;
                /* 9 */
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
