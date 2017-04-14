// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: POGOProtos/Settings/TranslationSettings.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace POGOProtos.Settings {

  /// <summary>Holder for reflection information generated from POGOProtos/Settings/TranslationSettings.proto</summary>
  public static partial class TranslationSettingsReflection {

    #region Descriptor
    /// <summary>File descriptor for POGOProtos/Settings/TranslationSettings.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static TranslationSettingsReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Ci1QT0dPUHJvdG9zL1NldHRpbmdzL1RyYW5zbGF0aW9uU2V0dGluZ3MucHJv",
            "dG8SE1BPR09Qcm90b3MuU2V0dGluZ3MiNQoTVHJhbnNsYXRpb25TZXR0aW5n",
            "cxIeChZ0cmFuc2xhdGlvbl9idW5kbGVfaWRzGAEgAygJYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::POGOProtos.Settings.TranslationSettings), global::POGOProtos.Settings.TranslationSettings.Parser, new[]{ "TranslationBundleIds" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class TranslationSettings : pb::IMessage<TranslationSettings> {
    private static readonly pb::MessageParser<TranslationSettings> _parser = new pb::MessageParser<TranslationSettings>(() => new TranslationSettings());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<TranslationSettings> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::POGOProtos.Settings.TranslationSettingsReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TranslationSettings() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TranslationSettings(TranslationSettings other) : this() {
      translationBundleIds_ = other.translationBundleIds_.Clone();
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TranslationSettings Clone() {
      return new TranslationSettings(this);
    }

    /// <summary>Field number for the "translation_bundle_ids" field.</summary>
    public const int TranslationBundleIdsFieldNumber = 1;
    private static readonly pb::FieldCodec<string> _repeated_translationBundleIds_codec
        = pb::FieldCodec.ForString(10);
    private readonly pbc::RepeatedField<string> translationBundleIds_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<string> TranslationBundleIds {
      get { return translationBundleIds_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as TranslationSettings);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(TranslationSettings other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!translationBundleIds_.Equals(other.translationBundleIds_)) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= translationBundleIds_.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      translationBundleIds_.WriteTo(output, _repeated_translationBundleIds_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += translationBundleIds_.CalculateSize(_repeated_translationBundleIds_codec);
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(TranslationSettings other) {
      if (other == null) {
        return;
      }
      translationBundleIds_.Add(other.translationBundleIds_);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            translationBundleIds_.AddEntriesFrom(input, _repeated_translationBundleIds_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
