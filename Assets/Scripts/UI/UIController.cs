using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    [SerializeField] private Button submitBtn;
    [SerializeField] private TMP_InputField userInputField;
    [SerializeField] private Button speakBtn;
    [SerializeField] private TMP_Text attemptResultText;
    [SerializeField] private TMP_Text realWord;
    [SerializeField] private Toggle saveMisspelledWords;
    [SerializeField] private Toggle loadOnlyMisspelledWords;

    #region Private Variable
    private GameManager gameManager;
    #endregion


    



    // Start is called before the first frame update
    void Start()
    {
        if(gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        initialization();
    }

    private void initialization()
    {
        submitBtn.onClick.AddListener(OnSubmit);
        speakBtn.onClick.AddListener(OnSpeakNextBtn);

        if (gameManager != null)
            gameManager.OnShowHint += OnShowHint;

    }

    private void OnShowHint(string obj)
    {
       StartCoroutine(ShowInRealWordFor(obj, 3));
    }

    private IEnumerator ShowInRealWordFor(string obj, int v)
    {
        realWord.text = obj;
        yield return new WaitForSeconds(v);
        realWord.text = "";
    }

    public void OnStartPress()
    {
        if (gameManager != null)
        {
            gameManager.UpdateDataSelection(loadOnlyMisspelledWords.isOn);
            gameManager.StartPlay();
        }
    }


    private void OnSubmit()
    {
        if (userInputField.text.Length <= 0 && string.IsNullOrWhiteSpace(userInputField.text))
        {
            Debug.Log("Please enter a word");
        }
        else
        {
           gameManager.setState(State.Submitted);
           bool result =  gameManager.evaluateCurrentWord(userInputField.text);

            if (result)
            {
                gameManager.setState(State.Win);
                HandleWin();
            }
            else 
            {
                gameManager.setState(State.Loose);
                HandleLoose();
            }

        }
    }

    private void HandleLoose()
    {
        attemptResultText.color = Color.red;
        attemptResultText.text = "Sorry Try again";
    }

    private void HandleWin()
    {
        // Congratulate

        attemptResultText.color = Color.green;
        attemptResultText.text = "Congratulations!! you are doing great.";

        // Next word select
        submitBtn.gameObject.SetActive(false);
        speakBtn.gameObject.SetActive(true);

        // Wait for ready to play input from user
        
    }

    private void OnSpeakNextBtn()
    {
        if (gameManager != null)
        {
            if (gameManager.getCurrentState() == State.Start || gameManager.getCurrentState() == State.Loose)
                gameManager.SpeakAgainSelectedWord();
            else if(gameManager.getCurrentState() == State.Win)
            {
                gameManager.SelectNextWord();
            }
            submitBtn.gameObject.SetActive(true);
            attemptResultText.color = Color.white;
            attemptResultText.text = "Try to make it";
        }
    }

}
