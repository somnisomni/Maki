using Somni.Maki.Core.Metadata;
using Somni.Maki.Core.Metadata.Steganography.Versions;
using SMU = Somni.Maki.Core.Metadata.Steganography.SteganoMetadataUtility;

namespace Somni.Maki.Core.Test.Metadata.Steganography {
  [TestFixture]
  public class SteganoMetadataUtility {
    [Test]
    public void CreateFromBytes_EmptyBytes() {
      byte[] input = [ ];
      
      Assert.That(
        del: () => SMU.CreateFromBytes(input),
        expr: Throws.Exception.InstanceOf<EndOfStreamException>()
                    .Or.InstanceOf<ArgumentException>());
    }

    [Test]
    public void CreateFromBytes_MagicNumberOnly() {
      byte[] input = [ ..Constants.Magic ];
      
      Assert.That(
        del: () => SMU.CreateFromBytes(input),
        expr: Throws.Exception.InstanceOf<EndOfStreamException>()
                                .Or.InstanceOf<ArgumentException>());
    }

    [Test]
    public void CreateFromBytes_MagicNumberAndVersionOnly() {
      byte[] input = [ ..Constants.Magic, 0x01, 0x00 ];  // Version 1 (little-endian)
      
      Assert.Throws<InvalidOperationException>(() => SMU.CreateFromBytes(input));
    }

    [Test]
    public void CreateFromBytes_MalformedMetadata() {
      byte[] input = [ ..Constants.Magic, 0x01, 0x00, 0xAA, 0xBB, 0xCC ];
      
      Assert.Throws<InvalidOperationException>(() => SMU.CreateFromBytes(input));
    }

    [Test]
    public void CreateFromBytes_UnsupportedVersion() {
      byte[] input = [ ..Constants.Magic, 0x99, 0x88, 0x00, 0x00, 0x00 ];
      
      Assert.Throws<ArgumentException>(() => SMU.CreateFromBytes(input));
    }

    [Test]
    public void CreateFromBytes_MetadataV1() {
      byte[] input = [
        ..Constants.Magic,  // magic number
        0x01, 0x00,         // metadata version
        0x03, 0x00, 0x00, 0x00,        // hash length
        0x1F, 0x41, 0x42, 0x43, 0x1E,  // hash data
        0x03, 0x00, 0x00, 0x00,        // signature length
        0x1F, 0xAA, 0xBB, 0xCC, 0x1E,  // signature data
        0x13, 0x00, 0x00, 0x00,        // payload length     // | payload data
        0x1F, 0x7B, 0x20, 0x22, 0x73, 0x75, 0x63, 0x63, 0x65, 0x73, 0x73, 0x22, 0x3A, 0x20, 0x74, 0x72, 0x75, 0x65, 0x20, 0x7D, 0x1E,
      ];

      MakiMetadataBase metadata = SMU.CreateFromBytes(input);
      Assert.That(metadata.MetadataVersion, Is.EqualTo(1));
      
      MakiMetadataBase metadataFromSpan = SMU.CreateFromBytes(input.AsSpan());
      Assert.That(metadataFromSpan.ToBytes().ToArray(), Is.EqualTo(metadata.ToBytes().ToArray()));
      
      SteganoMetadataV1 metadataV1 = (SteganoMetadataV1)metadata;
      using(Assert.EnterMultipleScope()) {
        Assert.That(metadataV1.GetInt32(SteganoMetadataV1Keys.PixelDataHashLength), Is.EqualTo(0x03));
        Assert.That(metadataV1.GetBytes(SteganoMetadataV1Keys.PixelDataHash).ToArray(), Is.EqualTo("ABC"u8.ToArray()));
        Assert.That(metadataV1.GetInt32(SteganoMetadataV1Keys.PixelDataSignatureLength), Is.EqualTo(0x03));
        Assert.That(metadataV1.GetBytes(SteganoMetadataV1Keys.PixelDataSignature).ToArray(), Is.EqualTo([ 0xAA, 0xBB, 0xCC ]));
        Assert.That(metadataV1.GetInt32(SteganoMetadataV1Keys.DataPayloadLength), Is.EqualTo(0x13));
        Assert.That(metadataV1.GetBytes(SteganoMetadataV1Keys.DataPayload).ToArray(), Is.EqualTo("{ \"success\": true }"u8.ToArray()));
      }
    }
  }
}
