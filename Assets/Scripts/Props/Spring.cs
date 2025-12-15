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
        if (other.transform.CompareTag("Player"))
        {
            _player.Jump(forceJump);
            transform.DOScale(2f, .5f).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo);
        }
    }
}
