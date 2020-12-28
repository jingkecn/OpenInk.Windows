using Windows.Networking.BackgroundTransfer;

namespace MyScript.OpenInk.Core.Infrastructure.Extensions
{
    public static class EnumExtensions
    {
        public static bool IsOnError(this BackgroundTransferStatus source)
        {
            return source == BackgroundTransferStatus.Error ||
                   source == BackgroundTransferStatus.PausedRecoverableWebErrorStatus;
        }

        public static bool IsPaused(this BackgroundTransferStatus source, bool byApplicationOnly = false)
        {
            return byApplicationOnly
                ? source == BackgroundTransferStatus.PausedByApplication
                : source == BackgroundTransferStatus.PausedByApplication ||
                  source == BackgroundTransferStatus.PausedCostedNetwork ||
                  source == BackgroundTransferStatus.PausedNoNetwork ||
                  source == BackgroundTransferStatus.PausedRecoverableWebErrorStatus ||
                  source == BackgroundTransferStatus.PausedSystemPolicy;
        }


        public static bool IsRunning(this BackgroundTransferStatus source)
        {
            return source == BackgroundTransferStatus.Running;
        }

        public static bool IsTerminated(this BackgroundTransferStatus source)
        {
            return source == BackgroundTransferStatus.Idle ||
                   source == BackgroundTransferStatus.Completed ||
                   source == BackgroundTransferStatus.Canceled ||
                   source == BackgroundTransferStatus.Error;
        }
    }
}
