
namespace TimeLine
{
    public static class GetService
    {
        public static SoundPlayer SoundPlayer { get; } = new SoundPlayer();
        public static ToastManager ToastManager { get; } = new ToastManager();
        public static Manager Manager { get; } = new Manager();
    }
}
