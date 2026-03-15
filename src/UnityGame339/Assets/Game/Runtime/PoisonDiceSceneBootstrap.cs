using UnityEngine;

namespace Game.Runtime
{
    public class PoisonDiceSceneBootstrap : MonoBehaviour
    {
        [SerializeField] private PoisonDiceGameController controller;
        [SerializeField] private PoisonDiceHudView hudView;
        [SerializeField] private PoisonDiceResultsView resultsView;

        private void Awake()
        {
            // Intentionally left as a safe stub until the scene is rewired.
            // Keeping the script active in the project makes the skeleton visible in Unity
            // without introducing broken references or partial runtime wiring.
            _ = controller;
            _ = hudView;
            _ = resultsView;
        }
    }
}
