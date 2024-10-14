using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Piper;
public class GameManager : MonoBehaviour
{

    [SerializeField] private State currnetState = State.Speak;



    #region Services Modules
    [SerializeField] private AudioManager audioManager;
    private TextDataProcessor textDataProcessor;
    [SerializeField] private PiperManager piperManager;
    #endregion





    void Start()
    {
        try
        {
            ServicesInitialization();

            // Data population
            string filePath = Application.streamingAssetsPath + "/data.txt";

            if (textDataProcessor != null)
                textDataProcessor.ReadFromPath(filePath);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void ServicesInitialization()
    {
        textDataProcessor = new TextDataProcessor();
        textDataProcessor.OnReadyComplete += OnDataRead;

    }


    [ContextMenu(nameof(OnDataRead))]
    private void OnDataRead(List<string> data)
    {
        string word = textDataProcessor.getRandomWord();

        Debug.Log($"Selected Word {word}");
        Debug.Log($"Data word count {data.Count}");


        OnTextWordPlay(word);


    }

    private async void OnTextWordPlay(string text)
    {
        var audio = await piperManager.TextToSpeech(text);
        audioManager.PlayAudioClip(audio);
    }



    void Update()
    {
        
    }


    public State getCurrentState() => currnetState;

    private void setState(State state)
    {
        currnetState = state;
    }


}


public enum State 
{
    Speak,
    Start,
    Hint,
    Submitted,
    Loose,
    Win

}
