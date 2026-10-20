using L = nvidiaProfileInspector.Localization.Localizer;
namespace nvidiaProfileInspector.UI.ViewModels
{
    using nvidiaProfileInspector.Common;

    public class ProfileListItem
    {
        public ProfileListItem(string profileName, bool isUserDefined)
        {
            ProfileName = profileName;
            IsGlobal = profileName == DrsSettingsService.GlobalProfileName;
            IsUserDefined = !IsGlobal && isUserDefined;
        }

        public string ProfileName { get; }

        public bool IsGlobal { get; }

        public bool IsUserDefined { get; }

        public string ProfileTypeLabel =>
            IsGlobal ? L.Get("Global_driver_profile_4b0e2a") :
            IsUserDefined ? L.Get("User_created_profile_fb1707") :
            L.Get("NVIDIA_predefined_profile_1052aa");

        public override string ToString()
        {
            return ProfileName;
        }
    }
}
