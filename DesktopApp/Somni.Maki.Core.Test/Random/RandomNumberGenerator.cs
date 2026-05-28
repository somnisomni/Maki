using RNG = Somni.Maki.Core.Random.RandomNumberGenerator;

namespace Somni.Maki.Core.Test.Random {
  [TestFixture]
  public class RandomNumberGenerator {
    [Test]
    public void Constructor_Defaults() {
      RNG rng = new();
      
      Assert.That(rng.CurrentStep, Is.EqualTo(0));
      Assert.That(rng.InitialSeed, Is.Not.EqualTo(0)
                                     .And.Not.Null);
    }

    [Test]
    public void Next() {
      RNG rng = new();
      Assert.That(rng.CurrentStep, Is.EqualTo(0));

      ulong value = rng.Next();
      TestContext.Out.WriteLine($"Random value: {value}");
      Assert.That(rng.CurrentStep, Is.EqualTo(1));
      Assert.That(value, Is.GreaterThan(0));
    }

    [Test]
    public void NextDouble() {
      RNG rng = new();
      Assert.That(rng.CurrentStep, Is.EqualTo(0));
      
      double value = rng.NextDouble();
      TestContext.Out.WriteLine($"Random value: {value}");
      Assert.That(rng.CurrentStep, Is.EqualTo(1));
      Assert.That(value, Is.GreaterThan(0.0d)
                           .And.LessThan(1.0d));
    }

    [Test]
    public void NextBoolean() {
      RNG rng = new();
      Assert.That(rng.CurrentStep, Is.EqualTo(0));
      
      bool value = rng.NextBoolean();
      TestContext.Out.WriteLine($"Random value: {value}");
      Assert.That(rng.CurrentStep, Is.EqualTo(1));
      Assert.That(value, Is.True.Or.False);
    }

    [Test]
    public void NextByte() {
      RNG rng = new();
      Assert.That(rng.CurrentStep, Is.EqualTo(0));
      
      byte value = rng.NextByte();
      TestContext.Out.WriteLine($"Random value: 0x{value:X2} (0b{value:B8})");
      Assert.That(rng.CurrentStep, Is.EqualTo(1));
      Assert.That(value, Is.GreaterThan(0));
    }
  }
}
