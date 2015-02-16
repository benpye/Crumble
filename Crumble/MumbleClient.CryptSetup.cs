using ProtoBuf;
using System.IO;

namespace Crumble
{
    public partial class MumbleClient
    {
        public byte[] CryptKey { get; private set; }
        public byte[] CryptClientNonce { get; private set; }
        public byte[] CryptServerNonce { get; private set; }

        private void HandleCryptSetup(Stream s)
        {
            var cs = Serializer.Deserialize<ProtoBuf.CryptSetup>(s);

            CryptKey = cs.key;
            CryptClientNonce = cs.client_nonce;
            CryptServerNonce = cs.server_nonce;
        }
    }
}
