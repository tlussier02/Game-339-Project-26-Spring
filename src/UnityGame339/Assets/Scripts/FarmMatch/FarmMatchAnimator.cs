using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmMatchAnimator : MonoBehaviour
{
    private TMP_Text _scoreText;
    private TMP_Text _goalText;
    private TMP_Text _timerText;
    
    private Tilemap _tilemap;

    private void Awake()
    {
        ResolveReferences();
    }

    private void ResolveReferences()
    {
        if (_scoreText == null)
        {
            var target = GameObject.Find("SCORE - Text ");
            _scoreText = target != null ? target.GetComponent<TMP_Text>() : null;
        }

        if (_goalText == null)
        {
            var target = GameObject.Find("GOAL - Text ");
            _goalText = target != null ? target.GetComponent<TMP_Text>() : null;
        }

        if (_timerText == null)
        {
            var target = GameObject.Find("Time - Text");
            _timerText = target != null ? target.GetComponent<TMP_Text>() : null;
        }
        
        if (_tilemap == null)
            _tilemap = FindFirstObjectByType<Tilemap>();
    }

    public void PlayScoreIncreasedAnimation() => PlayTextPunchAnimation(_scoreText);
    public void PlayGoalIncreasedAnimation()  => PlayTextPunchAnimation(_goalText);
    
    public void PlayCropsReplacedAnimation(IReadOnlyList<Vector3> worldPositions)
    {
        foreach (var worldPosition in worldPositions)
        {
            var cellPosition = _tilemap.WorldToCell(worldPosition);
            var tile = _tilemap.GetTile<Tile>(cellPosition);

            if (tile == null) continue;

            var tempObject = new GameObject("CropAnim");
            tempObject.transform.position = worldPosition;
            var spriteRenderer = tempObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = tile.sprite;
            spriteRenderer.sortingOrder = 1;

            DOTween.Kill(tempObject.transform);

            tempObject.transform.DOScale(0f, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(tempObject));
        }
    }

    private void PlayTextPunchAnimation(TMP_Text text)
    {
        if (text == null) return;

        text.transform.DOPunchScale(
            punch: Vector3.one * 0.4f,
            duration: 0.4f,
            vibrato: 1,
            elasticity: 0.5f
        );
    }
}