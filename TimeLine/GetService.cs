using TimeLine.Core;
using TimeLine.Views;

namespace TimeLine
{
    public static class GetService
    {
        public static Manager Manager { get; } = new Manager();
        public static ToastManager ToastManager { get; } = new ToastManager();
        public static SoundPlayer SoundPlayer { get; } = new SoundPlayer();
    }
}
