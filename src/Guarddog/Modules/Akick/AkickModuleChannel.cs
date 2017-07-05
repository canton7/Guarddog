using Guarddog.Data.Bans;
using Guarddog.IrcClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrcSays.Communication.Irc;

namespace Guarddog.Modules.Akick
{
    public class AkickModuleChannel : IDisposable
    {
        private readonly Client client;
        private readonly Channel opChannel;
        private readonly IReadOnlyList<Channel> adminChannels;
        private readonly BanRepository banRepository;
        private readonly AKickModuleConfig moduleConfig;
        private readonly CommandRegistration banCommand = new CommandRegistration("ban", HandleBanCommand);

        internal AkickModuleChannel(Client client, AKickModuleConfig moduleConfig, AkickModuleChannelConfig config, BanRepository banRepository)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.opChannel = config.OpChannel ?? throw new ArgumentNullException(nameof(opChannel));
            this.adminChannels = config.AdminChannels ?? throw new ArgumentNullException(nameof(adminChannels));
            this.banRepository = banRepository ?? throw new ArgumentNullException(nameof(banRepository));
            this.moduleConfig = moduleConfig ?? throw new ArgumentNullException(nameof(moduleConfig));

            this.client.AddPrivateMessageHandler(this.banCommand);
            foreach (var adminChannel in this.adminChannels)
            {
                adminChannel.AddPrivateMessageHandler(this.banCommand);
            }
        }

        public void Load()
        {
            this.opChannel.BanListReloaded += this.BanListReloaded;
            this.opChannel.BanAdded += this.BanAdded;
            this.opChannel.BanRemoved += this.BanRemoved;
            this.banCommand.IsEnabled = true;
        }

        public void Unload()
        {
            this.opChannel.BanListReloaded -= this.BanListReloaded;
            this.opChannel.BanAdded -= this.BanAdded;
            this.opChannel.BanRemoved -= this.BanRemoved;
            this.banCommand.IsEnabled = false;
        }

        private static void HandleBanCommand(IrcPeer from, string command, string text)
        {
            
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

        public void Dispose()
        {
            this.Unload();
            this.client.RemovePrivateMessageHandler(this.banCommand);
            foreach (var adminChannel in this.adminChannels)
            {
                adminChannel.RemovePrivateMessageHandler(this.banCommand);
            }
        }
    }
}
