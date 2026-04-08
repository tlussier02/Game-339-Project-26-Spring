using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpriteBehavior : MonoBehaviour
{
    [Header("Dice")] 
    [SerializeField] Sprite[] regularSprites;
    [SerializeField] Sprite[] poisonSprites;
    [SerializeField] Sprite newDiceFaceR;
    [SerializeField] Sprite newDiceFaceP;
    
    public void RollRegularDice()
    {
        newDiceFaceR = regularSprites[Random.Range(0, regularSprites.Length)];
        gameObject.GetComponent<SpriteRenderer>().sprite = newDiceFaceR;
    }
    
    public void RollPoisonDice()
    {
        newDiceFaceP = poisonSprites[Random.Range(0, poisonSprites.Length)];
        
        gameObject.GetComponent<SpriteRenderer>().sprite = newDiceFaceP;
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
