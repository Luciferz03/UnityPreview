using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip footstepSound;
    public AudioClip jumpSound;
    public AudioClip landingSound;
    public AudioClip crouchSound;

    private void Awake()
    {
        // Ensure there's only one instance of AudioManager
        if (FindObjectsOfType<AudioManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip);
    }
}
