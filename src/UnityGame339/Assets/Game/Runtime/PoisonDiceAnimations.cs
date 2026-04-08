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
        _animatorPoison.SetBool("isRollingPoison", true);
        poisonDice.SetActive(true);
        
    }
    public void StopPRAnimation()
    {
        _animatorPoison.SetBool("isRollingPoison", false);
    }
    
    public void PlayRRAnimation()
    {
        _animatorRegular.SetBool("isRegularRolling", true);
        regularDice.SetActive(true);
    }
    public void StopRRAnimation()
    {
        _animatorRegular.SetBool("isRegularRolling", false);
    }
    
    public void ShowFireAnimation()
    {
        fireAnimation.SetActive(true);
    }
}
