using Somni.Maki.Core.Random;
using RNG = Somni.Maki.Core.Random.RandomNumberGenerator;

namespace Somni.Maki.Core.Test.Random.Algorithms {
  [TestFixture]
  public class Xoroshiro128StarStar {
    private RNG _rng;
    private RNG _rngZeroSeed;
    
    [SetUp]
    public void Setup() {
      _rng = new RNG(1234567890UL, RandomAlgorithm.Xoroshiro128StarStar);
      _rngZeroSeed = new RNG(0UL, RandomAlgorithm.Xoroshiro128StarStar);
    }

    [Test]
    public void Next() {
      ulong value = _rng.Next();
      
      TestContext.Out.WriteLine($"Random value: {value}");
      Assert.That(value, Is.Not.Zero
                           .And.Not.Null);
    }

    [Test]
    public void NextZeroSeed() {
      ulong value = _rngZeroSeed.Next();
      
      TestContext.Out.WriteLine($"Random value: {value}");
      Assert.That(value, Is.Not.Zero
                           .And.Not.Null);
    }
  }
}
