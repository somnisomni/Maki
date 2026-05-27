namespace Somni.Maki.Core.Metadata {
  public abstract class MakiMetadataBase {
    public abstract ushort MetadataVersion { get; }

    protected virtual Dictionary<int, byte[]> BytesProperties { get; set; } = new();
    protected virtual Dictionary<int, int> Int32Properties { get; set; } = new();
    protected virtual Dictionary<int, ushort> UInt16Properties { get; set; } = new();

    public abstract void InitializeFromBytes(ReadOnlySpan<byte> bytes);
    public abstract ReadOnlySpan<byte> ToBytes();
    
    public ReadOnlySpan<byte> GetBytes(int key) => Get<byte[]>(key);
    public int GetInt32(int key) => Get<int>(key);
    public ushort GetUInt16(int key) => Get<ushort>(key);

    protected T Get<T>(int key) {
      IDictionary<int, T>? targetDictionary = typeof(T) switch {
        { } type when type == typeof(byte[]) => BytesProperties as IDictionary<int, T>,
        { } type when type == typeof(int) => Int32Properties as IDictionary<int, T>,
        { } type when type == typeof(ushort) => UInt16Properties as IDictionary<int, T>,
        _ => throw new ArgumentException("Invalid property type."),
      };

      if(targetDictionary == null) {
        throw new InvalidOperationException($"Invalid property dictionary definition for the type {typeof(T).Name}.");
      }
      
      if(!targetDictionary.TryGetValue(key, out T? value)) {
        throw new ArgumentException($"Property with the key {key} is not defined for the type {typeof(T).Name}.");
      }

      if(value == null) {
        throw new InvalidOperationException($"Invalid property definition for the key {key} and the type {typeof(T).Name}.");
      }

      return value;
    }
  }
  
  public abstract class MakiMetadataBase<TKey> : MakiMetadataBase where TKey : Enum {
    protected new virtual Dictionary<TKey, byte[]> BytesProperties { get; } = new();
    protected new virtual Dictionary<TKey, int> Int32Properties { get; } = new();
    protected new virtual Dictionary<TKey, ushort> UInt16Properties { get; } = new();

    protected MakiMetadataBase() {
      InitializeBase();
    }
    
    public ReadOnlySpan<byte> GetBytes(TKey key) => Get<byte[]>(Convert.ToInt32(key));
    public int GetInt32(TKey key) => Get<int>(Convert.ToInt32(key));
    public ushort GetUInt16(TKey key) => Get<ushort>(Convert.ToInt32(key));

    private void InitializeBase() {
      base.BytesProperties = BytesProperties.ToDictionary(pair => Convert.ToInt32(pair.Key), pair => pair.Value);
      base.Int32Properties = Int32Properties.ToDictionary(pair => Convert.ToInt32(pair.Key), pair => pair.Value);
      base.UInt16Properties = UInt16Properties.ToDictionary(pair => Convert.ToInt32(pair.Key), pair => pair.Value);
    }
  }
}
