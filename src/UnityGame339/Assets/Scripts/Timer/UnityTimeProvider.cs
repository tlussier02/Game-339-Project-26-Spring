using UnityEngine;
using Game339.Shared.Services;

namespace Game
{
    public class UnityTimeProvider: ITimeProvider
    {
        public float DeltaTime => Time.deltaTime;
    }
}