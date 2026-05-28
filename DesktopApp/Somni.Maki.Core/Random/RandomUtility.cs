using SecurityRNG = System.Security.Cryptography.RandomNumberGenerator;

namespace Somni.Maki.Core.Random {
  public static class RandomUtility {
    public static (byte[] nonce, string base64) GenerateNonce(int byteCount = 16) {
      if(byteCount <= 0) {
        throw new ArgumentOutOfRangeException(nameof(byteCount), "Byte count must be a positive integer.");
      }
      
      byte[] nonce = SecurityRNG.GetBytes(byteCount);
      string base64 = Convert.ToBase64String(nonce);
      
      return (nonce, base64);
    }
  }
}
