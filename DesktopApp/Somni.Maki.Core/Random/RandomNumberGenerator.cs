using System.Collections.ObjectModel;
using Somni.Maki.Core.Random.Algorithms;

namespace Somni.Maki.Core.Random {
  public enum RandomAlgorithm {
    Xoroshiro128StarStar,
  }
  
  public class RandomNumberGenerator {
    private static readonly ReadOnlyDictionary<RandomAlgorithm, Func<IMakiRandomAlgorithm>> AlgorithmGenerators = new(new Dictionary<RandomAlgorithm, Func<IMakiRandomAlgorithm>> {
      { RandomAlgorithm.Xoroshiro128StarStar, () => new Xoroshiro128StarStar() },
    });

    private readonly IMakiRandomAlgorithm _algorithm;

    public ulong InitialSeed { get; }
    public ulong CurrentStep { get; private set; } = 0;
    
    public RandomNumberGenerator(ulong? initSeed = null, RandomAlgorithm algorithm = RandomAlgorithm.Xoroshiro128StarStar) {
      initSeed ??= (ulong)DateTime.Now.Ticks ^ (ulong)Environment.ProcessId;
      InitialSeed = initSeed.Value;
      
      _algorithm = AlgorithmGenerators[algorithm]();
      _algorithm.SetSeed(InitialSeed);
    }

    public ulong Next() {
      CurrentStep++;
      return _algorithm.Next();
    }

    public double NextDouble() {
      CurrentStep++;
      return _algorithm.NextDouble();
    }

    public bool NextBoolean() {
      CurrentStep++;
      return _algorithm.Next() % 2 == 0;
    }

    public byte NextByte() {
      CurrentStep++;
      return (byte)(_algorithm.Next() & 0xFF);
    }
  }
}
