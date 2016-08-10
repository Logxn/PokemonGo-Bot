// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: POGOProtos/Settings/Master/TypeEffectiveSettings.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace POGOProtos.Settings.Master {

  /// <summary>Holder for reflection information generated from POGOProtos/Settings/Master/TypeEffectiveSettings.proto</summary>
  public static partial class TypeEffectiveSettingsReflection {

    #region Descriptor
    /// <summary>File descriptor for POGOProtos/Settings/Master/TypeEffectiveSettings.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static TypeEffectiveSettingsReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CjZQT0dPUHJvdG9zL1NldHRpbmdzL01hc3Rlci9UeXBlRWZmZWN0aXZlU2V0",
            "dGluZ3MucHJvdG8SGlBPR09Qcm90b3MuU2V0dGluZ3MuTWFzdGVyGiJQT0dP",
            "UHJvdG9zL0VudW1zL1Bva2Vtb25UeXBlLnByb3RvImIKFVR5cGVFZmZlY3Rp",
            "dmVTZXR0aW5ncxIVCg1hdHRhY2tfc2NhbGFyGAEgAygCEjIKC2F0dGFja190",
            "eXBlGAIgASgOMh0uUE9HT1Byb3Rvcy5FbnVtcy5Qb2tlbW9uVHlwZWIGcHJv",
            "dG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::POGOProtos.Enums.PokemonTypeReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::POGOProtos.Settings.Master.TypeEffectiveSettings), global::POGOProtos.Settings.Master.TypeEffectiveSettings.Parser, new[]{ "AttackScalar", "AttackType" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class TypeEffectiveSettings : pb::IMessage<TypeEffectiveSettings> {
    private static readonly pb::MessageParser<TypeEffectiveSettings> _parser = new pb::MessageParser<TypeEffectiveSettings>(() => new TypeEffectiveSettings());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<TypeEffectiveSettings> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::POGOProtos.Settings.Master.TypeEffectiveSettingsReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TypeEffectiveSettings() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TypeEffectiveSettings(TypeEffectiveSettings other) : this() {
      attackScalar_ = other.attackScalar_.Clone();
      attackType_ = other.attackType_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TypeEffectiveSettings Clone() {
      return new TypeEffectiveSettings(this);
    }

    /// <summary>Field number for the "attack_scalar" field.</summary>
    public const int AttackScalarFieldNumber = 1;
    private static readonly pb::FieldCodec<float> _repeated_attackScalar_codec
        = pb::FieldCodec.ForFloat(10);
    private readonly pbc::RepeatedField<float> attackScalar_ = new pbc::RepeatedField<float>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<float> AttackScalar {
      get { return attackScalar_; }
    }

    /// <summary>Field number for the "attack_type" field.</summary>
    public const int AttackTypeFieldNumber = 2;
    private global::POGOProtos.Enums.PokemonType attackType_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::POGOProtos.Enums.PokemonType AttackType {
      get { return attackType_; }
      set {
        attackType_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as TypeEffectiveSettings);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(TypeEffectiveSettings other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!attackScalar_.Equals(other.attackScalar_)) return false;
      if (AttackType != other.AttackType) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= attackScalar_.GetHashCode();
      if (AttackType != 0) hash ^= AttackType.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      attackScalar_.WriteTo(output, _repeated_attackScalar_codec);
      if (AttackType != 0) {
        output.WriteRawTag(16);
        output.WriteEnum((int) AttackType);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += attackScalar_.CalculateSize(_repeated_attackScalar_codec);
      if (AttackType != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) AttackType);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(TypeEffectiveSettings other) {
      if (other == null) {
        return;
      }
      attackScalar_.Add(other.attackScalar_);
      if (other.AttackType != 0) {
        AttackType = other.AttackType;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10:
          case 13: {
            attackScalar_.AddEntriesFrom(input, _repeated_attackScalar_codec);
            break;
          }
          case 16: {
            attackType_ = (global::POGOProtos.Enums.PokemonType) input.ReadEnum();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
