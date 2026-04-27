namespace DefaultNamespace;

public class FakeTimeProvider : ITimeProvider
{
    public float DeltaTime { get; set; }
}