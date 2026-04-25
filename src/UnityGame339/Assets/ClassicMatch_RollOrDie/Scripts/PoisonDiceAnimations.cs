using Game.Runtime;
using UnityEngine;

public class PoisonDiceAnimations : MonoBehaviour
{
    [SerializeField] private Animator _animatorRegular;
    [SerializeField] private Animator _animatorPoison;

    public GameObject fireAnimation;
    public GameObject regularDice;
    public GameObject poisonDice;

    private void Awake()
    {
        _animatorRegular = GetComponent<Animator>();
        _animatorPoison = GetComponent<Animator>();
    }

    public void PlayPRAnimation()
    {
        poisonDice.SetActive(true);
        _animatorPoison.SetBool("isRollingPoison", true);
        // poison dice status set to false at start of game
        // when player hits start, have this animation play
        
    }
    public void StopPRAnimation()
    {
        _animatorPoison.SetBool("isRollingPoison", false);
    }
    
    public void PlayRRAnimation()
    {
        regularDice.SetActive(true);
        _animatorRegular.SetBool("isRegularRolling", true);
        // regular dice status set to false at start of game
        // when player hits roll, have this animation play
    }
    public void StopRRAnimation()
    {
        _animatorRegular.SetBool("isRegularRolling", false);
    }
    
    public void ShowFireAnimation()
    {
        fireAnimation.SetActive(true);
        // fire panel status set to false at start of game
        // when player reaches a new highscore, while actively playing the game, have this animation play
    }
}
