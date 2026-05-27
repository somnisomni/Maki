namespace Somni.Maki.Core.Metadata {
  public static class Constants {
    public static readonly byte[] Magic = "39MAKI39"u8.ToArray();
    public const byte VariableStart = 0x1F;
    public const byte VariableEnd = 0x1E;
  }
}
