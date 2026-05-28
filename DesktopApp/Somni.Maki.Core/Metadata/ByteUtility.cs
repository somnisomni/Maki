using System.Text;

namespace Somni.Maki.Core.Metadata {
  public static class ByteUtility {
    public static byte[] GetBytes(this string input, out int bytesCount, Encoding? encoding = null) {
      encoding ??= Encoding.UTF8;
      input = input.Trim();

      if(string.IsNullOrWhiteSpace(input)) {
        bytesCount = 0;
        return [ ];
      }
      
      byte[] buffer = new byte[input.Length * encoding.GetMaxByteCount(1)];
      if(!encoding.TryGetBytes(input, buffer, out bytesCount) || bytesCount < 0) {
        throw new ArgumentException("Failed to safely encode string to bytes.");
      }
      
      return buffer[..bytesCount];
    }

    public static byte[] ReadGuardedBytes(this BinaryReader reader, int count) {
      if(reader.ReadByte() != Constants.VariableStart) {
        throw new InvalidOperationException("Expected variable start byte not found.");
      }

      byte[] bytes = count > 0 ? reader.ReadBytes(count) : [ ];

      if(reader.ReadByte() != Constants.VariableEnd) {
        throw new InvalidOperationException("Expected variable end byte not found.");
      }

      return bytes;
    }

    public static void WriteGuardedBytes(this BinaryWriter writer, byte[] bytes, int count) {
      ArgumentOutOfRangeException.ThrowIfGreaterThan(count, bytes.Length);

      writer.Write(Constants.VariableStart);
      writer.Write(bytes.ToArray(), 0, count);
      writer.Write(Constants.VariableEnd);
    }

    public static void WriteGuardedBytes(this BinaryWriter writer, byte[] bytes) {
      writer.WriteGuardedBytes(bytes, bytes.Length);
    }
  }
}
