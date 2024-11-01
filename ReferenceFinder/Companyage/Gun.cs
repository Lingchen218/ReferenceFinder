// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace Companyage
{

using global::System;
using global::System.Collections.Generic;
using global::Google.FlatBuffers;

public struct Gun : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_24_3_25(); }
  public static Gun GetRootAsGun(ByteBuffer _bb) { return GetRootAsGun(_bb, new Gun()); }
  public static Gun GetRootAsGun(ByteBuffer _bb, Gun obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public static bool GunBufferHasIdentifier(ByteBuffer _bb) { return Table.__has_identifier(_bb, "WHAT"); }
  public static bool VerifyGun(ByteBuffer _bb) {Google.FlatBuffers.Verifier verifier = new Google.FlatBuffers.Verifier(_bb); return verifier.VerifyBuffer("WHAT", false, GunVerify.Verify); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public Gun __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string SerializedGuid(int j) { int o = __p.__offset(4); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : null; }
  public int SerializedGuidLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }
  public string SerializedDependencyHash(int j) { int o = __p.__offset(6); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : null; }
  public int SerializedDependencyHashLength { get { int o = __p.__offset(6); return o != 0 ? __p.__vector_len(o) : 0; } }
  public Companyage.IntArray? SerializedDenpendencies(int j) { int o = __p.__offset(8); return o != 0 ? (Companyage.IntArray?)(new Companyage.IntArray()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int SerializedDenpendenciesLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<Companyage.Gun> CreateGun(FlatBufferBuilder builder,
      VectorOffset serializedGuidOffset = default(VectorOffset),
      VectorOffset serializedDependencyHashOffset = default(VectorOffset),
      VectorOffset serializedDenpendenciesOffset = default(VectorOffset)) {
    builder.StartTable(3);
    Gun.AddSerializedDenpendencies(builder, serializedDenpendenciesOffset);
    Gun.AddSerializedDependencyHash(builder, serializedDependencyHashOffset);
    Gun.AddSerializedGuid(builder, serializedGuidOffset);
    return Gun.EndGun(builder);
  }

  public static void StartGun(FlatBufferBuilder builder) { builder.StartTable(3); }
  public static void AddSerializedGuid(FlatBufferBuilder builder, VectorOffset serializedGuidOffset) { builder.AddOffset(0, serializedGuidOffset.Value, 0); }
  public static VectorOffset CreateSerializedGuidVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateSerializedGuidVectorBlock(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateSerializedGuidVectorBlock(FlatBufferBuilder builder, ArraySegment<StringOffset> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateSerializedGuidVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<StringOffset>(dataPtr, sizeInBytes); return builder.EndVector(); }
  public static void StartSerializedGuidVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSerializedDependencyHash(FlatBufferBuilder builder, VectorOffset serializedDependencyHashOffset) { builder.AddOffset(1, serializedDependencyHashOffset.Value, 0); }
  public static VectorOffset CreateSerializedDependencyHashVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateSerializedDependencyHashVectorBlock(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateSerializedDependencyHashVectorBlock(FlatBufferBuilder builder, ArraySegment<StringOffset> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateSerializedDependencyHashVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<StringOffset>(dataPtr, sizeInBytes); return builder.EndVector(); }
  public static void StartSerializedDependencyHashVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSerializedDenpendencies(FlatBufferBuilder builder, VectorOffset serializedDenpendenciesOffset) { builder.AddOffset(2, serializedDenpendenciesOffset.Value, 0); }
  public static VectorOffset CreateSerializedDenpendenciesVector(FlatBufferBuilder builder, Offset<Companyage.IntArray>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateSerializedDenpendenciesVectorBlock(FlatBufferBuilder builder, Offset<Companyage.IntArray>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateSerializedDenpendenciesVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<Companyage.IntArray>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateSerializedDenpendenciesVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<Companyage.IntArray>>(dataPtr, sizeInBytes); return builder.EndVector(); }
  public static void StartSerializedDenpendenciesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Companyage.Gun> EndGun(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<Companyage.Gun>(o);
  }
  public static void FinishGunBuffer(FlatBufferBuilder builder, Offset<Companyage.Gun> offset) { builder.Finish(offset.Value, "WHAT"); }
  public static void FinishSizePrefixedGunBuffer(FlatBufferBuilder builder, Offset<Companyage.Gun> offset) { builder.FinishSizePrefixed(offset.Value, "WHAT"); }
}


static public class GunVerify
{
  static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyVectorOfStrings(tablePos, 4 /*SerializedGuid*/, false)
      && verifier.VerifyVectorOfStrings(tablePos, 6 /*SerializedDependencyHash*/, false)
      && verifier.VerifyVectorOfTables(tablePos, 8 /*SerializedDenpendencies*/, Companyage.IntArrayVerify.Verify, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}

}