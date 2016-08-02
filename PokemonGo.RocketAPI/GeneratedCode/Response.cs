#pragma warning disable 1591, 0612, 3021

#region Designer generated code

#region

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;

#endregion

namespace PokemonGo.RocketAPI.GeneratedCode
{
    /// <summary>Holder for reflection information generated from Response.proto</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public static partial class ResponseReflection
    {
        #region Descriptor

        /// <summary>File descriptor for Response.proto</summary>
        public static pbr::FileDescriptor Descriptor
        {
            get { return descriptor; }
        }

        private static pbr::FileDescriptor descriptor;

        static ResponseReflection()
        {
            var descriptorData = global::System.Convert.FromBase64String(
                string.Concat(
                    "Cg5SZXNwb25zZS5wcm90bxIhUG9rZW1vbkdvLlJvY2tldEFQSS5HZW5lcmF0",
                    "ZWRDb2RlIrYFCghSZXNwb25zZRIQCgh1bmtub3duMRgBIAEoBRIQCgh1bmtu",
                    "b3duMhgCIAEoAxIPCgdhcGlfdXJsGAMgASgJEkYKCHVua25vd242GAYgASgL",
                    "MjQuUG9rZW1vbkdvLlJvY2tldEFQSS5HZW5lcmF0ZWRDb2RlLlJlc3BvbnNl",
                    "LlVua25vd242Ej4KBGF1dGgYByABKAsyMC5Qb2tlbW9uR28uUm9ja2V0QVBJ",
                    "LkdlbmVyYXRlZENvZGUuUmVzcG9uc2UuQXV0aBIPCgdwYXlsb2FkGGQgAygM",
                    "EhQKDGVycm9yTWVzc2FnZRhlIAEoCRqLAQoIVW5rbm93bjYSEAoIdW5rbm93",
                    "bjEYASABKAUSTwoIdW5rbm93bjIYAiABKAsyPS5Qb2tlbW9uR28uUm9ja2V0",
                    "QVBJLkdlbmVyYXRlZENvZGUuUmVzcG9uc2UuVW5rbm93bjYuVW5rbm93bjIa",
                    "HAoIVW5rbm93bjISEAoIdW5rbm93bjEYASABKAwaPwoEQXV0aBIRCgl1bmtu",
                    "b3duNzEYASABKAwSEQoJdGltZXN0YW1wGAIgASgDEhEKCXVua25vd243MxgD",
                    "IAEoDBr2AQocUmVjeWNsZUludmVudG9yeUl0ZW1SZXNwb25zZRJfCgZyZXN1",
                    "bHQYASABKA4yTy5Qb2tlbW9uR28uUm9ja2V0QVBJLkdlbmVyYXRlZENvZGUu",
                    "UmVzcG9uc2UuUmVjeWNsZUludmVudG9yeUl0ZW1SZXNwb25zZS5SZXN1bHQS",
                    "EQoJbmV3X2NvdW50GAIgASgFImIKBlJlc3VsdBIJCgVVTlNFVBAAEgsKB1NV",
                    "Q0NFU1MQARIbChdFUlJPUl9OT1RfRU5PVUdIX0NPUElFUxACEiMKH0VSUk9S",
                    "X0NBTk5PVF9SRUNZQ0xFX0lOQ1VCQVRPUlMQA2IGcHJvdG8z"));
            descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
                new pbr::FileDescriptor[] { },
                new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[]
                {
                    new pbr::GeneratedClrTypeInfo(typeof(global::PokemonGo.RocketAPI.GeneratedCode.Response),
                        global::PokemonGo.RocketAPI.GeneratedCode.Response.Parser,
                        new[] {"Unknown1", "Unknown2", "ApiUrl", "Unknown6", "Auth", "Payload", "ErrorMessage"}, null,
                        null, new pbr::GeneratedClrTypeInfo[]
                        {
                            new pbr::GeneratedClrTypeInfo(
                                typeof(global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Unknown6),
                                global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Unknown6.Parser,
                                new[] {"Unknown1", "Unknown2"}, null, null,
                                new pbr::GeneratedClrTypeInfo[]
                                {
                                    new pbr::GeneratedClrTypeInfo(
                                        typeof(
                                            global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Unknown6.Types.
                                                Unknown2),
                                        global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Unknown6.Types.Unknown2
                                            .Parser, new[] {"Unknown1"}, null, null, null)
                                }),
                            new pbr::GeneratedClrTypeInfo(
                                typeof(global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Auth),
                                global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Auth.Parser,
                                new[] {"Unknown71", "Timestamp", "Unknown73"}, null, null, null),
                            new pbr::GeneratedClrTypeInfo(
                                typeof(
                                    global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.
                                        RecycleInventoryItemResponse),
                                global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.RecycleInventoryItemResponse
                                    .Parser, new[] {"Result", "NewCount"}, null,
                                new[]
                                {
                                    typeof(
                                        global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.
                                            RecycleInventoryItemResponse.Types.Result)
                                }, null)
                        })
                }));
        }

        #endregion
    }

    #region Messages

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public sealed partial class Response : pb::IMessage<Response>
    {
        /// <summary>Field number for the "unknown1" field.</summary>
        public const int Unknown1FieldNumber = 1;

        /// <summary>Field number for the "unknown2" field.</summary>
        public const int Unknown2FieldNumber = 2;

        /// <summary>Field number for the "api_url" field.</summary>
        public const int ApiUrlFieldNumber = 3;

        /// <summary>Field number for the "unknown6" field.</summary>
        public const int Unknown6FieldNumber = 6;

        /// <summary>Field number for the "auth" field.</summary>
        public const int AuthFieldNumber = 7;

        /// <summary>Field number for the "payload" field.</summary>
        public const int PayloadFieldNumber = 100;

        /// <summary>Field number for the "errorMessage" field.</summary>
        public const int ErrorMessageFieldNumber = 101;

        private static readonly pb::MessageParser<Response> _parser =
            new pb::MessageParser<Response>(() => new Response());

        private static readonly pb::FieldCodec<pb::ByteString> _repeated_payload_codec
            = pb::FieldCodec.ForBytes(802);

        private readonly pbc::RepeatedField<pb::ByteString> payload_ = new pbc::RepeatedField<pb::ByteString>();
        private string apiUrl_ = "";
        private global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Auth auth_;
        private string errorMessage_ = "";
        private int unknown1_;
        private long unknown2_;
        private global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Unknown6 unknown6_;

        public Response()
        {
            OnConstruction();
        }

        public Response(Response other) : this()
        {
            unknown1_ = other.unknown1_;
            unknown2_ = other.unknown2_;
            apiUrl_ = other.apiUrl_;
            Unknown6 = other.unknown6_ != null ? other.Unknown6.Clone() : null;
            Auth = other.auth_ != null ? other.Auth.Clone() : null;
            payload_ = other.payload_.Clone();
            errorMessage_ = other.errorMessage_;
        }

        public static pb::MessageParser<Response> Parser
        {
            get { return _parser; }
        }

        public static pbr::MessageDescriptor Descriptor
        {
            get { return global::PokemonGo.RocketAPI.GeneratedCode.ResponseReflection.Descriptor.MessageTypes[0]; }
        }

        public int Unknown1
        {
            get { return unknown1_; }
            set { unknown1_ = value; }
        }

        public long Unknown2
        {
            get { return unknown2_; }
            set { unknown2_ = value; }
        }

        public string ApiUrl
        {
            get { return apiUrl_; }
            set { apiUrl_ = pb::ProtoPreconditions.CheckNotNull(value, "value"); }
        }

        public global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Unknown6 Unknown6
        {
            get { return unknown6_; }
            set { unknown6_ = value; }
        }

        public global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Auth Auth
        {
            get { return auth_; }
            set { auth_ = value; }
        }

        public pbc::RepeatedField<pb::ByteString> Payload
        {
            get { return payload_; }
        }

        public string ErrorMessage
        {
            get { return errorMessage_; }
            set { errorMessage_ = pb::ProtoPreconditions.CheckNotNull(value, "value"); }
        }

        pbr::MessageDescriptor pb::IMessage.Descriptor
        {
            get { return Descriptor; }
        }

        public Response Clone()
        {
            return new Response(this);
        }

        public bool Equals(Response other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            if (Unknown1 != other.Unknown1) return false;
            if (Unknown2 != other.Unknown2) return false;
            if (ApiUrl != other.ApiUrl) return false;
            if (!Equals(Unknown6, other.Unknown6)) return false;
            if (!Equals(Auth, other.Auth)) return false;
            if (!payload_.Equals(other.payload_)) return false;
            if (ErrorMessage != other.ErrorMessage) return false;
            return true;
        }

        public void WriteTo(pb::CodedOutputStream output)
        {
            if (Unknown1 != 0)
            {
                output.WriteRawTag(8);
                output.WriteInt32(Unknown1);
            }
            if (Unknown2 != 0L)
            {
                output.WriteRawTag(16);
                output.WriteInt64(Unknown2);
            }
            if (ApiUrl.Length != 0)
            {
                output.WriteRawTag(26);
                output.WriteString(ApiUrl);
            }
            if (unknown6_ != null)
            {
                output.WriteRawTag(50);
                output.WriteMessage(Unknown6);
            }
            if (auth_ != null)
            {
                output.WriteRawTag(58);
                output.WriteMessage(Auth);
            }
            payload_.WriteTo(output, _repeated_payload_codec);
            if (ErrorMessage.Length != 0)
            {
                output.WriteRawTag(170, 6);
                output.WriteString(ErrorMessage);
            }
        }

        public int CalculateSize()
        {
            var size = 0;
            if (Unknown1 != 0)
            {
                size += 1 + pb::CodedOutputStream.ComputeInt32Size(Unknown1);
            }
            if (Unknown2 != 0L)
            {
                size += 1 + pb::CodedOutputStream.ComputeInt64Size(Unknown2);
            }
            if (ApiUrl.Length != 0)
            {
                size += 1 + pb::CodedOutputStream.ComputeStringSize(ApiUrl);
            }
            if (unknown6_ != null)
            {
                size += 1 + pb::CodedOutputStream.ComputeMessageSize(Unknown6);
            }
            if (auth_ != null)
            {
                size += 1 + pb::CodedOutputStream.ComputeMessageSize(Auth);
            }
            size += payload_.CalculateSize(_repeated_payload_codec);
            if (ErrorMessage.Length != 0)
            {
                size += 2 + pb::CodedOutputStream.ComputeStringSize(ErrorMessage);
            }
            return size;
        }

        public void MergeFrom(Response other)
        {
            if (other == null)
            {
                return;
            }
            if (other.Unknown1 != 0)
            {
                Unknown1 = other.Unknown1;
            }
            if (other.Unknown2 != 0L)
            {
                Unknown2 = other.Unknown2;
            }
            if (other.ApiUrl.Length != 0)
            {
                ApiUrl = other.ApiUrl;
            }
            if (other.unknown6_ != null)
            {
                if (unknown6_ == null)
                {
                    unknown6_ = new global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Unknown6();
                }
                Unknown6.MergeFrom(other.Unknown6);
            }
            if (other.auth_ != null)
            {
                if (auth_ == null)
                {
                    auth_ = new global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Auth();
                }
                Auth.MergeFrom(other.Auth);
            }
            payload_.Add(other.payload_);
            if (other.ErrorMessage.Length != 0)
            {
                ErrorMessage = other.ErrorMessage;
            }
        }

        public void MergeFrom(pb::CodedInputStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch (tag)
                {
                    default:
                        input.SkipLastField();
                        break;
                    case 8:
                        {
                            Unknown1 = input.ReadInt32();
                            break;
                        }
                    case 16:
                        {
                            Unknown2 = input.ReadInt64();
                            break;
                        }
                    case 26:
                        {
                            ApiUrl = input.ReadString();
                            break;
                        }
                    case 50:
                        {
                            if (unknown6_ == null)
                            {
                                unknown6_ = new global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Unknown6();
                            }
                            input.ReadMessage(unknown6_);
                            break;
                        }
                    case 58:
                        {
                            if (auth_ == null)
                            {
                                auth_ = new global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Auth();
                            }
                            input.ReadMessage(auth_);
                            break;
                        }
                    case 802:
                        {
                            payload_.AddEntriesFrom(input, _repeated_payload_codec);
                            break;
                        }
                    case 810:
                        {
                            ErrorMessage = input.ReadString();
                            break;
                        }
                }
            }
        }

        public override bool Equals(object other)
        {
            return Equals(other as Response);
        }

        public override int GetHashCode()
        {
            var hash = 1;
            if (Unknown1 != 0) hash ^= Unknown1.GetHashCode();
            if (Unknown2 != 0L) hash ^= Unknown2.GetHashCode();
            if (ApiUrl.Length != 0) hash ^= ApiUrl.GetHashCode();
            if (unknown6_ != null) hash ^= Unknown6.GetHashCode();
            if (auth_ != null) hash ^= Auth.GetHashCode();
            hash ^= payload_.GetHashCode();
            if (ErrorMessage.Length != 0) hash ^= ErrorMessage.GetHashCode();
            return hash;
        }

        partial void OnConstruction();

        public override string ToString()
        {
            return pb::JsonFormatter.ToDiagnosticString(this);
        }

        #region Nested types

        /// <summary>Container for nested types declared in the Response message type.</summary>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static partial class Types
        {
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public sealed partial class Unknown6 : pb::IMessage<Unknown6>
            {
                /// <summary>Field number for the "unknown1" field.</summary>
                public const int Unknown1FieldNumber = 1;

                /// <summary>Field number for the "unknown2" field.</summary>
                public const int Unknown2FieldNumber = 2;

                private static readonly pb::MessageParser<Unknown6> _parser =
                    new pb::MessageParser<Unknown6>(() => new Unknown6());

                private int unknown1_;
                private global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Unknown6.Types.Unknown2 unknown2_;

                public Unknown6()
                {
                    OnConstruction();
                }

                public Unknown6(Unknown6 other) : this()
                {
                    unknown1_ = other.unknown1_;
                    Unknown2 = other.unknown2_ != null ? other.Unknown2.Clone() : null;
                }

                public static pb::MessageParser<Unknown6> Parser
                {
                    get { return _parser; }
                }

                public static pbr::MessageDescriptor Descriptor
                {
                    get { return global::PokemonGo.RocketAPI.GeneratedCode.Response.Descriptor.NestedTypes[0]; }
                }

                public int Unknown1
                {
                    get { return unknown1_; }
                    set { unknown1_ = value; }
                }

                public global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Unknown6.Types.Unknown2 Unknown2
                {
                    get { return unknown2_; }
                    set { unknown2_ = value; }
                }

                pbr::MessageDescriptor pb::IMessage.Descriptor
                {
                    get { return Descriptor; }
                }

                public Unknown6 Clone()
                {
                    return new Unknown6(this);
                }

                public bool Equals(Unknown6 other)
                {
                    if (ReferenceEquals(other, null))
                    {
                        return false;
                    }
                    if (ReferenceEquals(other, this))
                    {
                        return true;
                    }
                    if (Unknown1 != other.Unknown1) return false;
                    if (!Equals(Unknown2, other.Unknown2)) return false;
                    return true;
                }

                public void WriteTo(pb::CodedOutputStream output)
                {
                    if (Unknown1 != 0)
                    {
                        output.WriteRawTag(8);
                        output.WriteInt32(Unknown1);
                    }
                    if (unknown2_ != null)
                    {
                        output.WriteRawTag(18);
                        output.WriteMessage(Unknown2);
                    }
                }

                public int CalculateSize()
                {
                    var size = 0;
                    if (Unknown1 != 0)
                    {
                        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Unknown1);
                    }
                    if (unknown2_ != null)
                    {
                        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Unknown2);
                    }
                    return size;
                }

                public void MergeFrom(Unknown6 other)
                {
                    if (other == null)
                    {
                        return;
                    }
                    if (other.Unknown1 != 0)
                    {
                        Unknown1 = other.Unknown1;
                    }
                    if (other.unknown2_ != null)
                    {
                        if (unknown2_ == null)
                        {
                            unknown2_ =
                                new global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Unknown6.Types.Unknown2();
                        }
                        Unknown2.MergeFrom(other.Unknown2);
                    }
                }

                public void MergeFrom(pb::CodedInputStream input)
                {
                    uint tag;
                    while ((tag = input.ReadTag()) != 0)
                    {
                        switch (tag)
                        {
                            default:
                                input.SkipLastField();
                                break;
                            case 8:
                                {
                                    Unknown1 = input.ReadInt32();
                                    break;
                                }
                            case 18:
                                {
                                    if (unknown2_ == null)
                                    {
                                        unknown2_ =
                                            new global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Unknown6.Types.
                                                Unknown2();
                                    }
                                    input.ReadMessage(unknown2_);
                                    break;
                                }
                        }
                    }
                }

                public override bool Equals(object other)
                {
                    return Equals(other as Unknown6);
                }

                public override int GetHashCode()
                {
                    var hash = 1;
                    if (Unknown1 != 0) hash ^= Unknown1.GetHashCode();
                    if (unknown2_ != null) hash ^= Unknown2.GetHashCode();
                    return hash;
                }

                partial void OnConstruction();

                public override string ToString()
                {
                    return pb::JsonFormatter.ToDiagnosticString(this);
                }

                #region Nested types

                /// <summary>Container for nested types declared in the Unknown6 message type.</summary>
                [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
                public static partial class Types
                {
                    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
                    public sealed partial class Unknown2 : pb::IMessage<Unknown2>
                    {
                        /// <summary>Field number for the "unknown1" field.</summary>
                        public const int Unknown1FieldNumber = 1;

                        private static readonly pb::MessageParser<Unknown2> _parser =
                            new pb::MessageParser<Unknown2>(() => new Unknown2());

                        private pb::ByteString unknown1_ = pb::ByteString.Empty;

                        public Unknown2()
                        {
                            OnConstruction();
                        }

                        public Unknown2(Unknown2 other) : this()
                        {
                            unknown1_ = other.unknown1_;
                        }

                        public static pb::MessageParser<Unknown2> Parser
                        {
                            get { return _parser; }
                        }

                        public static pbr::MessageDescriptor Descriptor
                        {
                            get
                            {
                                return
                                    global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.Unknown6.Descriptor
                                        .NestedTypes[0];
                            }
                        }

                        public pb::ByteString Unknown1
                        {
                            get { return unknown1_; }
                            set { unknown1_ = pb::ProtoPreconditions.CheckNotNull(value, "value"); }
                        }

                        pbr::MessageDescriptor pb::IMessage.Descriptor
                        {
                            get { return Descriptor; }
                        }

                        public Unknown2 Clone()
                        {
                            return new Unknown2(this);
                        }

                        public bool Equals(Unknown2 other)
                        {
                            if (ReferenceEquals(other, null))
                            {
                                return false;
                            }
                            if (ReferenceEquals(other, this))
                            {
                                return true;
                            }
                            if (Unknown1 != other.Unknown1) return false;
                            return true;
                        }

                        public void WriteTo(pb::CodedOutputStream output)
                        {
                            if (Unknown1.Length != 0)
                            {
                                output.WriteRawTag(10);
                                output.WriteBytes(Unknown1);
                            }
                        }

                        public int CalculateSize()
                        {
                            var size = 0;
                            if (Unknown1.Length != 0)
                            {
                                size += 1 + pb::CodedOutputStream.ComputeBytesSize(Unknown1);
                            }
                            return size;
                        }

                        public void MergeFrom(Unknown2 other)
                        {
                            if (other == null)
                            {
                                return;
                            }
                            if (other.Unknown1.Length != 0)
                            {
                                Unknown1 = other.Unknown1;
                            }
                        }

                        public void MergeFrom(pb::CodedInputStream input)
                        {
                            uint tag;
                            while ((tag = input.ReadTag()) != 0)
                            {
                                switch (tag)
                                {
                                    default:
                                        input.SkipLastField();
                                        break;
                                    case 10:
                                        {
                                            Unknown1 = input.ReadBytes();
                                            break;
                                        }
                                }
                            }
                        }

                        public override bool Equals(object other)
                        {
                            return Equals(other as Unknown2);
                        }

                        public override int GetHashCode()
                        {
                            var hash = 1;
                            if (Unknown1.Length != 0) hash ^= Unknown1.GetHashCode();
                            return hash;
                        }

                        partial void OnConstruction();

                        public override string ToString()
                        {
                            return pb::JsonFormatter.ToDiagnosticString(this);
                        }
                    }
                }

                #endregion
            }

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public sealed partial class Auth : pb::IMessage<Auth>
            {
                /// <summary>Field number for the "unknown71" field.</summary>
                public const int Unknown71FieldNumber = 1;

                /// <summary>Field number for the "timestamp" field.</summary>
                public const int TimestampFieldNumber = 2;

                /// <summary>Field number for the "unknown73" field.</summary>
                public const int Unknown73FieldNumber = 3;

                private static readonly pb::MessageParser<Auth> _parser = new pb::MessageParser<Auth>(() => new Auth());
                private long timestamp_;
                private pb::ByteString unknown71_ = pb::ByteString.Empty;
                private pb::ByteString unknown73_ = pb::ByteString.Empty;

                public Auth()
                {
                    OnConstruction();
                }

                public Auth(Auth other) : this()
                {
                    unknown71_ = other.unknown71_;
                    timestamp_ = other.timestamp_;
                    unknown73_ = other.unknown73_;
                }

                public static pb::MessageParser<Auth> Parser
                {
                    get { return _parser; }
                }

                public static pbr::MessageDescriptor Descriptor
                {
                    get { return global::PokemonGo.RocketAPI.GeneratedCode.Response.Descriptor.NestedTypes[1]; }
                }

                public pb::ByteString Unknown71
                {
                    get { return unknown71_; }
                    set { unknown71_ = pb::ProtoPreconditions.CheckNotNull(value, "value"); }
                }

                public long Timestamp
                {
                    get { return timestamp_; }
                    set { timestamp_ = value; }
                }

                public pb::ByteString Unknown73
                {
                    get { return unknown73_; }
                    set { unknown73_ = pb::ProtoPreconditions.CheckNotNull(value, "value"); }
                }

                pbr::MessageDescriptor pb::IMessage.Descriptor
                {
                    get { return Descriptor; }
                }

                public Auth Clone()
                {
                    return new Auth(this);
                }

                public bool Equals(Auth other)
                {
                    if (ReferenceEquals(other, null))
                    {
                        return false;
                    }
                    if (ReferenceEquals(other, this))
                    {
                        return true;
                    }
                    if (Unknown71 != other.Unknown71) return false;
                    if (Timestamp != other.Timestamp) return false;
                    if (Unknown73 != other.Unknown73) return false;
                    return true;
                }

                public void WriteTo(pb::CodedOutputStream output)
                {
                    if (Unknown71.Length != 0)
                    {
                        output.WriteRawTag(10);
                        output.WriteBytes(Unknown71);
                    }
                    if (Timestamp != 0L)
                    {
                        output.WriteRawTag(16);
                        output.WriteInt64(Timestamp);
                    }
                    if (Unknown73.Length != 0)
                    {
                        output.WriteRawTag(26);
                        output.WriteBytes(Unknown73);
                    }
                }

                public int CalculateSize()
                {
                    var size = 0;
                    if (Unknown71.Length != 0)
                    {
                        size += 1 + pb::CodedOutputStream.ComputeBytesSize(Unknown71);
                    }
                    if (Timestamp != 0L)
                    {
                        size += 1 + pb::CodedOutputStream.ComputeInt64Size(Timestamp);
                    }
                    if (Unknown73.Length != 0)
                    {
                        size += 1 + pb::CodedOutputStream.ComputeBytesSize(Unknown73);
                    }
                    return size;
                }

                public void MergeFrom(Auth other)
                {
                    if (other == null)
                    {
                        return;
                    }
                    if (other.Unknown71.Length != 0)
                    {
                        Unknown71 = other.Unknown71;
                    }
                    if (other.Timestamp != 0L)
                    {
                        Timestamp = other.Timestamp;
                    }
                    if (other.Unknown73.Length != 0)
                    {
                        Unknown73 = other.Unknown73;
                    }
                }

                public void MergeFrom(pb::CodedInputStream input)
                {
                    uint tag;
                    while ((tag = input.ReadTag()) != 0)
                    {
                        switch (tag)
                        {
                            default:
                                input.SkipLastField();
                                break;
                            case 10:
                                {
                                    Unknown71 = input.ReadBytes();
                                    break;
                                }
                            case 16:
                                {
                                    Timestamp = input.ReadInt64();
                                    break;
                                }
                            case 26:
                                {
                                    Unknown73 = input.ReadBytes();
                                    break;
                                }
                        }
                    }
                }

                public override bool Equals(object other)
                {
                    return Equals(other as Auth);
                }

                public override int GetHashCode()
                {
                    var hash = 1;
                    if (Unknown71.Length != 0) hash ^= Unknown71.GetHashCode();
                    if (Timestamp != 0L) hash ^= Timestamp.GetHashCode();
                    if (Unknown73.Length != 0) hash ^= Unknown73.GetHashCode();
                    return hash;
                }

                partial void OnConstruction();

                public override string ToString()
                {
                    return pb::JsonFormatter.ToDiagnosticString(this);
                }
            }

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public sealed partial class RecycleInventoryItemResponse : pb::IMessage<RecycleInventoryItemResponse>
            {
                /// <summary>Field number for the "result" field.</summary>
                public const int ResultFieldNumber = 1;

                /// <summary>Field number for the "new_count" field.</summary>
                public const int NewCountFieldNumber = 2;

                private static readonly pb::MessageParser<RecycleInventoryItemResponse> _parser =
                    new pb::MessageParser<RecycleInventoryItemResponse>(() => new RecycleInventoryItemResponse());

                private int newCount_;

                private
                    global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.RecycleInventoryItemResponse.Types.Result
                    result_ = 0;

                public RecycleInventoryItemResponse()
                {
                    OnConstruction();
                }

                public RecycleInventoryItemResponse(RecycleInventoryItemResponse other) : this()
                {
                    result_ = other.result_;
                    newCount_ = other.newCount_;
                }

                public static pb::MessageParser<RecycleInventoryItemResponse> Parser
                {
                    get { return _parser; }
                }

                public static pbr::MessageDescriptor Descriptor
                {
                    get { return global::PokemonGo.RocketAPI.GeneratedCode.Response.Descriptor.NestedTypes[2]; }
                }

                public
                    global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.RecycleInventoryItemResponse.Types.Result
                    Result
                {
                    get { return result_; }
                    set { result_ = value; }
                }

                public int NewCount
                {
                    get { return newCount_; }
                    set { newCount_ = value; }
                }

                pbr::MessageDescriptor pb::IMessage.Descriptor
                {
                    get { return Descriptor; }
                }

                public RecycleInventoryItemResponse Clone()
                {
                    return new RecycleInventoryItemResponse(this);
                }

                public bool Equals(RecycleInventoryItemResponse other)
                {
                    if (ReferenceEquals(other, null))
                    {
                        return false;
                    }
                    if (ReferenceEquals(other, this))
                    {
                        return true;
                    }
                    if (Result != other.Result) return false;
                    if (NewCount != other.NewCount) return false;
                    return true;
                }

                public void WriteTo(pb::CodedOutputStream output)
                {
                    if (Result != 0)
                    {
                        output.WriteRawTag(8);
                        output.WriteEnum((int)Result);
                    }
                    if (NewCount != 0)
                    {
                        output.WriteRawTag(16);
                        output.WriteInt32(NewCount);
                    }
                }

                public int CalculateSize()
                {
                    var size = 0;
                    if (Result != 0)
                    {
                        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int)Result);
                    }
                    if (NewCount != 0)
                    {
                        size += 1 + pb::CodedOutputStream.ComputeInt32Size(NewCount);
                    }
                    return size;
                }

                public void MergeFrom(RecycleInventoryItemResponse other)
                {
                    if (other == null)
                    {
                        return;
                    }
                    if (other.Result != 0)
                    {
                        Result = other.Result;
                    }
                    if (other.NewCount != 0)
                    {
                        NewCount = other.NewCount;
                    }
                }

                public void MergeFrom(pb::CodedInputStream input)
                {
                    uint tag;
                    while ((tag = input.ReadTag()) != 0)
                    {
                        switch (tag)
                        {
                            default:
                                input.SkipLastField();
                                break;
                            case 8:
                                {
                                    result_ =
                                        (
                                            global::PokemonGo.RocketAPI.GeneratedCode.Response.Types.
                                                RecycleInventoryItemResponse.Types.Result)input.ReadEnum();
                                    break;
                                }
                            case 16:
                                {
                                    NewCount = input.ReadInt32();
                                    break;
                                }
                        }
                    }
                }

                public override bool Equals(object other)
                {
                    return Equals(other as RecycleInventoryItemResponse);
                }

                public override int GetHashCode()
                {
                    var hash = 1;
                    if (Result != 0) hash ^= Result.GetHashCode();
                    if (NewCount != 0) hash ^= NewCount.GetHashCode();
                    return hash;
                }

                partial void OnConstruction();

                public override string ToString()
                {
                    return pb::JsonFormatter.ToDiagnosticString(this);
                }

                #region Nested types

                /// <summary>Container for nested types declared in the RecycleInventoryItemResponse message type.</summary>
                [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
                public static partial class Types
                {
                    public enum Result
                    {
                        [pbr::OriginalName("UNSET")]
                        Unset = 0,
                        [pbr::OriginalName("SUCCESS")]
                        Success = 1,
                        [pbr::OriginalName("ERROR_NOT_ENOUGH_COPIES")]
                        ErrorNotEnoughCopies = 2,
                        [pbr::OriginalName("ERROR_CANNOT_RECYCLE_INCUBATORS")]
                        ErrorCannotRecycleIncubators = 3,
                    }
                }

                #endregion
            }
        }

        #endregion
    }

    #endregion
}

#endregion Designer generated code