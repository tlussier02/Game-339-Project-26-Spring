using System;
using System.Collections;
using Game.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpriteBehavior : MonoBehaviour
{
    [Header("Dice")] 
    [SerializeField] Sprite[] diceRSprites;
    [SerializeField] Sprite[] dicePSprites;
    [SerializeField] Sprite newDiceFaceR;
    [SerializeField] Sprite newDiceFaceP;
    
    public GameObject poisonDice;
    public GameObject regularDice;

    private PoisonDiceGameModel _poisonDiceGameModel;
    
    public void RollRegularDice(int roll)
    {
        newDiceFaceR = diceRSprites[roll-1];
        regularDice.GetComponent<SpriteRenderer>().sprite = newDiceFaceR;
    }
    
    public void RollPoisonDice(int poisonValue)
    {
        newDiceFaceP = dicePSprites[poisonValue-1];
        poisonDice.GetComponent<SpriteRenderer>().sprite = newDiceFaceP;
    }
    
    //public void checkForBust()
    //{
        //int positionR = Array.IndexOf(regularSprites, DiceFaceR);
        //int positionP = Array.IndexOf(poisonSprites, DiceFaceP);

       // if (positionR == positionP)
        //{
            //end game//
        //}
    //}
}
