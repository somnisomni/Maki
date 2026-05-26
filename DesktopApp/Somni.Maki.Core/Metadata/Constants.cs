namespace Somni.Maki.Core.Metadata {
  public static class Constants {
    public static readonly byte[] Magic = "MAKI39"u8.ToArray();
    public static readonly byte VariableStart = 0x1F;
    public static readonly byte VariableEnd = 0x1E;
  }
}
