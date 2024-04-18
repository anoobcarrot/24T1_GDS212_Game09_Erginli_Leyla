using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public Slider volumeSlider;

    public AudioSource[] allAudioSources; // Assign the audio sources in the inspector

    private void Start()
    {
        Time.timeScale = 1;
        // Set the volume slider value based on PlayerPrefs
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        volumeSlider.value = savedVolume;
    }

    public void AdjustVolume()
    {
        float volume = volumeSlider.value;
        Debug.Log("Volume Adjusted: " + volume);

        // Adjust volume for all assigned audio sources
        foreach (var audioSource in allAudioSources)
        {
            audioSource.volume = volume;
        }

        // Save volume
        PlayerPrefs.SetFloat("Volume", volume);
    }
}
