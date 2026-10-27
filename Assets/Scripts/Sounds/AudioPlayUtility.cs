using UnityEngine;

/// <summary>
/// Avoids Unity warnings when Play is invoked on inactive or disabled AudioSources.
/// </summary>
public static class AudioPlayUtility
{
    public static bool CanPlay(AudioSource source)
    {
        return source != null && source.enabled && source.gameObject.activeInHierarchy;
    }

    public static void Play(AudioSource source)
    {
        if (!CanPlay(source))
            return;

        source.Play();
    }

    public static void PlayOneShot(AudioSource source, AudioClip clip)
    {
        if (!CanPlay(source) || clip == null)
            return;

        source.PlayOneShot(clip);
    }

    public static void PlayDelayed(AudioSource source, float delay)
    {
        if (!CanPlay(source))
            return;

        source.PlayDelayed(delay);
    }
}
