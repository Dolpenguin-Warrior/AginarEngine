using OpenToolkit.Windowing.Desktop;

namespace Aginar
{
    using Aginar.Core;
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game("Aginar", 800, 600, 60, 60, GameWindowSettings.Default, NativeWindowSettings.Default))
            {
                game.Run();
            }
        }
    }
}
