using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace ArtlessUI.Ui.SubPanels
{
    public class SoundsOptionsPanel : SubPanel
    {
        [Header("Ui")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider soundsSlider;

        [Header("Audio params name")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string musicVolumeParameterName = "Music";
        [SerializeField] private string soundVolumeParamterName = "Sounds";
        [SerializeField] private int musicDefaultValue = 60;
        [SerializeField] private int soundsDeaultValue = 80;

        public override void OnActivation()
        {
            musicSlider.onValueChanged.AddListener(MusicLevelChanged);
            soundsSlider.onValueChanged.AddListener(SoundsLevelChanged);

            musicSlider.value = PlayerPrefs.GetFloat(musicVolumeParameterName, musicDefaultValue);
            soundsSlider.value = PlayerPrefs.GetFloat(soundVolumeParamterName, soundsDeaultValue);
        }

        private void MusicLevelChanged(float value)
        {
            if (value == 0)
                audioMixer.SetFloat(musicVolumeParameterName, 20f * Mathf.Log(0.001f));
            else
                audioMixer.SetFloat(musicVolumeParameterName, 20f * Mathf.Log(value / 100f));
            PlayerPrefs.SetFloat(musicVolumeParameterName, value);
            PlayerPrefs.Save();
        }

        private void SoundsLevelChanged(float value)
        {
            if (value == 0)
                audioMixer.SetFloat(soundVolumeParamterName, 20f * Mathf.Log(0.001f));
            else
                audioMixer.SetFloat(soundVolumeParamterName, 20f * Mathf.Log(value / 100f));
            PlayerPrefs.SetFloat(soundVolumeParamterName, value);
            PlayerPrefs.Save();
        }

        public override void OnInactivation()
        {
            musicSlider.onValueChanged.RemoveListener(MusicLevelChanged);
            soundsSlider.onValueChanged.RemoveListener(SoundsLevelChanged);
        }
    }
}