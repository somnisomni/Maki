namespace Somni.Maki.Core.Random {
  public interface IMakiRandomAlgorithm {
    public void SetSeed(ulong seed);
    public ulong Next();
    public double NextDouble();
  }
}
