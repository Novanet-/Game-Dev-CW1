using JetBrains.Annotations;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    #region Private Fields

    private static AudioController _audioController;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _coinSound;
    [SerializeField] private AudioClip _moveSound;

    #endregion Private Fields

    #region Public Properties

    [NotNull] public AudioClip CoinSound
    {
        get { return _coinSound; }
        private set { _coinSound = value; }
    }

    [NotNull] public AudioClip MoveSound
    {
        get { return _moveSound; }
        private set { _moveSound = value; }
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    /// Gets the UI controller.
    /// </summary>
    /// <returns></returns>
    public static AudioController GetAudioController()
    {
        return _audioController;
    }

    /// <summary>
    /// Plays the sound once.
    /// </summary>
    /// <param name="sound">The sound.</param>
    public void PlaySoundOnce(AudioClip sound)
    {
        _audioSource.PlayOneShot(sound);
    }

    /// <summary>
    /// Plays the sound once.
    /// </summary>
    /// <param name="sound">The sound.</param>
    /// <param name="volumeScale">The volume scale.</param>
    public void PlaySoundOnce(AudioClip sound, float volumeScale)
    {
        _audioSource.PlayOneShot(sound, volumeScale);
    }

    #endregion Public Methods

    #region Private Methods

    private void Awake()
    {
        _audioController = this;
    }

    #endregion Private Methods
}