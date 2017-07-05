using Guarddog.Data;
using Guarddog.Data.Bans;
using Guarddog.IrcClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guarddog.Modules.Akick
{
    public class AkickModule
    {
        private readonly Client client;
        private readonly Channel opChannel;
        private readonly IReadOnlyList<Channel> adminChannels;
        private readonly BanRepository banRepository;

        public AkickModule(Client client, List<AkickModuleChannelConfig> config, BanRepository banRepository)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.opChannel = opChannel ?? throw new ArgumentNullException(nameof(opChannel));
            this.adminChannels = adminChannels ?? throw new ArgumentNullException(nameof(adminChannels));
            this.banRepository = banRepository ?? throw new ArgumentNullException(nameof(banRepository));

            this.opChannel.BanListReloaded += this.BanListReloaded;
            this.opChannel.BanAdded += this.BanAdded;
            this.opChannel.BanRemoved += this.BanRemoved;

            foreach (var adminChannel in this.adminChannels)
            {
                adminChannel.Messaged += this.AdminChannelMessaged;
            }
        }

        private void BanListReloaded(object sender, BanListReloadedEventArgs e)
        {
            var banList = e.BanList;

            this.banRepository.InsertBans(banList.Select(this.BanToBanRecord));
        }

        private void BanAdded(object sender, BanChangedEventArgs e)
        {
            this.banRepository.InsertBans(new[] { this.BanToBanRecord(e.Ban) });
        }

        private void BanRemoved(object sender, BanChangedEventArgs e)
        {
            this.banRepository.DeleteBan(this.BanToBanRecord(e.Ban));
        }

        private BanRecord BanToBanRecord(UserBan ban)
        {
            return new BanRecord()
            {
                Channel = this.opChannel.Name,
                Mask = ban.Mask,
                FromUser = ban.From,
                Date = ban.Date,
                Type = ban.Type,
            };
        }

        private void AdminChannelMessaged(object sender, MessagedEventArgs e)
        {
        }
    }
}
