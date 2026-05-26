using System.Text;

namespace Somni.Maki.Core.Metadata.Steganography.Versions {
  public struct SteganoMetadataV1 : IMakiMetadata {
    public ushort MetadataVersion => 1;

    public int PixelDataHashLength { get; private set; } = 0;
    public byte[] PixelDataHash { get; private set; } = [];
    public int PixelDataSignatureLength { get; private set; } = 0;
    public byte[] PixelDataSignature { get; private set; } = [];
    public int DataPayloadLength { get; private set; } = 0;
    public byte[] DataPayload { get; private set; } = [];
    
    public SteganoMetadataV1() { }
    public SteganoMetadataV1(string pixelDataHash, ReadOnlySpan<byte> pixelDataSignature, string? dataPayload) {
      if(!string.IsNullOrEmpty(pixelDataHash)) {
        PixelDataHash = pixelDataHash.GetBytes(out int bytesCount, Encoding.ASCII);
        PixelDataHashLength = bytesCount;
      }

      if(!pixelDataSignature.IsEmpty || pixelDataSignature.Length > 0) {
        PixelDataSignature = pixelDataSignature.ToArray();
        PixelDataSignatureLength = pixelDataSignature.Length;
      }
      
      if(!string.IsNullOrEmpty(dataPayload)) {
        DataPayload = dataPayload.GetBytes(out int bytesCount);
        DataPayloadLength = bytesCount;
      }
    }
    
    public ReadOnlySpan<byte> ToBytes() {
      using MemoryStream stream = new();
      using BinaryWriter writer = new(stream);
      
      writer.Write(MetadataVersion);                                            // 2 bytes
      writer.Write(PixelDataHashLength);                                        // 4 bytes
      writer.WriteGuardedBytes(PixelDataHash, PixelDataHashLength);             // 1 byte + Variable + 1 byte
      writer.Write(PixelDataSignatureLength);                                   // 4 bytes
      writer.WriteGuardedBytes(PixelDataSignature, PixelDataSignatureLength);   // 1 byte + Variable + 1 byte
      writer.Write(DataPayloadLength);                                          // 4 bytes
      writer.WriteGuardedBytes(DataPayload, DataPayloadLength);                 // 1 byte + Variable + 1 byte

      return new ReadOnlySpan<byte>(stream.ToArray());
    }
    
    public IMakiMetadata FromBytes(ReadOnlySpan<byte> bytes) {
      using MemoryStream stream = new(bytes.ToArray());
      using BinaryReader reader = new(stream);

      ushort version = reader.ReadUInt16();                                    // 2 bytes
      if(version != MetadataVersion) {
        throw new ArgumentException($"Metadata version from input bytes is {version}, but tried to parse as version {MetadataVersion}.");
      }

      PixelDataHashLength = reader.ReadInt32();                                // 4 bytes
      PixelDataHash = reader.ReadGuardedBytes(PixelDataHashLength);            // 1 byte + Variable + 1 byte
      PixelDataSignatureLength = reader.ReadInt32();                           // 4 bytes
      PixelDataSignature = reader.ReadGuardedBytes(PixelDataSignatureLength);  // 1 byte + Variable + 1 byte
      DataPayloadLength = reader.ReadInt32();                                  // 4 bytes
      DataPayload = reader.ReadGuardedBytes(DataPayloadLength);                // 1 byte + Variable + 1 byte

      return this;
    }
  }
}
