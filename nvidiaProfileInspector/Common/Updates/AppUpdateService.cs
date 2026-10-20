using L = nvidiaProfileInspector.Localization.Localizer;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace nvidiaProfileInspector.Common.Updates
{
    public sealed class AppUpdateService
    {
        private readonly GitHubReleaseClient _releaseClient;
        private readonly IUpdateInstaller _installer;

        public AppUpdateService()
            : this(new GitHubReleaseClient(GetCurrentVersion()), new InplaceUpdateInstaller())
        {
        }

        public AppUpdateService(GitHubReleaseClient releaseClient, IUpdateInstaller installer)
        {
            _releaseClient = releaseClient;
            _installer = installer;
        }

        public async Task<UpdateCheckResult> CheckAsync(UpdateChannel channel)
        {
            try
            {
                var releases = await _releaseClient.GetReleasesAsync();
                var latestRelease = releases
                    .Where(release => channel == UpdateChannel.Prerelease || !release.IsPreRelease)
                    .OrderByDescending(release => release.Version)
                    .FirstOrDefault();

                if (latestRelease == null)
                    return UpdateCheckResult.Unavailable(L.Get("No_release_information_is_available_229f04"));

                return latestRelease.Version > GetCurrentVersion()
                    ? UpdateCheckResult.Available(latestRelease)
                    : UpdateCheckResult.UpToDate(latestRelease);
            }
            catch
            {
                return UpdateCheckResult.Unavailable(L.Get("Could_not_read_release_information_79f9a4"));
            }
        }

        public Task PrepareInstallAsync(UpdateRelease release)
        {
            if (release == null || !release.IsInstallable)
                throw new InvalidOperationException(L.Get("The_selected_release_does_not_contain_ba3d9c"));

            return _installer.PrepareAndRunAsync(release);
        }

        public static Version GetCurrentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
