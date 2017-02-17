// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: POGOProtos/Enums/BadgeType.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace POGOProtos.Enums
{

    /// <summary>Holder for reflection information generated from POGOProtos/Enums/BadgeType.proto</summary>
    public static partial class BadgeTypeReflection
    {

        #region Descriptor
        /// <summary>File descriptor for POGOProtos/Enums/BadgeType.proto</summary>
        public static pbr::FileDescriptor Descriptor
        {
            get { return descriptor; }
        }
        private static pbr::FileDescriptor descriptor;

        static BadgeTypeReflection()
        {
            byte[] descriptorData = global::System.Convert.FromBase64String(
                string.Concat(
                  "CiBQT0dPUHJvdG9zL0VudW1zL0JhZGdlVHlwZS5wcm90bxIQUE9HT1Byb3Rv",
                  "cy5FbnVtcyrSBwoJQmFkZ2VUeXBlEg8KC0JBREdFX1VOU0VUEAASEwoPQkFE",
                  "R0VfVFJBVkVMX0tNEAESGQoVQkFER0VfUE9LRURFWF9FTlRSSUVTEAISFwoT",
                  "QkFER0VfQ0FQVFVSRV9UT1RBTBADEhcKE0JBREdFX0RFRkVBVEVEX0ZPUlQQ",
                  "BBIXChNCQURHRV9FVk9MVkVEX1RPVEFMEAUSFwoTQkFER0VfSEFUQ0hFRF9U",
                  "T1RBTBAGEhsKF0JBREdFX0VOQ09VTlRFUkVEX1RPVEFMEAcSGwoXQkFER0Vf",
                  "UE9LRVNUT1BTX1ZJU0lURUQQCBIaChZCQURHRV9VTklRVUVfUE9LRVNUT1BT",
                  "EAkSGQoVQkFER0VfUE9LRUJBTExfVEhST1dOEAoSFgoSQkFER0VfQklHX01B",
                  "R0lLQVJQEAsSGAoUQkFER0VfREVQTE9ZRURfVE9UQUwQDBIbChdCQURHRV9C",
                  "QVRUTEVfQVRUQUNLX1dPThANEh0KGUJBREdFX0JBVFRMRV9UUkFJTklOR19X",
                  "T04QDhIbChdCQURHRV9CQVRUTEVfREVGRU5EX1dPThAPEhkKFUJBREdFX1BS",
                  "RVNUSUdFX1JBSVNFRBAQEhoKFkJBREdFX1BSRVNUSUdFX0RST1BQRUQQERIV",
                  "ChFCQURHRV9UWVBFX05PUk1BTBASEhcKE0JBREdFX1RZUEVfRklHSFRJTkcQ",
                  "ExIVChFCQURHRV9UWVBFX0ZMWUlORxAUEhUKEUJBREdFX1RZUEVfUE9JU09O",
                  "EBUSFQoRQkFER0VfVFlQRV9HUk9VTkQQFhITCg9CQURHRV9UWVBFX1JPQ0sQ",
                  "FxISCg5CQURHRV9UWVBFX0JVRxAYEhQKEEJBREdFX1RZUEVfR0hPU1QQGRIU",
                  "ChBCQURHRV9UWVBFX1NURUVMEBoSEwoPQkFER0VfVFlQRV9GSVJFEBsSFAoQ",
                  "QkFER0VfVFlQRV9XQVRFUhAcEhQKEEJBREdFX1RZUEVfR1JBU1MQHRIXChNC",
                  "QURHRV9UWVBFX0VMRUNUUklDEB4SFgoSQkFER0VfVFlQRV9QU1lDSElDEB8S",
                  "EgoOQkFER0VfVFlQRV9JQ0UQIBIVChFCQURHRV9UWVBFX0RSQUdPThAhEhMK",
                  "D0JBREdFX1RZUEVfREFSSxAiEhQKEEJBREdFX1RZUEVfRkFJUlkQIxIXChNC",
                  "QURHRV9TTUFMTF9SQVRUQVRBECQSEQoNQkFER0VfUElLQUNIVRAlEg8KC0JB",
                  "REdFX1VOT1dOECYSHgoaQkFER0VfUE9LRURFWF9FTlRSSUVTX0dFTjIQJ2IG",
                  "cHJvdG8z"));
            descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
                new pbr::FileDescriptor[] { },
                new pbr::GeneratedClrTypeInfo(new[] { typeof(global::POGOProtos.Enums.BadgeType), }, null));
        }
        #endregion

    }
    #region Enums
    public enum BadgeType
    {
        [pbr::OriginalName("BADGE_UNSET")]
        BadgeUnset = 0,
        [pbr::OriginalName("BADGE_TRAVEL_KM")]
        BadgeTravelKm = 1,
        [pbr::OriginalName("BADGE_POKEDEX_ENTRIES")]
        BadgePokedexEntries = 2,
        [pbr::OriginalName("BADGE_CAPTURE_TOTAL")]
        BadgeCaptureTotal = 3,
        [pbr::OriginalName("BADGE_DEFEATED_FORT")]
        BadgeDefeatedFort = 4,
        [pbr::OriginalName("BADGE_EVOLVED_TOTAL")]
        BadgeEvolvedTotal = 5,
        [pbr::OriginalName("BADGE_HATCHED_TOTAL")]
        BadgeHatchedTotal = 6,
        [pbr::OriginalName("BADGE_ENCOUNTERED_TOTAL")]
        BadgeEncounteredTotal = 7,
        [pbr::OriginalName("BADGE_POKESTOPS_VISITED")]
        BadgePokestopsVisited = 8,
        [pbr::OriginalName("BADGE_UNIQUE_POKESTOPS")]
        BadgeUniquePokestops = 9,
        [pbr::OriginalName("BADGE_POKEBALL_THROWN")]
        BadgePokeballThrown = 10,
        [pbr::OriginalName("BADGE_BIG_MAGIKARP")]
        BadgeBigMagikarp = 11,
        [pbr::OriginalName("BADGE_DEPLOYED_TOTAL")]
        BadgeDeployedTotal = 12,
        [pbr::OriginalName("BADGE_BATTLE_ATTACK_WON")]
        BadgeBattleAttackWon = 13,
        [pbr::OriginalName("BADGE_BATTLE_TRAINING_WON")]
        BadgeBattleTrainingWon = 14,
        [pbr::OriginalName("BADGE_BATTLE_DEFEND_WON")]
        BadgeBattleDefendWon = 15,
        [pbr::OriginalName("BADGE_PRESTIGE_RAISED")]
        BadgePrestigeRaised = 16,
        [pbr::OriginalName("BADGE_PRESTIGE_DROPPED")]
        BadgePrestigeDropped = 17,
        [pbr::OriginalName("BADGE_TYPE_NORMAL")]
        Normal = 18,
        [pbr::OriginalName("BADGE_TYPE_FIGHTING")]
        Fighting = 19,
        [pbr::OriginalName("BADGE_TYPE_FLYING")]
        Flying = 20,
        [pbr::OriginalName("BADGE_TYPE_POISON")]
        Poison = 21,
        [pbr::OriginalName("BADGE_TYPE_GROUND")]
        Ground = 22,
        [pbr::OriginalName("BADGE_TYPE_ROCK")]
        Rock = 23,
        [pbr::OriginalName("BADGE_TYPE_BUG")]
        Bug = 24,
        [pbr::OriginalName("BADGE_TYPE_GHOST")]
        Ghost = 25,
        [pbr::OriginalName("BADGE_TYPE_STEEL")]
        Steel = 26,
        [pbr::OriginalName("BADGE_TYPE_FIRE")]
        Fire = 27,
        [pbr::OriginalName("BADGE_TYPE_WATER")]
        Water = 28,
        [pbr::OriginalName("BADGE_TYPE_GRASS")]
        Grass = 29,
        [pbr::OriginalName("BADGE_TYPE_ELECTRIC")]
        Electric = 30,
        [pbr::OriginalName("BADGE_TYPE_PSYCHIC")]
        Psychic = 31,
        [pbr::OriginalName("BADGE_TYPE_ICE")]
        Ice = 32,
        [pbr::OriginalName("BADGE_TYPE_DRAGON")]
        Dragon = 33,
        [pbr::OriginalName("BADGE_TYPE_DARK")]
        Dark = 34,
        [pbr::OriginalName("BADGE_TYPE_FAIRY")]
        Fairy = 35,
        [pbr::OriginalName("BADGE_SMALL_RATTATA")]
        BadgeSmallRattata = 36,
        [pbr::OriginalName("BADGE_PIKACHU")]
        BadgePikachu = 37,
        [pbr::OriginalName("BADGE_UNOWN")]
        BadgeUnown = 38,
        [pbr::OriginalName("BADGE_POKEDEX_ENTRIES_GEN2")]
        BadgePokedexEntriesGen2 = 39,
    }

    #endregion

}

#endregion Designer generated code