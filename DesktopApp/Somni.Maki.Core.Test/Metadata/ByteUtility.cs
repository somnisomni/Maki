using System.Text;
using Somni.Maki.Core.Metadata;

namespace Somni.Maki.Core.Test.Metadata {
  [TestFixture]
  public class ByteUtility {
    [Test]
    public void GetBytes_EmptyString() {
      string input = string.Empty;
      byte[] expectedBytes = [ ];

      byte[] actualBytes = input.GetBytes(out int actualBytesCount);
      
      Assert.That(actualBytes, Is.EqualTo(expectedBytes));
      Assert.That(actualBytes, Has.Length.EqualTo(expectedBytes.Length));
      Assert.That(actualBytesCount, Is.EqualTo(expectedBytes.Length));
    }

    [Test]
    public void GetBytes_String() {
      const string input = "미쿠";
      byte[] expectedBytes = [ 0xEB, 0xAF, 0xB8, 0xEC, 0xBF, 0xA0 ];
      
      byte[] actualBytes = input.GetBytes(out int actualBytesCount, Encoding.UTF8);
      
      Assert.That(actualBytes, Is.EqualTo(expectedBytes));
      Assert.That(actualBytes, Has.Length.EqualTo(expectedBytes.Length));
      Assert.That(actualBytesCount, Is.EqualTo(expectedBytes.Length));
    }

    [Test]
    public void GetBytes_AutoTrim() {
      const string input = "   abc   \n";
      byte[] expectedBytes = "abc"u8.ToArray();
      
      byte[] actualBytes = input.GetBytes(out int actualBytesCount);
      
      Assert.That(actualBytes, Is.EqualTo(expectedBytes));
      Assert.That(actualBytes, Has.Length.EqualTo(expectedBytes.Length));
      Assert.That(actualBytesCount, Is.EqualTo(expectedBytes.Length));
    }
    
    [Test]
    public void WriteGuardedBytes_EmptyString() {
      string input = string.Empty;
      byte[] expectedBytes = [ Constants.VariableStart, Constants.VariableEnd ];

      using MemoryStream stream = new();
      using BinaryWriter writer = new(stream);
      
      writer.WriteGuardedBytes(input.GetBytes(out _));

      byte[] actualBytes = stream.ToArray();

      Assert.That(actualBytes, Is.EqualTo(expectedBytes));
      Assert.That(actualBytes, Has.Length.EqualTo(expectedBytes.Length));
    }

    [Test]
    public void WriteGuardedBytes_EmptyStringWithWrongCount() {
      string input = string.Empty;
      byte[] expectedBytes = [ ];
      
      using MemoryStream stream = new();
      using BinaryWriter writer = new(stream);
      
      Assert.Throws<ArgumentOutOfRangeException>(() => writer.WriteGuardedBytes(input.GetBytes(out _), 123));
      
      byte[] actualBytes = stream.ToArray();
      
      Assert.That(actualBytes, Is.EqualTo(expectedBytes));
      Assert.That(actualBytes, Has.Length.EqualTo(expectedBytes.Length));
    }

    [Test]
    public void WriteGuardedBytes_String() {
      const string input = "abc미쿠";
      byte[] expectedBytes = [ Constants.VariableStart, 0x61, 0x62, 0x63, 0xEB, 0xAF, 0xB8, 0xEC, 0xBF, 0xA0, Constants.VariableEnd ];

      using MemoryStream stream = new();
      using BinaryWriter writer = new(stream);
      
      writer.WriteGuardedBytes(input.GetBytes(out _, Encoding.UTF8));

      byte[] actualBytes = stream.ToArray();

      Assert.That(actualBytes, Is.EqualTo(expectedBytes));
      Assert.That(actualBytes, Has.Length.EqualTo(expectedBytes.Length));
    }

    [Test]
    public void ReadGuardedBytes_EmptyPayload() {
      byte[] inputBytes = [ Constants.VariableStart, Constants.VariableEnd ];
      
      using MemoryStream stream = new(inputBytes);
      using BinaryReader reader = new(stream);

      byte[] actualBytes = reader.ReadGuardedBytes(0);
      
      Assert.That(actualBytes, Is.EqualTo(Array.Empty<byte>()));
      Assert.That(actualBytes, Has.Length.EqualTo(0));
    }

    [Test]
    public void ReadGuardedBytes_WithPayload() {
      byte[] inputBytes = [ Constants.VariableStart, 0x39, 0x93, 0x39, 0x93, Constants.VariableEnd ];
      
      using MemoryStream stream = new(inputBytes);
      using BinaryReader reader = new(stream);
      
      byte[] actualBytes = reader.ReadGuardedBytes(4);
      
      Assert.That(actualBytes, Is.EqualTo([ 0x39, 0x93, 0x39, 0x93 ]));
      Assert.That(actualBytes, Has.Length.EqualTo(4));
    }

    [Test]
    public void ReadGuardedBytes_NoGuardBytes() {
      byte[] inputBytes = [ 0x39, 0x93, 0x39, 0x93 ];
      
      using MemoryStream stream = new(inputBytes);
      using BinaryReader reader = new(stream);

      byte[] actualBytes = [ ];
      Assert.Throws<InvalidOperationException>(() => actualBytes = reader.ReadGuardedBytes(4));
      
      Assert.That(actualBytes, Is.EqualTo(Array.Empty<byte>()));
      Assert.That(actualBytes, Has.Length.EqualTo(0));
    }

    [Test]
    public void ReadGuardedBytes_NoEndGuardByteAtStreamEnd() {
      byte[] inputBytes = [ Constants.VariableStart, 0x39, 0x93, 0x39, 0x93 ];
      
      using MemoryStream stream = new(inputBytes);
      using BinaryReader reader = new(stream);

      byte[] actualBytes = [ ];
      Assert.That(
        del: () => actualBytes = reader.ReadGuardedBytes(4),
        expr: Throws.Exception.InstanceOf<InvalidOperationException>()
                    .Or.InstanceOf<EndOfStreamException>());
      
      Assert.That(actualBytes, Is.EqualTo(Array.Empty<byte>()));
      Assert.That(actualBytes, Has.Length.EqualTo(0));
    }

    [Test]
    public void ReadGuardedBytes_NoEndGuardByteAfterCount() {
      byte[] inputBytes = [ Constants.VariableStart, 0x39, 0x93, 0x39, 0x93, 0xAA ];
      
      using MemoryStream stream = new(inputBytes);
      using BinaryReader reader = new(stream);
      
      byte[] actualBytes = [ ];
      Assert.Throws<InvalidOperationException>(() => actualBytes = reader.ReadGuardedBytes(4));
      
      Assert.That(actualBytes, Is.EqualTo(Array.Empty<byte>()));
      Assert.That(actualBytes, Has.Length.EqualTo(0));
    }

    [Test]
    public void ReadGuardedBytes_ReadBeyondStream() {
      byte[] inputBytes = [ Constants.VariableStart, 0x39, 0x93, 0x39, 0x93, Constants.VariableEnd ];
      
      using MemoryStream stream = new(inputBytes);
      using BinaryReader reader = new(stream);
      
      byte[] actualBytes = [ ];
      Assert.Throws<EndOfStreamException>(() => actualBytes = reader.ReadGuardedBytes(3939));
      
      Assert.That(actualBytes, Is.EqualTo(Array.Empty<byte>()));
      Assert.That(actualBytes, Has.Length.EqualTo(0));
    }
  }
}
