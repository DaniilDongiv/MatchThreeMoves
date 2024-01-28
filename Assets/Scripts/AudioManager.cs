using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _itemDestroyingClip;

    public void PlayItemDestroyingClip()
    {
        _audioSource.clip = _itemDestroyingClip;
        _audioSource.Play();
    }
}