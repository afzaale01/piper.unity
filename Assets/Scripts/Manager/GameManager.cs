using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Piper;
public class GameManager : MonoBehaviour
{

    [SerializeField] private State currnetState = State.Start;

    #region Services Modules
    [SerializeField] private AudioManager audioManager;
    private TextDataProcessor textDataProcessor;
    [SerializeField] private PiperManager piperManager;

    private string currentSelectedWord;
    #endregion
    #region Public Action
    public Action OnStart;
    public Action OnSubmission;
    public Action OnWin;
    public Action OnLoose;
    public Action<string> OnShowHint;
    #endregion

    #region DataConfigurations
    [SerializeField] private bool loadMisspelledWordsOnly;
    #endregion

    private int AttemptCount;



    void Start()
    {
        try
        {
            ServicesInitialization();
            // Data population            
            if (textDataProcessor != null)
            {
                textDataProcessor.ReadDataList();
                textDataProcessor.ReadMisspelledList();
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void ServicesInitialization()
    {
        textDataProcessor = new TextDataProcessor();

    }

    public void UpdateDataSelection(bool setData) 
    {
        loadMisspelledWordsOnly = setData;
    }

    public void StartPlay()
    {
        currentSelectedWord = textDataProcessor.getNextWord(loadMisspelledWordsOnly);
        Debug.Log($"Selected Word {currentSelectedWord}");
        OnTextWordPlay(currentSelectedWord);
    }
    private async void OnTextWordPlay(string text)
    {
        var audio = await piperManager.TextToSpeech(text);
        audioManager.PlayAudioClip(audio);
    }
    public void SpeakAgainSelectedWord()
    {
        if (string.IsNullOrEmpty(this.currentSelectedWord))
            return;
        OnTextWordPlay(currentSelectedWord);
        
    }
    public void SelectNextWord()
    {
        string word = textDataProcessor.getNextWord(loadMisspelledWordsOnly);
        if (currentSelectedWord.CompareTo(word) == 1)
        {
            currentSelectedWord = word;
            OnTextWordPlay(currentSelectedWord);
            setState(State.Start);
        }
        else 
        {
            SelectNextWord();
        }
    }
    public bool evaluateCurrentWord(string input)
    {
        // Remove the trailing '\r' from the currentSelectedWord if it exists
        string cleanedCurrentWord = currentSelectedWord.TrimEnd('\r');

        // Compare the cleaned current word with the input string, ignoring case

        var result = cleanedCurrentWord == input.ToLower().Trim();
        if (!result)
        {
            AttemptCount++;
            // Add the word to wrong queue
            textDataProcessor.AddToWrongWordList(input.ToLower().Trim());
            // Store the word in another file and write it streaming assets.

            if (AttemptCount >= 2)
                OnShowHint?.Invoke(currentSelectedWord);
        }
        else 
        {
            AttemptCount = 0;
        }


        return result;
    }
    public State getCurrentState() => currnetState;
    public void setState(State state)
    {
        currnetState = state;
        switch (currnetState)
        {
            case State.Start:
                OnStart?.Invoke();
                break;
            case State.Submitted:
                OnSubmission?.Invoke();
                break;
            case State.Win:
                OnWin?.Invoke();
                break;
            case State.Loose:
                OnLoose?.Invoke();
                break;
        }

        Debug.Log($"Current State {currnetState}");
    }


}


public enum State 
{
    Start,
    Submitted,
    Loose,
    Win
}
