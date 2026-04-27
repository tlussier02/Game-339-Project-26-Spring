using Game339.Shared.Services.Implementation;
using NUnit.Framework;

namespace Game339.Tests;

public class TimerTests
{
    // imitating the dependency injection
    private FakeTimeProvider _fakeTimeProvider;
    private Timer _timer;

    [SetUp]

    public void SetUp()
    {
        _fakeTimeProvider = new FakeTimeProvider();
        _timer = new Timer(_fakeTimeProvider);
    }

    [Test]
    public void Timer_StartsAtCorrectValue()
    {
        _timer.Start(10f);
        Assert.AreEqual(10f, _timer.Current, 0.001f);
    }
    
    [Test]
    public void Timer_CountsDownCorrectly()
    {
        _fakeTimeProvider.DeltaTime = 1f;
        _timer.Start(10f);
        _timer.Tick();
        
        Assert.AreEqual(9f, _timer.Current, 0.001f);
    }

    [Test]
    public void Timer_StopsAtZero()
    {
        _fakeTimeProvider.DeltaTime = 3f;
        _timer.Start(3f);
        while (_timer.Current > 0f)
        {
            _timer.Tick();
        }

        _timer.Tick();
        Assert.AreEqual(0f, _timer.Current);
    }

    [Test]
    public void Timer_NoResponseAfterZero()
    {
        _fakeTimeProvider.DeltaTime = 1f;
        _timer.Start(1f);
        _timer.Tick();
        _timer.Tick();

        Assert.AreEqual(0f,_timer.Current, 0.001f);

    }
}