using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpriteBehavior : MonoBehaviour
{
    [Header("Dice")] 
    [SerializeField] Sprite[] regularSprites;
    [SerializeField] Sprite[] poisonSprites;
    [SerializeField] Sprite newDiceFaceR;
    [SerializeField] Sprite newDiceFaceP;
    
    private PoisonDiceAnimations _poisonDiceAnimations;
    
    public void RollRegularDice()
    {
        _poisonDiceAnimations.PlayRRAnimation();
        WaitToChangeRegular();
        //trying to put a delay on the sprite change, so that it changes in the middle of the animation
    }

    private void WaitToChangeRegular()
    {
        StartCoroutine(ChangeAfterRegularStart(2.5f));
    }

    private IEnumerator ChangeAfterRegularStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        newDiceFaceR = regularSprites[Random.Range(0, regularSprites.Length)];
        gameObject.GetComponent<SpriteRenderer>().sprite = newDiceFaceR;
        _poisonDiceAnimations.StopRRAnimation();
    }
    
    public void RollPoisonDice()
    {
        _poisonDiceAnimations.PlayPRAnimation();
        WaitToChangePoison();
        //trying to put a delay on the sprite change, so that it changes in the middle of the animation
    }
    
    private void WaitToChangePoison()
    {
        StartCoroutine(ChangeAfterPoisonStart(2.5f));
    }

    private IEnumerator ChangeAfterPoisonStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        newDiceFaceP = poisonSprites[Random.Range(0, poisonSprites.Length)];
        gameObject.GetComponent<SpriteRenderer>().sprite = newDiceFaceP;
        _poisonDiceAnimations.StopPRAnimation();
    }

    public void checkForBust()
    {
        int positionR = Array.IndexOf(regularSprites, newDiceFaceR);
        int positionP = Array.IndexOf(poisonSprites, newDiceFaceP);

        if (positionR == positionP)
        {
            //end game//
        }
    }
}
