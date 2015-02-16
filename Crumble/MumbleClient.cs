using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Threading;

using ProtoBuf;

namespace Crumble
{
    public partial class MumbleClient
    {
        private readonly Dictionary<PacketType, Action<Stream>> _packetLookup;

        private TcpClient _tcpClient = new TcpClient();
        private SslStream _sslStream;

        private SemaphoreSlim _readLock = new SemaphoreSlim(1);
        private SemaphoreSlim _writeLock = new SemaphoreSlim(1);

        public MumbleClient()
        {
            _packetLookup = new Dictionary<PacketType, Action<Stream>>
            {
                { PacketType.Version,             HandleVersion },
                { PacketType.UDPTunnel,           null },
                { PacketType.Authenticate,        null },
                { PacketType.Ping,                null },
                { PacketType.Reject,              null },
                { PacketType.ServerSync,          null },
                { PacketType.ChannelRemove,       HandleChannelRemove },
                { PacketType.ChannelState,        HandleChannelState },
                { PacketType.UserRemove,          null },
                { PacketType.UserState,           null },
                { PacketType.BanList,             null },
                { PacketType.TextMessage,         null },
                { PacketType.PermissionDenied,    null },
                { PacketType.ACL,                 null },
                { PacketType.QueryUsers,          null },
                { PacketType.CryptSetup,          HandleCryptSetup },
                { PacketType.ContextActionModify, null },
                { PacketType.ContextAction,       null },
                { PacketType.UserList,            null },
                { PacketType.VoiceTarget,         null },
                { PacketType.PermissionQuery,     null },
                { PacketType.CodecVersion,        null },
                { PacketType.UserStats,           null },
                { PacketType.RequestBlob,         null },
                { PacketType.ServerConfig,        null },
                { PacketType.SuggestConfig,       null },
            };
        }

        public async Task ConnectAsync(IPEndPoint remoteEP, string hostname)
        {
            await _tcpClient.ConnectAsync(remoteEP.Address, remoteEP.Port);

            _sslStream = new SslStream(
                _tcpClient.GetStream(),
                false,
                ValidateServerCertificate,
                null
                );

            await _sslStream.AuthenticateAsClientAsync(hostname, new X509CertificateCollection(), SslProtocols.Tls, true);
        }

        public async Task PollAsync()
        {
            if (_sslStream == null || !_sslStream.CanRead)
                return;

            byte[] buffer = new byte[6];
            byte[] payload;
            PacketType type;
            int size;

            await _readLock.WaitAsync();

            try
            {
                await _sslStream.ReadAsync(buffer, 0, 6);

                type = (PacketType)(buffer[0] << 8 | buffer[1]);
                size = (buffer[2] << 24 | buffer[3] << 16 | buffer[4] << 8 | buffer[5]);

                int n = 0;
                int t = 0;

                payload = new byte[size];
                while (t != size)
                {
                    n = await _sslStream.ReadAsync(payload, t, size - t);
                    if (n == 0)
                        break;
                    t += n;
                }
            }
            finally
            {
                _readLock.Release();
            }

            //Console.WriteLine("Packet: {0}, Length: {1}", type, size);

            Action<Stream> handler;
            if(_packetLookup.TryGetValue(type, out handler))
                handler(new MemoryStream(payload));
        }

        public async Task SendPacketAsync<T>(T packet, PacketType type)
        {
            if (_sslStream == null || !_sslStream.CanWrite)
                return;

            MemoryStream ms = new MemoryStream();
            Serializer.Serialize(ms, packet);

            byte[] buffer = new byte[6];

            buffer[0] = (byte)(((int)type >> 8) & 0xFF);
            buffer[1] = (byte)(((int)type     ) & 0xFF);

            buffer[2] = (byte)(((int)ms.Length >> 24) & 0xFF);
            buffer[3] = (byte)(((int)ms.Length >> 16) & 0xFF);
            buffer[4] = (byte)(((int)ms.Length >>  8) & 0xFF);
            buffer[5] = (byte)(((int)ms.Length      ) & 0xFF);

            await _writeLock.WaitAsync();

            try
            {
                await _sslStream.WriteAsync(buffer, 0, 6);
                await _sslStream.WriteAsync(ms.GetBuffer(), 0, (int)ms.Length);
            }
            finally
            {
                _writeLock.Release();
            }
        }

        // TODO: Certificate validation should be handled by the user of the library
        private static bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
