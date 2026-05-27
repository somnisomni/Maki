using System.Text;

namespace Somni.Maki.Core.Metadata.Steganography.Versions {
  public enum SteganoMetadataV1Keys {
    PixelDataHashLength,
    PixelDataHash,
    PixelDataSignatureLength,
    PixelDataSignature,
    DataPayloadLength,
    DataPayload,
  }
  
  public sealed class SteganoMetadataV1 : MakiMetadataBase<SteganoMetadataV1Keys> {
    public override ushort MetadataVersion => 1;

    protected override Dictionary<SteganoMetadataV1Keys, byte[]> BytesProperties { get; } = new() {
      { SteganoMetadataV1Keys.PixelDataHash, [ ] },
      { SteganoMetadataV1Keys.PixelDataSignature, [ ] },
      { SteganoMetadataV1Keys.DataPayload, [ ] },
    };

    protected override Dictionary<SteganoMetadataV1Keys, int> Int32Properties { get; } = new() {
      { SteganoMetadataV1Keys.PixelDataHashLength, 0 },
      { SteganoMetadataV1Keys.PixelDataSignatureLength, 0 },
      { SteganoMetadataV1Keys.DataPayloadLength, 0 },
    };
    
    public SteganoMetadataV1() { }
    public SteganoMetadataV1(string pixelDataHash, ReadOnlySpan<byte> pixelDataSignature, string? dataPayload) {
      if(!string.IsNullOrEmpty(pixelDataHash)) {
        BytesProperties[SteganoMetadataV1Keys.PixelDataHash] = Encoding.ASCII.GetBytes(pixelDataHash);
        Int32Properties[SteganoMetadataV1Keys.PixelDataHashLength] = Encoding.ASCII.GetByteCount(pixelDataHash);
      }

      if(!pixelDataSignature.IsEmpty || pixelDataSignature.Length > 0) {
        BytesProperties[SteganoMetadataV1Keys.PixelDataSignature] = pixelDataSignature.ToArray();
        Int32Properties[SteganoMetadataV1Keys.PixelDataSignatureLength] = pixelDataSignature.Length;
      }
      
      if(!string.IsNullOrEmpty(dataPayload)) {
        BytesProperties[SteganoMetadataV1Keys.DataPayload] = Encoding.UTF8.GetBytes(dataPayload);
        Int32Properties[SteganoMetadataV1Keys.DataPayloadLength] = Encoding.UTF8.GetByteCount(dataPayload);
      }
    }

    public override void InitializeFromBytes(ReadOnlySpan<byte> bytes) {
      using MemoryStream stream = new(bytes.ToArray());
      using BinaryReader reader = new(stream);
      
      int hashLength = Int32Properties[SteganoMetadataV1Keys.PixelDataHashLength] = reader.ReadInt32();            // 4 bytes
      BytesProperties[SteganoMetadataV1Keys.PixelDataHash] = reader.ReadGuardedBytes(hashLength);                  // 1 byte + Variable + 1 byte
      int signatureLength = Int32Properties[SteganoMetadataV1Keys.PixelDataSignatureLength] = reader.ReadInt32();  // 4 bytes
      BytesProperties[SteganoMetadataV1Keys.PixelDataSignature] = reader.ReadGuardedBytes(signatureLength);        // 1 byte + Variable + 1 byte
      int payloadLength = Int32Properties[SteganoMetadataV1Keys.DataPayloadLength] = reader.ReadInt32();           // 4 bytes
      BytesProperties[SteganoMetadataV1Keys.DataPayload] = reader.ReadGuardedBytes(payloadLength);                 // 1 byte + Variable + 1 byte
      
      UpdateBaseDictionaries();
    }

    public override ReadOnlySpan<byte> ToBytes() {
      using MemoryStream stream = new();
      using BinaryWriter writer = new(stream);
      
      writer.Write(Int32Properties[SteganoMetadataV1Keys.PixelDataHashLength]);       // 4 bytes
      writer.WriteGuardedBytes(BytesProperties[SteganoMetadataV1Keys.PixelDataHash],
        Int32Properties[SteganoMetadataV1Keys.PixelDataHashLength]);                  // 1 byte + Variable + 1 byte
      writer.Write(Int32Properties[SteganoMetadataV1Keys.PixelDataSignatureLength]);  // 4 bytes
      writer.WriteGuardedBytes(BytesProperties[SteganoMetadataV1Keys.PixelDataSignature],
        Int32Properties[SteganoMetadataV1Keys.PixelDataSignatureLength]);             // 1 byte + Variable + 1 byte
      writer.Write(Int32Properties[SteganoMetadataV1Keys.DataPayloadLength]);         // 4 bytes
      writer.WriteGuardedBytes(BytesProperties[SteganoMetadataV1Keys.DataPayload],
        Int32Properties[SteganoMetadataV1Keys.DataPayloadLength]);                    // 1 byte + Variable + 1 byte

      return stream.ToArray();
    }
  }
}
