namespace Somni.Maki.Core.Metadata {
  public abstract class MakiMetadataBase {
    public abstract ushort MetadataVersion { get; }

    protected virtual Dictionary<int, object> Properties { get; set; } = new();
    
    public abstract void InitializeFromBytes(ReadOnlySpan<byte> bytes);
    public abstract ReadOnlySpan<byte> ToBytes();

    public T Get<T>(int key) {
      if(Properties == null) {
        throw new InvalidOperationException($"Invalid property dictionary definition.");
      }
      
      if(!Properties.TryGetValue(key, out object? value)) {
        throw new ArgumentException($"Property with the key {key} is not defined.");
      }

      if(value is not T typedValue) {
        throw new InvalidOperationException($"Invalid property definition for the key {key} and the type {typeof(T).Name}.");
      }

      return typedValue;
    }
  }
  
  public abstract class MakiMetadataBase<TKey> : MakiMetadataBase where TKey : Enum {
    protected new virtual Dictionary<TKey, object> Properties { get; } = new();

    protected MakiMetadataBase() {
      UpdateBaseDictionaries();
    }

    protected void UpdateBaseDictionaries() {
      base.Properties = Properties.ToDictionary(pair => Convert.ToInt32(pair.Key), pair => pair.Value);
    }

    public T Get<T>(TKey key) => base.Get<T>(Convert.ToInt32(key));
  }
}
