using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crumble
{
    public class User
    {
        public uint Session { get; internal set; }
        public uint Actor { get; internal set; }
        public string Name { get; internal set; }
        public uint UserId { get; internal set; }
        public uint ChannelId { get; internal set; }
        public bool Mute { get; internal set; }
        public bool Deaf { get; internal set; }
        public bool Suppress { get; internal set; }
        public bool SelfMute { get; internal set; }
        public bool SelfDeaf { get; internal set; }
        public byte[] Texture { get; internal set; }
        public byte[] PluginContext { get; internal set; }
        public string PluginIdentity { get; internal set; }
        public string Comment { get; internal set; }
        public string Hash { get; internal set; }
        public byte[] CommentHash { get; internal set; }
        public byte[] TextureHash { get; internal set; }
        public bool PrioritySpeaker { get; internal set; }
        public bool Recording { get; internal set; }
    }
}
