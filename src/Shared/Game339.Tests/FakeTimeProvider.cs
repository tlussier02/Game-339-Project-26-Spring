using Game339.Shared.Services;
namespace Game339.Tests;

public class FakeTimeProvider : ITimeProvider
{
    public float DeltaTime { get; set; }
}