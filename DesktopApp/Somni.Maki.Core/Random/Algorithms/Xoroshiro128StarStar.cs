using System.Runtime.CompilerServices;

namespace Somni.Maki.Core.Random.Algorithms {
  // https://prng.di.unimi.it/xoroshiro128starstar.c
  internal class Xoroshiro128StarStar : IMakiRandomAlgorithm {
    private ulong _state0, _state1;

    public void SetSeed(ulong seed) {
      _state0 = SplitMix64(ref seed);
      _state1 = SplitMix64(ref seed);
    }

    public ulong Next() {
      ulong s0 = _state0;
      ulong s1 = _state1;
      ulong result = Rotl(s0 * 5, 7) * 9;

      s1 ^= s0;
      _state0 = Rotl(s0, 24) ^ s1 ^ (s1 << 16); // a, b
      _state1 = Rotl(s1, 37); // c

      return result;
    }
    
    public double NextDouble() {
      // Take only 53 bits of random data to fit in the precision of a double-precision floating-point number
      // The value will be in range between 0.0d ~ 1.0d
      return (Next() >> 11) * (1.0 / (1UL << 53));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong Rotl(ulong x, int k) {
      return (x << k) | (x >> (64 - k));
    }
    
    private static ulong SplitMix64(ref ulong state) {
      // https://xoshiro.di.unimi.it/splitmix64.c
      ulong z = (state += 0x9E3779B97F4A7C15UL);
      z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
      z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
      return z ^ (z >> 31);
    }
  }
}
