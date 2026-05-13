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
    
    private readonly HashSet<Vector3Int> _animatingCells = new HashSet<Vector3Int>();

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
            var newTile = _tilemap.GetTile<Tile>(cellPosition);
            _tilemap.SetTile(cellPosition, null);
            _animatingCells.Add(cellPosition);

            // old sprite
            var tempObject = new GameObject("CropAnim");
            tempObject.transform.position = worldPosition;
            tempObject.AddComponent<SpriteRenderer>().sprite = oldSprite;
            tempObject.GetComponent<SpriteRenderer>().sortingOrder = 1;

            // new sprite
            var newTempObject = new GameObject("CropAnimSpawn");
            newTempObject.transform.position = worldPosition;
            newTempObject.transform.localScale = Vector3.zero;
            newTempObject.AddComponent<SpriteRenderer>().sprite = newTile.sprite;
            newTempObject.GetComponent<SpriteRenderer>().sortingOrder = 1;

            DOTween.Sequence()
                .SetId("cropAnim")
                .Append(tempObject.transform.DOScale(0f, 0.4f).SetEase(Ease.InBack))
                .AppendCallback(() => Destroy(tempObject))
                .Append(newTempObject.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack))
                .AppendCallback(() =>
                {
                    Destroy(newTempObject);
                    _tilemap.SetTile(cellPosition, newTile);
                    _animatingCells.Remove(cellPosition);
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
    
    public void StopAllCropAnimations()
    {
        DOTween.Kill("cropAnim"); // ← only kills crop sequences!

        foreach (var obj in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (obj.name == "CropAnim" || obj.name == "CropAnimSpawn")
                Destroy(obj);
        }

        _animatingCells.Clear();
    }
}