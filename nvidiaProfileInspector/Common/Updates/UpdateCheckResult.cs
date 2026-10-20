using L = nvidiaProfileInspector.Localization.Localizer;
namespace nvidiaProfileInspector.Common.Updates
{
    public sealed class UpdateCheckResult
    {
        private UpdateCheckResult(bool succeeded, bool isUpdateAvailable, UpdateRelease latestRelease, string statusMessage, string errorMessage)
        {
            Succeeded = succeeded;
            IsUpdateAvailable = isUpdateAvailable;
            LatestRelease = latestRelease;
            StatusMessage = statusMessage;
            ErrorMessage = errorMessage;
        }

        public bool Succeeded { get; }
        public bool IsUpdateAvailable { get; }
        public UpdateRelease LatestRelease { get; }
        public string StatusMessage { get; }
        public string ErrorMessage { get; }
        public bool CanInstall => IsUpdateAvailable && LatestRelease?.IsInstallable == true;

        public static UpdateCheckResult Available(UpdateRelease release)
        {
            var message = release?.IsInstallable == true
                ? L.Get("A_newer_version_is_ready_to_b2a0a3")
                : L.Get("A_newer_version_is_available_but_871bc1");
            return new UpdateCheckResult(true, true, release, message, null);
        }

        public static UpdateCheckResult UpToDate(UpdateRelease release)
        {
            return new UpdateCheckResult(true, false, release, L.Get("You_are_already_on_the_latest_ec9db9"), null);
        }

        public static UpdateCheckResult Unavailable(string message)
        {
            return new UpdateCheckResult(false, false, null, message, message);
        }
    }
}
