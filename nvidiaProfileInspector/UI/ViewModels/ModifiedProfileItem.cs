using L = nvidiaProfileInspector.Localization.Localizer;
namespace nvidiaProfileInspector.UI.ViewModels
{
    public class ModifiedProfileItem
    {
        public ModifiedProfileItem(string profileName, bool isUserDefined)
        {
            ProfileName = profileName;
            IsUserDefined = isUserDefined;
        }

        public string ProfileName { get; }

        public bool IsUserDefined { get; }

        public string ProfileTypeLabel => IsUserDefined ? L.Get("User_created_profile_fb1707") : L.Get("NVIDIA_predefined_profile_1052aa");

        public override string ToString()
        {
            return ProfileName;
        }
    }
}
