namespace Game339.Shared.Services; 
public class Timer
{ 
    private ITimeProvider _timeProvider;
    public float Current { get; private set; }

    public Timer(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }
    public void Start(float duration)
    {
        Current = duration;
    }

    public void Tick()
    {
        if(Current > 0)
            Current -= _timeProvider.DeltaTime;
        if (Current < 0)
            Current = 0;
    }
}