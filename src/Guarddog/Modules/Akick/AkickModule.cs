using Guarddog.Data;
using Guarddog.Data.Bans;
using Guarddog.IrcClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guarddog.Modules.Akick
{
    public class AkickModule : IModule, IDisposable
    {
        public IReadOnlyList<AkickModuleChannel> Channels { get; }

        public AkickModule(Client client, AKickModuleConfig config, BanRepository banRepository)
        {
            var channels = new List<AkickModuleChannel>();
            foreach (var channelConfig in config.Channels)
            {
                channels.Add(new AkickModuleChannel(client, config, channelConfig, banRepository));
            }

            this.Channels = channels;
        }

        public void Load()
        {
            foreach (var channel in this.Channels)
            {
                channel.Load();
            }
        }

        public void Unload()
        {
            foreach (var channel in this.Channels)
            {
                channel.Unload();
            }
        }

        public void Dispose()
        {
            this.Unload();
        }
    }
}
