using DG.Tweening;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public float forceJump;
    private PlayerSpeed _player;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerSpeed>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _player.Jump(forceJump);
        transform.DOScale(.5f, 1f).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo);
    }
}
