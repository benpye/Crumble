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
        public Dictionary<int, Channel> Channels = new Dictionary<int, Channel>();

        private void HandleChannelState(Stream s)
        {
            var cs = Serializer.Deserialize<ProtoBuf.ChannelState>(s);

            int id = (int)cs.channel_id;

            if (!Channels.ContainsKey(id))
            {
                Channels[id] = new Channel();
                Channels[id].Links = new List<uint>();
            }

            if(cs.channel_idSpecified)
                Channels[id].Id = cs.channel_id;

            if(cs.parentSpecified)
                Channels[id].Parent = cs.parent;

            if(cs.nameSpecified)
                Channels[id].Name = cs.name;

            if(cs.links.Count > 0)
                Channels[id].Links = cs.links;

            if(cs.links_add.Count > 0)
                Channels[id].Links.AddRange(cs.links_add);

            if(cs.links_remove.Count > 0)
                Channels[id].Links = Channels[id].Links.Except(cs.links_remove).ToList();

            if(cs.descriptionSpecified)
                Channels[id].Description = cs.description;

            if(cs.temporarySpecified)
                Channels[id].Temporary = cs.temporary;

            if(cs.positionSpecified)
                Channels[id].Position = cs.position;

            //Console.WriteLine("Channel ID: {0}, Parent: {1}, Name: {2}, Links: ({3}), Description: {4}, Temporary: {5}, Position: {6}",
            //    Channels[id].Id, Channels[id].Parent, Channels[id].Name, string.Join(",", Channels[id].Links), Channels[id].Description, Channels[id].Temporary, Channels[id].Position);
        }

        private void HandleChannelRemove(Stream s)
        {
            var cr = Serializer.Deserialize<ProtoBuf.ChannelRemove>(s);

            if (Channels.ContainsKey((int)cr.channel_id))
                Channels.Remove((int)cr.channel_id);
        }
    }
}
