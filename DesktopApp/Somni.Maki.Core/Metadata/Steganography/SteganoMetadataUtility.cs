using System.Collections.ObjectModel;
using Somni.Maki.Core.Metadata.Steganography.Versions;

namespace Somni.Maki.Core.Metadata.Steganography {
  public static class SteganoMetadataUtility {
    private static readonly ReadOnlyDictionary<ushort, Func<MakiMetadataBase>> InstantiatorsByVersion = new(new Dictionary<ushort, Func<MakiMetadataBase>> {
      { 1, () => new SteganoMetadataV1() },
    });
    
    public static MakiMetadataBase CreateFromBytes(byte[] bytes) {
      Func<MakiMetadataBase>? metadataInstantiator;
      byte[] remainingBytes;
      
      using(MemoryStream stream = new(bytes)) {
        using BinaryReader reader = new(stream);

        byte[] magic = reader.ReadBytes(Constants.Magic.Length);  // 8 bytes (according to Constants.Magic)
        if(!magic.SequenceEqual(Constants.Magic)) {
          throw new ArgumentException("Invalid magic number.");
        }
        
        ushort version = reader.ReadUInt16();  // 2 bytes
        if(!InstantiatorsByVersion.TryGetValue(version, out metadataInstantiator)) {
          throw new ArgumentException($"Metadata version from input bytes is {version}, which is not supported.");
        }

        if(metadataInstantiator == null) {
          throw new InvalidOperationException($"Invalid metadata instantiator definition for the version {version}.");
        }

        remainingBytes = reader.ReadBytes((int)(stream.Length - stream.Position));
      }
      
      if(remainingBytes.Length <= 0) {
        throw new InvalidOperationException("No remaining bytes available to parse metadata content.");
      }

      try {
        MakiMetadataBase instance = metadataInstantiator();
        instance.InitializeFromBytes(remainingBytes);

        return instance;
      } catch(Exception exception) {
        throw new InvalidOperationException("Metadata malformed or invalid.", exception);
      }
    }
    
    public static MakiMetadataBase CreateFromBytes(ReadOnlySpan<byte> bytes) {
      return CreateFromBytes(bytes.ToArray());
    }
  }
}
