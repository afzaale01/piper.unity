using System;
using UnityEngine;
using Piper;
public class GameManager : MonoBehaviour
{

    [SerializeField] private State currnetState = State.Start;

    #region Services Modules
    [SerializeField] private AudioManager audioManager;
    public TextDataProcessor textDataProcessor { get; private set; }
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
    public string selectedDataFile { get;  private set; }
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
                textDataProcessor.ReadFilesFromStreamingAssets();
                
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    
    private void ServicesInitialization()
    {
        this.textDataProcessor = new TextDataProcessor();

    }
    
    public void UpdateDataSelection(string selectedDataFile)
    {
        this.selectedDataFile = selectedDataFile;
    }
    
    public void StartPlay()
    {
        currentSelectedWord = textDataProcessor.getNextWordFrom(this.selectedDataFile);
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
        string word = textDataProcessor.getNextWordFrom(this.selectedDataFile);
        if (currentSelectedWord != word)
        {
            currentSelectedWord = word;
            OnTextWordPlay(currentSelectedWord);
            setState(State.Start);
        }
        else 
        {
            //SelectNextWord();
        }
    }
    
    public bool EvaluateCurrentWord(string input)
    {
        // Remove the trailing '\r' from the currentSelectedWord if it exists
        string cleanedCurrentWord = currentSelectedWord.TrimEnd('\r');

        // Compare the cleaned current word with the input string, ignoring case

        var result = cleanedCurrentWord == input.ToLower().Trim();
        if (!result)
        {
            AttemptCount++;
            // Add the word to wrong queue
            //textDataProcessor.AddToWrongWordList(input.ToLower().Trim());
            // Store the word in another file and write it streaming assets.

            if (AttemptCount >= 2)
                OnShowHint?.Invoke(currentSelectedWord);
        }
        else 
        {
            AttemptCount = 0;
            textDataProcessor.SaveCurrentIndex(selectedDataFile);
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

        Debug.Log($"{nameof(GameManager)}:Current State {currnetState}");
    }
    
}


public enum State 
{
    Start,
    Submitted,
    Loose,
    Win
}
