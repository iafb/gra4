using System.ComponentModel;
using GRA.Domain.Model;

namespace GRA
{
    public struct SiteSettingKey
    {
        [DisplayName("Secret Code")]
        public struct SecretCode
        {
            // TODO make this truly disable secret codes for the site
            public static SiteSetting Disable = new SiteSetting()
            {
                Name = "Disable",
                Format = SiteSettingFormat.Boolean,
                Key = "SecretCode.Disable",
                Info = "Currently: if this is set (i.e. not null), hide the secret code entry"
            };
        }

        public struct Challenges
        {
            public static SiteSetting HideUntilRegistrationOpen = new SiteSetting()
            {
                Name = "Hide until registration opens",
                Format = SiteSettingFormat.Boolean,
                Key = "Challenges.HideUntilRegistrationOpen",
                Info = "If this is set (i.e. not null) hide challenges until the registration " +
                    "period is open"
            };
        }

        public struct Events
        {
            public static SiteSetting HideUntilRegistrationOpen = new SiteSetting()
            {
                Name = "Hide until registration opens",
                Format = SiteSettingFormat.Boolean,
                Key = "Events.HideUntilRegistrationOpen",
                Info = "If this is set (i.e. not null) hide events until the registration period " +
                "is open"
            };

            public static SiteSetting RequireBadge = new SiteSetting()
            {
                Name = "Require badges",
                Format = SiteSettingFormat.Boolean,
                Key = "Events.RequireBadge",
                Info = "If this is set (i.e. not null) require all events to be created with a " +
                    "badge. With this set anyone who has the ManageEvents permission will need " +
                    "the ManageTriggers permission as well."
            };
        }

        public struct Points
        {
            public static SiteSetting MaximumPermitted = new SiteSetting()
            {
                Name = "Maximum points",
                Format = SiteSettingFormat.Integer,
                Key = "Points.MaximumPermitted",
                Info = "If this is set to an integer, it represents the maximum amount of points " +
                    "permitted for a participant to have through regular means (it may be able " +
                    "to be overidden from Mission Control)."
            };
        }

        public struct Users
        {
            public static SiteSetting RestrictChangingSystemBranch = new SiteSetting()
            {
                Name = "Restrict changing system and branch",
                Format = SiteSettingFormat.Boolean,
                Key = "Users.RestrictChangingSystemBranch",
                Info = "If this is set (i.e. not null) do not allow users to change their " +
                    "system/branch after joining"
            };

            public static SiteSetting MaximumHouseholdSizeBeforeGroup = new SiteSetting()
            {
                Name = "Maximum household size before group",
                Format = SiteSettingFormat.Integer,
                Key = "Users.MaximumHouseholdSizeBeforeGroup",
                Info = "If this is set to an integer, when the household count exceeds this " +
                    "number the household head will be forced to enter a group name and select " +
                    "a group type."
            };

            public static SiteSetting AskIfFirstTime = new SiteSetting()
            {
                Name = "Ask if first time",
                Format = SiteSettingFormat.Boolean,
                Key = "Users.AskIfFirstTime",
                Info = " If this is set (i.e. not null) do not allow users to sign up without " +
                    "selecting if it's their first time in the program"
            };

            public static SiteSetting DefaultDailyPersonalGoal = new SiteSetting()
            {
                Name = "Default daily personal goal",
                Format = SiteSettingFormat.Integer,
                Key = "Users.DefaultDailyPersonalGoal",
                Info = "If this is set to an integer ask users on sign up for a daily personal " +
                    "activity goal with this value being the default option. Requires the site " +
                    "to have dates set for ProgramStarst and ProgramEnds."
            };
        }
    }
}
