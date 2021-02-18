using System;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info( "Anti Code Raid", "kwamaking", "1.0.0" )]
    [Description( "Prevents players from code raiding if they're not on your team, TC authed, or the owner." )]
    class AntiCodeRaid : RustPlugin
    {
        private AntiCodeRaidConfiguration pluginConfiguration { get; set; }
        private const string UsePermission = "anticoderaid.enabled";

        #region Oxide Hooks

        protected override void LoadDefaultConfig() => pluginConfiguration = new AntiCodeRaidConfiguration();

        private void Loaded()
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

            if ( player.userID == baseLock.GetParentEntity()?.OwnerID )
                return null;

            if ( pluginConfiguration.allowTeamMembers && null != player.Team && player.Team.members.Contains( baseLock.GetParentEntity().OwnerID ) )
                return null;

            if ( pluginConfiguration.allowTCAuthed && player.IsBuildingAuthed() )
                return null;

            SendMessage( player, "CannotUnlock" );

            return false;
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
            var prefix = "[<color=red>AntiCodeRaid</color>] ";
            var message = lang.GetMessage( messageKey, this, player.UserIDString );

            Player.Message( player, message, prefix, 0, args );
        }

        #endregion

        #region Configuration

        private class AntiCodeRaidConfiguration
        {
            public bool allowTeamMembers { get; set; } = true;
            public bool allowTCAuthed { get; set; } = true;
        }

        #endregion
    }
}