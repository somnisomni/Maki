namespace Somni.Maki.Core.Metadata.Creator.Versions {
  public enum CreatorMetadataV1Keys {
    NameLength,
    Name,
    Website1Length,
    Website1Url,
    Website2Length,
    Website2Url,
    Website3Length,
    Website3Url,
  }
  
  public class CreatorMetadataV1 : MakiMetadataBase<CreatorMetadataV1Keys> {
    public override ushort MetadataVersion => 1;
    
    public override void InitializeFromBytes(ReadOnlySpan<byte> bytes) {
      throw new NotImplementedException();
    }
    
    public override ReadOnlySpan<byte> ToBytes() {
      throw new NotImplementedException();
    }
  }
}
