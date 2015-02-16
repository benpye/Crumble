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
        Dictionary<int, User> Users = new Dictionary<int, User>();

        private void HandleUserState(Stream s)
        {
            var us = Serializer.Deserialize<ProtoBuf.UserState>(s);

            int id = (int)us.session;

            if (!Users.ContainsKey(id))
                Users[id] = new User();

            if (us.sessionSpecified)
                Users[id].Session = us.session;

            if (us.actorSpecified)
                Users[id].Actor = us.actor;

            if (us.nameSpecified)
                Users[id].Name = us.name;

            if (us.user_idSpecified)
                Users[id].UserId = us.user_id;

            if (us.channel_idSpecified)
                Users[id].ChannelId = us.channel_id;

            if (us.muteSpecified)
                Users[id].Mute = us.mute;

            if (us.deafSpecified)
                Users[id].Deaf = us.deaf;

            if (us.suppressSpecified)
                Users[id].Suppress = us.suppress;

            if (us.self_muteSpecified)
                Users[id].SelfMute = us.self_mute;

            if (us.self_deafSpecified)
                Users[id].SelfDeaf = us.self_deaf;

            if (us.textureSpecified)
                Users[id].Texture = us.texture;

            if (us.plugin_contextSpecified)
                Users[id].PluginContext = us.plugin_context;

            if (us.plugin_identitySpecified)
                Users[id].PluginIdentity = us.plugin_identity;

            if (us.commentSpecified)
                Users[id].Comment = us.comment;

            if (us.hashSpecified)
                Users[id].Hash = us.hash;

            if (us.comment_hashSpecified)
                Users[id].CommentHash = us.comment_hash;

            if (us.texture_hashSpecified)
                Users[id].TextureHash = us.texture_hash;

            if (us.priority_speakerSpecified)
                Users[id].PrioritySpeaker = us.priority_speaker;

            if (us.recordingSpecified)
                Users[id].Recording = us.recording;
        }

        private void HandleUserRemove(Stream s)
        {
            var ur = Serializer.Deserialize<ProtoBuf.UserRemove>(s);

            if (Users.ContainsKey((int)ur.session))
                Users.Remove((int)ur.session);
        }
    }
}
