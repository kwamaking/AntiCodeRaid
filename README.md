# Anti Code Raid
This plugin prevents players from code raiding items and doors locked with a code lock.  It is configurable to allow team members and anyone who is TC Authed to attempt to unlock the code lock.

# Permissions
* `anticoderaid.enabled` - Turns this plugin on for the group or players assigned.

# Configuration
* `allowTeamMembers` - If set to `true` allows members of your team to unlock code locks that you own.
* `allowTCAuthned` - If set to `true` allows anyone who has building privilege to unlock code locks.
* `pluginPrefixColor` - Allows you to change the plugin's chat prefix color, default is `#FF0000`, it must be Hex format.

```json
{
    "allowTeamMembers": true,  
    "allowTCAuthed": true,
    "pluginPrefixColor": "#FF0000"
}
```

# Localization
```json
{
  "CannotUnlock": "You are not allowed to unlock this lock."
}
```
