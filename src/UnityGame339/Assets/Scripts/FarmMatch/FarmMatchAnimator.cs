using DG.Tweening;
using TMPro;
using UnityEngine;

public class FarmMatchAnimator : MonoBehaviour
{
    private TMP_Text _scoreText;
    private TMP_Text _goalText;
    private TMP_Text _timerText;

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
    }

    public void PlayScoreIncreasedAnimation() => PlayTextPunchAnimation(_scoreText);
    public void PlayGoalIncreasedAnimation()  => PlayTextPunchAnimation(_goalText);
    public void PlayTimerAnimation()          => PlayTextPunchAnimation(_timerText);

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