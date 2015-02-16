using ProtoBuf;
using System;
using System.IO;

namespace Crumble
{
    public partial class MumbleClient
    {
        public string ServerOS { get; private set; }
        public string ServerOSVersion { get; private set; }
        public uint ServerVersion { get; private set; }
        public uint ServerVersionMajor { get { return (ServerVersion >> 16) & 0xFFFF; } }
        public uint ServerVersionMinor { get { return (ServerVersion >> 8) & 0xFF; } }
        public uint ServerVersionPatch { get { return ServerVersion & 0xFF; } }
        public string ServerRelease { get; private set; }

        private void HandleVersion(Stream s)
        {
            var v = Serializer.Deserialize<ProtoBuf.Version>(s);

            ServerOS = v.os;
            ServerOSVersion = v.os_version;
            ServerVersion = v.version;
            ServerRelease = v.release;
            
            //Console.WriteLine("Server OS: {0}, OS Version: {1}, Version: {2}.{3}.{4}, Release: {5}",
            //    ServerOS, ServerOSVersion, ServerVersionMajor, ServerVersionMinor,
            //    ServerVersionPatch, ServerRelease);
        }
    }
}
