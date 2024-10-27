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
    [SerializeField] private TMP_Dropdown _dataDropDown;
    
    #region Private Variable
    private GameManager gameManager;
    private TextDataProcessor _textDataProcessor;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            _textDataProcessor = gameManager.textDataProcessor;
        }
        initialization();
    }

    private void Update()
    {
        if (gameManager != null && userInputField.text.Length >0 )
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnSubmit();
            }
        }

        if (gameManager != null && gameManager.getCurrentState() == State.Win)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnSpeakNextBtn();
            }
        }
    }

    private void initialization()
    {
        submitBtn.onClick.AddListener(OnSubmit);
        speakBtn.onClick.AddListener(OnSpeakNextBtn);

        if (gameManager != null)
        {
            gameManager.OnShowHint += OnShowHint;

            SetupDropDown(this._textDataProcessor.GetFileNames());

        }
    }

    private void SetupDropDown(List<string> list)
    {
        if (_dataDropDown != null)
        {
            _dataDropDown.ClearOptions();
            _dataDropDown.AddOptions(list);
        }
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
            gameManager.UpdateDataSelection(getTheSelectionFromDropDown());
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
           bool result =  gameManager.EvaluateCurrentWord(userInputField.text);

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
        userInputField.text = "";
        userInputField.Select();
        userInputField.ActivateInputField();
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

    private string getTheSelectionFromDropDown()
    {
        int selectedIndex = _dataDropDown.value;

        // Get the list of options
        TMP_Dropdown.OptionData[] options = _dataDropDown.options.ToArray();

        // Access the selected option using the index
        string selectedFilename = options[selectedIndex].text;
        return string.IsNullOrEmpty(selectedFilename) ? "data" : selectedFilename;
    }

}
