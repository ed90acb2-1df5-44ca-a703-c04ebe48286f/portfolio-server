// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: errors.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Portfolio.Protocol.Errors {

  /// <summary>Holder for reflection information generated from errors.proto</summary>
  public static partial class ErrorsReflection {

    #region Descriptor
    /// <summary>File descriptor for errors.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ErrorsReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxlcnJvcnMucHJvdG8iLwoMRXJyb3JNZXNzYWdlEh8KBENvZGUYASABKA4y",
            "ES5FcnJvck1lc3NhZ2VDb2RlKmQKEEVycm9yTWVzc2FnZUNvZGUSDQoJVW5k",
            "ZWZpbmVkEAASGwoXUmVnaXN0cmF0aW9uTG9naW5FeGlzdHMQARIkCiBBdXRo",
            "ZW50aWNhdGlvbkludmFsaWRDcmVkZW50aWFscxACQhyqAhlQb3J0Zm9saW8u",
            "UHJvdG9jb2wuRXJyb3JzYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Portfolio.Protocol.Errors.ErrorMessageCode), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Portfolio.Protocol.Errors.ErrorMessage), global::Portfolio.Protocol.Errors.ErrorMessage.Parser, new[]{ "Code" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum ErrorMessageCode {
    [pbr::OriginalName("Undefined")] Undefined = 0,
    [pbr::OriginalName("RegistrationLoginExists")] RegistrationLoginExists = 1,
    [pbr::OriginalName("AuthenticationInvalidCredentials")] AuthenticationInvalidCredentials = 2,
  }

  #endregion

  #region Messages
  public sealed partial class ErrorMessage : pb::IMessage<ErrorMessage>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ErrorMessage> _parser = new pb::MessageParser<ErrorMessage>(() => new ErrorMessage());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ErrorMessage> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Portfolio.Protocol.Errors.ErrorsReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ErrorMessage() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ErrorMessage(ErrorMessage other) : this() {
      code_ = other.code_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ErrorMessage Clone() {
      return new ErrorMessage(this);
    }

    /// <summary>Field number for the "Code" field.</summary>
    public const int CodeFieldNumber = 1;
    private global::Portfolio.Protocol.Errors.ErrorMessageCode code_ = global::Portfolio.Protocol.Errors.ErrorMessageCode.Undefined;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Portfolio.Protocol.Errors.ErrorMessageCode Code {
      get { return code_; }
      set {
        code_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as ErrorMessage);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ErrorMessage other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Code != other.Code) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (Code != global::Portfolio.Protocol.Errors.ErrorMessageCode.Undefined) hash ^= Code.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (Code != global::Portfolio.Protocol.Errors.ErrorMessageCode.Undefined) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Code);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (Code != global::Portfolio.Protocol.Errors.ErrorMessageCode.Undefined) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Code);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (Code != global::Portfolio.Protocol.Errors.ErrorMessageCode.Undefined) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Code);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(ErrorMessage other) {
      if (other == null) {
        return;
      }
      if (other.Code != global::Portfolio.Protocol.Errors.ErrorMessageCode.Undefined) {
        Code = other.Code;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Code = (global::Portfolio.Protocol.Errors.ErrorMessageCode) input.ReadEnum();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            Code = (global::Portfolio.Protocol.Errors.ErrorMessageCode) input.ReadEnum();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code