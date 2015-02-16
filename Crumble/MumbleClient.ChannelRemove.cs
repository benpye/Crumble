using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crumble
{
    public partial class MumbleClient
    {
        private void HandleChannelRemove(Stream s)
        {
            var cr = Serializer.Deserialize<ProtoBuf.ChannelRemove>(s);

            if (Channels.ContainsKey((int)cr.channel_id))
                Channels.Remove((int)cr.channel_id);
        }
    }
}
