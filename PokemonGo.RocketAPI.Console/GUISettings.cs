/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 18/01/2017
 * Time: 23:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using PokemonGo.RocketAPI.Logic;
using PokemonGo.RocketAPI.Logic.Shared;

namespace PokemonGo.RocketAPI.Console
{
    /// <summary>
    /// Description of GUISettings.
    /// </summary>
    public class GUISettings : ISettings
    {
        #region ISettings implementation
    public string pFHashKey {
        get;set;
    }
    public string ProfileName {
        get;set;
    }
    public bool IsDefault {
        get;set;
    }
    public PokemonGo.RocketAPI.Enums.AuthType AuthType {
        get;set;
    }
    public string PtcPassword {
        get;set;
    }
    public string PtcUsername {
        get;set;
    }
    public string GoogleUsername {
        get;set;
    }
    public string GooglePassword {
        get;set;
    }
    public bool UsePwdEncryption {
        get;set;
    }
    public double DefaultLatitude {
        get;set;
    }
    public double DefaultLongitude {
        get;set;
    }
    public double DefaultAltitude {
        get;set;
    }
    public bool UseLastCords {
        get;set;
    }
    public bool WalkBackToDefaultLocation {
        get;set;
    }
    public double RelocateDefaultLocationTravelSpeed {
        get;set;
    }
    public bool RelocateDefaultLocation {
        get;set;
    }
    public int MaxWalkingRadiusInMeters {
        get;set;
    }
    public bool FarmPokestops {
        get;set;
    }
    public bool CatchPokemon {
        get;set;





    }
    public bool UseLureAtBreak {
        get;set;





    }
    public bool UseGoogleMapsAPI {
        get;set;





    }
    public bool pauseAtPokeStop {
        get;set;





    }
    public bool BreakAtLure {
        get;set;





    }
    public bool RandomReduceSpeed {
        get;set;





    }
    public bool UseBreakFields {
        get;set;





    }
    public bool Espiral {
        get;set;





    }
    public bool RepeatUserRoute {
        get;set;





    }
    public int XPFarmedLimit {
        get;set;





    }
    public int BreakInterval {
        get;set;





    }
    public int BreakLength {
        get;set;





    }
    public int MinWalkSpeed {
        get;set;





    }
    public int PokemonCatchLimit {
        get;set;





    }
    public int PokestopFarmLimit {
        get;set;





    }
    public int navigation_option {
        get;set;





    }
    public double WalkingSpeedInKilometerPerHour {
        get;set;





    }
    public System.Collections.Generic.LinkedList<System.Device.Location.GeoCoordinate> NextDestinationOverride {
        get;set;





    }
    public System.Collections.Generic.LinkedList<System.Device.Location.GeoCoordinate> RouteToRepeat {
        get;set;





    }
    public bool EvolvePokemonsIfEnoughCandy {
        get;set;





    }
    public bool TransferDoublePokemons {
        get;set;





    }
    public bool TransferFirstLowIV {
        get;set;





    }
    public int DontTransferWithCPOver {
        get;set;





    }
    public bool sleepatpokemons {
        get;set;





    }
    public bool keepPokemonsThatCanEvolve {
        get;set;





    }
    public int HoldMaxDoublePokemons {
        get;set;





    }
    public bool UseLuckyEgg {
        get;set;





    }
    public bool UseRazzBerry {
        get;set;





    }
    public double razzberry_chance {
        get;set;





    }
    public bool UseLuckyEggIfNotRunning {
        get;set;





    }
    public bool UseIncense {
        get;set;





    }
    public bool AutoIncubate {
        get;set;





    }
    public bool UseBasicIncubators {
        get;set;





    }
    public ProxySettings proxySettings {
        get;set;





    }
    public int Pb_Excellent {
        get;set;





    }
    public int Pb_Great {
        get;set;





    }
    public int Pb_Nice {
        get;set;





    }
    public int Pb_Ordinary {
        get;set;





    }
    public bool logPokemons {
        get;set;





    }
    public bool logManualTransfer {
        get;set;





    }
    public bool bLogEvolve {
        get;set;





    }
    public bool logEggs {
        get;set;





    }
    public bool AutoUpdate {
        get;set;





    }
    public bool CheckWhileRunning {
        get;set;





    }
    public int TelegramLiveStatsDelay {
        get;set;





    }
    public string TelegramAPIToken {
        get;set;





    }
    public string TelegramName {
        get;set;





    }
    public bool FarmGyms {
        get;set;





    }
    public bool CollectDailyBonus {
        get;set;





    }
    public bool pokevision {
        get;set;





    }
    public bool LogPokemon {
        get;set;





    }
    public bool LogTransfer {
        get;set;





    }
    public bool LogEvolve {
        get;set;





    }
    public bool LogEggs {
        get;set;





    }
    public bool UseAnimationTimes {
        get;set;





    }
    public bool MapLoaded {
        get;set;





    }
    public bool pauseAtEvolve2 {
        get;set;





    }
    public bool UseLureGUIClick {
        get;set;





    }
    public bool UseIncenseGUIClick {
        get;set;





    }
    public bool UseLuckyEggGUIClick {
        get;set;





    }
    public bool LimitPokeballUse {
        get;set;





    }
    public bool LimitGreatballUse {
        get;set;





    }
    public bool LimitUltraballUse {
        get;set;





    }
    public int MinCPtoCatch {
        get;set;





    }
    public int MinIVtoCatch {
        get;set;





    }
    public int Max_Missed_throws {
        get;set;





    }
    public int MinCPforGreatBall {
        get;set;





    }
    public int MinCPforUltraBall {
        get;set;





    }
    public int InventoryBasePokeball {
        get;set;





    }
    public int InventoryBaseGreatball {
        get;set;





    }
    public int InventoryBaseUltraball {
        get;set;





    }
    public double TimeToRun {
        get;set;





    }
    public int ivmaxpercent {
        get;set;





    }
    public string SelectedLanguage {
        get;set;





    }
    public string GoogleMapsAPIKey {
        get;set;





    }
    public System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<POGOProtos.Inventory.Item.ItemId, int>> itemRecycleFilter {
        get;set;





    }
    public int MaxPokeballs {
        get;set;





    }
    public int MaxGreatballs {
        get;set;





    }
    public int MaxUltraballs {
        get;set;





    }
    public int MaxRevives {
        get;set;





    }
    public int MaxTopRevives {
        get;set;





    }
    public int MaxPotions {
        get;set;





    }
    public int MaxSuperPotions {
        get;set;





    }
    public int MaxHyperPotions {
        get;set;





    }
    public int MaxTopPotions {
        get;set;





    }
    public int MaxBerries {
        get;set;





    }
    public System.Collections.Generic.List<POGOProtos.Enums.PokemonId> pokemonsToHold {
        get;set;





    }
    public System.Collections.Generic.List<POGOProtos.Enums.PokemonId> pokemonsToEvolve {
        get;set;





    }
    public System.Collections.Generic.List<POGOProtos.Enums.PokemonId> catchPokemonSkipList {
        get;set;





    }
    public bool AvoidRegionLock {
        get;set;





    }
    public bool NextBestBallOnEscape {
        get;set;





    }
    public bool SnipePokemon {
        get;set;





    }
    public System.Collections.Generic.List<POGOProtos.Enums.PokemonId> NotToSnipe {
        get;set;





    }
    public ManualSnipePokemon SnipeOpts {
        get;set;





    }
    public Google.Protobuf.ByteString SessionHash {
        get;set;





    }
    public bool No10kmEggs {
        get;set;





    }
    public bool No5kmEggs {
        get;set;





    }
    public bool No2kmEggs {
        get;set;





    }
    public bool EggsAscendingSelection {
        get;set;





    }
    public bool No10kmEggsBasicInc {
        get;set;





    }
    public bool No5kmEggsBasicInc {
        get;set;





    }
    public bool No2kmEggsBasicInc {
        get;set;





    }
    public bool EggsAscendingSelectionBasicInc {
        get;set;





    }
    public bool EnableVerboseLogging {
        get;set;





    }
    public bool EnableConsoleInTab {
        get;set;


    }
    public bool PauseTheWalking {
        get;set;
    }
    public bool EnablePokeList {
        get;set;
    }
    public bool simulatedPGO {
        get;set;
    }
    public bool pauseAtEvolve {
        get;set;
    }

    #endregion
        
    }
}
