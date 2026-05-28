using System.Text;

namespace Somni.Maki.Core.Metadata.Steganography.Versions {
  public enum SteganoMetadataV1Keys {
    /* int */    PixelDataHashLength,
    /* byte[] */ PixelDataHash,
    /* int */    PixelDataSignatureLength,
    /* byte[] */ PixelDataSignature,
    /* int */    DataPayloadLength,
    /* byte[] */ DataPayload,
  }
  
  public sealed class SteganoMetadataV1 : MakiMetadataBase<SteganoMetadataV1Keys> {
    public override ushort MetadataVersion => 1;

    protected override Dictionary<SteganoMetadataV1Keys, object> Properties { get; } = new() {
      { SteganoMetadataV1Keys.PixelDataHashLength, 0 },
      { SteganoMetadataV1Keys.PixelDataHash, Array.Empty<byte>() },
      { SteganoMetadataV1Keys.PixelDataSignatureLength, 0 },
      { SteganoMetadataV1Keys.PixelDataSignature, Array.Empty<byte>() },
      { SteganoMetadataV1Keys.DataPayloadLength, 0 },
      { SteganoMetadataV1Keys.DataPayload, Array.Empty<byte>() },
    };
    
    public SteganoMetadataV1() { }
    public SteganoMetadataV1(string pixelDataHash, ReadOnlySpan<byte> pixelDataSignature, string? dataPayload) {
      if(!string.IsNullOrEmpty(pixelDataHash)) {
        Properties[SteganoMetadataV1Keys.PixelDataHash] = Encoding.ASCII.GetBytes(pixelDataHash);
        Properties[SteganoMetadataV1Keys.PixelDataHashLength] = Encoding.ASCII.GetByteCount(pixelDataHash);
      }

      if(!pixelDataSignature.IsEmpty || pixelDataSignature.Length > 0) {
        Properties[SteganoMetadataV1Keys.PixelDataSignature] = pixelDataSignature.ToArray();
        Properties[SteganoMetadataV1Keys.PixelDataSignatureLength] = pixelDataSignature.Length;
      }
      
      if(!string.IsNullOrEmpty(dataPayload)) {
        Properties[SteganoMetadataV1Keys.DataPayload] = Encoding.UTF8.GetBytes(dataPayload);
        Properties[SteganoMetadataV1Keys.DataPayloadLength] = Encoding.UTF8.GetByteCount(dataPayload);
      }
    }

    public override void InitializeFromBytes(ReadOnlySpan<byte> bytes) {
      using MemoryStream stream = new(bytes.ToArray());
      using BinaryReader reader = new(stream);
      
      int hashLength = (int)(Properties[SteganoMetadataV1Keys.PixelDataHashLength] = reader.ReadInt32());   // 4 bytes
      Properties[SteganoMetadataV1Keys.PixelDataHash] = reader.ReadGuardedBytes(hashLength);                // 1 byte + Variable + 1 byte
      int signatureLength = (int)(Properties[SteganoMetadataV1Keys.PixelDataSignatureLength] = reader.ReadInt32());  // 4 bytes
      Properties[SteganoMetadataV1Keys.PixelDataSignature] = reader.ReadGuardedBytes(signatureLength);      // 1 byte + Variable + 1 byte
      int payloadLength = (int)(Properties[SteganoMetadataV1Keys.DataPayloadLength] = reader.ReadInt32());  // 4 bytes
      Properties[SteganoMetadataV1Keys.DataPayload] = reader.ReadGuardedBytes(payloadLength);               // 1 byte + Variable + 1 byte
      
      UpdateBaseDictionaries();
    }

    public override ReadOnlySpan<byte> ToBytes() {
      using MemoryStream stream = new();
      using BinaryWriter writer = new(stream);
      
      writer.Write(Get<int>(SteganoMetadataV1Keys.PixelDataHashLength));       // 4 bytes
      writer.WriteGuardedBytes(Get<byte[]>(SteganoMetadataV1Keys.PixelDataHash),
        Get<int>(SteganoMetadataV1Keys.PixelDataHashLength));                  // 1 byte + Variable + 1 byte
      writer.Write(Get<int>(SteganoMetadataV1Keys.PixelDataSignatureLength));  // 4 bytes
      writer.WriteGuardedBytes(Get<byte[]>(SteganoMetadataV1Keys.PixelDataSignature),
        Get<int>(SteganoMetadataV1Keys.PixelDataSignatureLength));             // 1 byte + Variable + 1 byte
      writer.Write(Get<int>(SteganoMetadataV1Keys.DataPayloadLength));         // 4 bytes
      writer.WriteGuardedBytes(Get<byte[]>(SteganoMetadataV1Keys.DataPayload),
        Get<int>(SteganoMetadataV1Keys.DataPayloadLength));                    // 1 byte + Variable + 1 byte

      return stream.ToArray();
    }
  }
}
