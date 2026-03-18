namespace Game339.Shared.Diagnostics;

public sealed class EmptyGameLog : IGameLog
{
    public static EmptyGameLog Instance { get; } = new();

    private EmptyGameLog()
    {
    }

    public void Info(string message)
    {
    }

    public void Warn(string message)
    {
    }

    public void Error(string message)
    {
    }
}
