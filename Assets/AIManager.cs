using UnityEngine;
using OpenAI;
using OpenAI.Models;
using OpenAI.Chat;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meta.WitAi.Json;
using Meta.WitAi;
using PassthroughCameraSamples;
using TMPro;
using UnityEngine.UI;
using System.Globalization;
using Meta.WitAi.TTS.Utilities;

public class AIManager : MonoBehaviour
{
    // used to help us send a request to OpenAI
    public OpenAIConfiguration openAIConfiguration;
    public WakeWordManager wakeWordManager;
    public WebCamTextureManager webcamManager;
    public Texture2D debugPicture;

    public GameObject aiCanvas;
    public GameObject loadingCanvas;
    public TextMeshProUGUI aiResponseText;
    public RawImage describePicture;

    public TTSSpeaker ttsSpeaker;

    private OpenAIClient openAI;
    private Texture2D picture;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        openAI = new OpenAIClient(openAIConfiguration);
        wakeWordManager.OnResponseDetected.AddListener(HandleResponse);
        wakeWordManager.OnWakeWordDetected.AddListener(ResetUI);

        aiCanvas.SetActive(false);
        loadingCanvas.SetActive(false);
    }

    public void ResetUI()
    {
        aiCanvas.SetActive(false);
        loadingCanvas.SetActive(false);
    }

    public async void HandleResponse(WitResponseNode response)
    {
        string intent = response.GetIntentName();
        string result;

        loadingCanvas.SetActive(true);

        if (intent == "describe_vision")
        { 
            result = await DescribeChatGPT();
            Debug.Log("CHAT GPT RESPONSE : " + result);
            UpdateResultUI(result, picture);
        }
        else
        {
            result = await AskChatGPT(response.GetTranscription());
            Debug.Log("CHAT GPT RESPONSE : " + result);
            UpdateResultUI(result, null);
        }

        ttsSpeaker.Speak(result);
        
    }

    public void UpdateResultUI(string resultText, Texture2D resultTexture)
    {
        loadingCanvas.SetActive(false);
        aiCanvas.SetActive(true);

        aiResponseText.text = resultText;
        if (resultTexture)
        {
            describePicture.enabled = true;
            describePicture.texture = resultTexture;
        }
        else
        {
            describePicture.enabled = false;
        }
    }

    public async Task<string> AskChatGPT(string input)
    {
        var messages = new List<Message>
        {
            // tells the AI what type of role it should be playing, in this
            // case it is to be a helpful assistant, like Siri
            new Message(Role.System, "You are a helpful voice assistant"),
            // this is what we'll be asking it
            new Message(Role.User, input)
        };
        // contains the request and the specific model we want to run the request on
        var request = new ChatRequest(messages, model: Model.GPT4o);
        var result = await openAI.ChatEndpoint.GetCompletionAsync(request);

        return result.FirstChoice.Message.ToString();
    }

    public async Task<string> DescribeChatGPT()
    {
        // added specifically because Unity Editor doesn't allow passthrough
        // video, only works when built, so this can be used for in-editor testing
        if (Application.isEditor)
        {
            picture = debugPicture;
        }
        else
        {
            int width = webcamManager.WebCamTexture.width;
            int height = webcamManager.WebCamTexture.height;

            if (picture == null || picture.width != width || picture.height != height)
            {
                picture = new Texture2D(width, height, TextureFormat.RGB24, false);
            }

            Color32[] pixels = new Color32[width * height];
            webcamManager.WebCamTexture.GetPixels32(pixels);

            Debug.Log($"Webcam active: {webcamManager.WebCamTexture != null}");
            Debug.Log($"Webcam playing: {webcamManager.WebCamTexture?.isPlaying}");
            Debug.Log($"Webcam dimensions: {width}x{height}");


            picture.SetPixels32(pixels);
            picture.Apply();
        }

        // taken from AskChatGPT(), modified for this method
        var messages = new List<Message>
        {
            new Message(Role.System, "Describe the image in maximum 20 words."),
            new Message(Role.User, new List<Content>
            {
                "What's in this image ? ", picture
            })
        };
        var request = new ChatRequest(messages, model: Model.GPT4o);
        var result = await openAI.ChatEndpoint.GetCompletionAsync(request);

        return result.FirstChoice.Message.ToString();
    }
}
