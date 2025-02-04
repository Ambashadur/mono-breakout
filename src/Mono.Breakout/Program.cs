namespace Mono.Breakout;

internal class Program
{
    private static void Main(string[] args) {
        using var game = new GameExec();
        game.Run();
    }
}