namespace Somni.Maki.Core.Metadata {
  public interface IMakiMetadata {
    public ushort MetadataVersion { get; }  // Metadata version always should be in the first place of the header
    
    public ReadOnlySpan<byte> ToBytes();
    public IMakiMetadata FromBytes(ReadOnlySpan<byte> bytes);
  }
}
