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
    
    public void PlayCropsReplacedAnimation(IReadOnlyList<(Vector3 worldPos, Sprite oldSprite)> animData)
    {
        foreach (var (worldPosition, oldSprite) in animData)
        {
            if (oldSprite == null) continue;

            var cellPosition = _tilemap.WorldToCell(worldPosition);

            _tilemap.SetTileFlags(cellPosition, TileFlags.None);
            _tilemap.SetColor(cellPosition, Color.clear);

            var tempObject = new GameObject("CropAnim");
            tempObject.transform.position = worldPosition;
            var spriteRenderer = tempObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = oldSprite; // ← old sprite shrinks out!
            spriteRenderer.sortingOrder = 1;

            DOTween.Kill(tempObject.transform);

            tempObject.transform.DOScale(0f, 0.4f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    _tilemap.SetTileFlags(cellPosition, TileFlags.None);
                    _tilemap.SetColor(cellPosition, Color.clear);
                    Destroy(tempObject);

                    var newTile = _tilemap.GetTile<Tile>(cellPosition);
                    if (newTile == null) return;

                    var newTempObject = new GameObject("CropAnimSpawn");
                    newTempObject.transform.position = worldPosition;
                    newTempObject.transform.localScale = Vector3.zero;
                    var newSpriteRenderer = newTempObject.AddComponent<SpriteRenderer>();
                    newSpriteRenderer.sprite = newTile.sprite; // ← new sprite grows in!
                    newSpriteRenderer.sortingOrder = 1;

                    newTempObject.transform.DOScale(1f, 0.4f)
                        .SetEase(Ease.OutBack)
                        .OnComplete(() =>
                        {
                            Destroy(newTempObject);
                            _tilemap.SetColor(cellPosition, Color.white);
                        });
                });
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