using UnityEngine;
using Oculus.Voice;
using Meta.WitAi.Json;
using Meta.WitAi;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class WakeWordManager : MonoBehaviour
{
    public AppVoiceExperience appVoice;
    public GameObject voiceUI;
    public Image micVolume;
    public float micVolumeMultiplier = 5;
    public TextMeshProUGUI transcriptionText;

    public UnityEvent OnWakeWordDetected;
    public UnityEvent<WitResponseNode> OnResponseDetected;

    private bool voiceActivated = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        appVoice.VoiceEvents.OnResponse.AddListener(HandleResponse);

        appVoice.VoiceEvents.OnMicAudioLevelChanged.AddListener(UpdateVolumeUI);
        appVoice.VoiceEvents.OnPartialTranscription.AddListener(UpdateTranscriptionUI);

        voiceUI.SetActive(false);
        appVoice.Activate();
    }

    /**
     * Updates the text with the user's speech
     */
    public void UpdateTranscriptionUI(string transcriptionValue)
    {
        if (voiceActivated) 
        {
            transcriptionText.text = transcriptionValue;
        }
    }

    /**
     * Have the mic get a red filled appearance depending on the volume
     * of the user's speech
     */
    public void UpdateVolumeUI(float micValue)
    {
        if (voiceActivated)
        {
            micVolume.fillAmount = Mathf.Clamp01(micValue * micVolumeMultiplier);
        }
    }

   public void HandleResponse(WitResponseNode witResponse)
    {
        Debug.Log(witResponse.GetTranscription() + "-" + witResponse.GetIntentName());

        if (voiceActivated)
        {
            Debug.Log("Voice Activated : ");
            voiceActivated = false;
            voiceUI.SetActive(false);
            OnResponseDetected.Invoke(witResponse);
        }
        // user is trying to wake the ai
        else
        {
            if (witResponse.GetIntentName() == "wake_word")
            {
                voiceActivated = true;
                Debug.Log("Wake Word detected");
                voiceUI.SetActive(true);
                transcriptionText.text = "";
                OnWakeWordDetected.Invoke();
            }
        }

        // ensures voice recognition is always activated
        appVoice.Activate();
    }
}
