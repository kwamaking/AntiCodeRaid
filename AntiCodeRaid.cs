using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Plugins
{
    [Info( "Anti Code Raid", "kwamaking", "1.0.2" )]
    [Description( "Prevents players from code raiding if they're not on your team, TC authed, or the owner." )]
    class AntiCodeRaid : RustPlugin
    {
        private AntiCodeRaidConfiguration pluginConfiguration { get; set; }
        private const string UsePermission = "anticoderaid.enabled";
        private const string DefaultPrefixColor = "#FF0000";
        #region Oxide Hooks

        protected override void LoadDefaultConfig() => pluginConfiguration = new AntiCodeRaidConfiguration();

        private void Init()
        {
            try
            {
                base.LoadConfig();
                permission.RegisterPermission( UsePermission, this );
                pluginConfiguration = Config.ReadObject<AntiCodeRaidConfiguration>();
            }
            catch ( Exception e )
            {
                pluginConfiguration = new AntiCodeRaidConfiguration();
                Puts( $"Failed to load plugin configuration, using default configuration: {e.Message} {e.StackTrace}" );
            }
        }

        private object CanUnlock( BasePlayer player, BaseLock baseLock )
        {
            if ( !permission.UserHasPermission( player.UserIDString, UsePermission ) )
                return null;

            if ( IsPlayerOwner( player, baseLock ) || IsPlayerOnTeam( player, baseLock ) || IsPlayerTCAuthed( player ) )
                return null;

            SendMessage( player, "CannotUnlock" );

            return false;
        }

        private bool IsPlayerOwner( BasePlayer player, BaseLock baseLock )
        {
            return player.userID == baseLock.GetParentEntity()?.OwnerID;
        }

        private bool IsPlayerOnTeam( BasePlayer player, BaseLock baseLock )
        {
            var lockEntity = baseLock.GetParentEntity();
            if ( pluginConfiguration.allowTeamMembers && null != lockEntity && null != player.Team )
                return player.Team.members.Contains( lockEntity.OwnerID );

            return false;
        }

        private bool IsPlayerTCAuthed( BasePlayer player )
        {
            return pluginConfiguration.allowTCAuthed && player.IsBuildingAuthed();
        }

        #endregion

        #region Localization

        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(
                new Dictionary<string, string>
                {
                    { "CannotUnlock", "You are not allowed to unlock this lock." }
                }, this, "en" );
        }

        private void SendMessage( BasePlayer player, string messageKey, params object[] args )
        {
            var prefix = String.Format( "[<color={0}>AntiCodeRaid</color>]", pluginConfiguration.pluginPrefixColor );
            var message = lang.GetMessage( messageKey, this, player.UserIDString );

            Player.Message( player, message, prefix, 0, args );
        }

        #endregion

        #region Configuration

        private class AntiCodeRaidConfiguration
        {
            [JsonProperty( "allowTeamMembers" )]
            public bool allowTeamMembers { get; private set; } = true;
            [JsonProperty( "allowTCAuthed" )]
            public bool allowTCAuthed { get; private set; } = true;
            [JsonProperty( "pluginPrefixColor" )]
            public string pluginPrefixColor { get; private set; } = DefaultPrefixColor;
        }

        #endregion
    }
}
