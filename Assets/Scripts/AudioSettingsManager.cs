using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;        // BGM
    public AudioSource[] sfxSources;       // Semua SFX

    [Header("UI Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("UI Icons")]
    public Image musicIcon;
    public Image sfxIcon;

    [Header("Sprites")]
    public Sprite audioOnSprite;
    public Sprite audioOffSprite;

    bool musicOn;
    bool sfxOn;

    void Start()
    {
        // Load data
        musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        sfxOn = PlayerPrefs.GetInt("SfxOn", 1) == 1;

        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SfxVolume", 1f);

        // Set slider
        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;

        ApplyMusic(musicVol);
        ApplySFX(sfxVol);
        UpdateIcons();
    }

    // ================= MUSIC =================
    public void ToggleMusic()
    {
        musicOn = !musicOn;
        PlayerPrefs.SetInt("MusicOn", musicOn ? 1 : 0);

        ApplyMusic(musicSlider.value);
        UpdateIcons();
    }

    public void OnMusicSlider(float value)
    {
        musicOn = value > 0;
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.SetInt("MusicOn", musicOn ? 1 : 0);

        ApplyMusic(value);
        UpdateIcons();
    }

    void ApplyMusic(float volume)
    {
        if (musicSource == null) return;

        musicSource.mute = !musicOn;
        musicSource.volume = musicOn ? volume : 0f;
    }

    // ================= SFX =================
    public void ToggleSFX()
    {
        sfxOn = !sfxOn;
        PlayerPrefs.SetInt("SfxOn", sfxOn ? 1 : 0);

        ApplySFX(sfxSlider.value);
        UpdateIcons();
    }

    public void OnSfxSlider(float value)
    {
        sfxOn = value > 0;
        PlayerPrefs.SetFloat("SfxVolume", value);
        PlayerPrefs.SetInt("SfxOn", sfxOn ? 1 : 0);

        ApplySFX(value);
        UpdateIcons();
    }

    void ApplySFX(float volume)
    {
        foreach (AudioSource sfx in sfxSources)
        {
            if (sfx == null) continue;

            sfx.mute = !sfxOn;
            sfx.volume = sfxOn ? volume : 0f;
        }
    }

    // ================= ICON =================
    void UpdateIcons()
    {
        if (musicIcon != null)
            musicIcon.sprite = musicOn ? audioOnSprite : audioOffSprite;

        if (sfxIcon != null)
            sfxIcon.sprite = sfxOn ? audioOnSprite : audioOffSprite;
    }

    // ================= DEBUG/RESET =================
    /// <summary>
    /// Reset semua audio settings ke default (musik ON, volume 100%).
    /// </summary>
    [ContextMenu("Reset Audio Settings")]
    public void ResetAudioSettings()
    {
        PlayerPrefs.SetInt("MusicOn", 1);
        PlayerPrefs.SetInt("SfxOn", 1);
        PlayerPrefs.SetFloat("MusicVolume", 1f);
        PlayerPrefs.SetFloat("SfxVolume", 1f);
        PlayerPrefs.Save();

        musicOn = true;
        sfxOn = true;

        if (musicSlider != null) musicSlider.value = 1f;
        if (sfxSlider != null) sfxSlider.value = 1f;

        ApplyMusic(1f);
        ApplySFX(1f);
        UpdateIcons();

        Debug.Log("Audio settings reset! Music ON, Volume 100%");
    }

    /// <summary>
    /// Force play musik sekarang.
    /// </summary>
    [ContextMenu("Force Play Music")]
    public void ForcePlayMusic()
    {
        if (musicSource != null)
        {
            musicSource.mute = false;
            musicSource.volume = 1f;
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
            Debug.Log("Music forced to play!");
        }
        else
        {
            Debug.LogError("Music Source belum di-assign!");
        }
    }
}

