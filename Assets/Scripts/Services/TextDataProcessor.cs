using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextDataProcessor
{

    public Action<List<string>> OnReadyComplete;

    private List<string> words ;

    public TextDataProcessor() 
    {
        if (this.words == null)
            this.words = new List<string>();
    }


    public void ReadFromPath(string filePath)
    {
        try
        {

            if (File.Exists(filePath))
            {
                string text = File.ReadAllText(filePath);
                string[] lines = text.Split('\n'); // Split the text into lines based on newlines

                foreach (string line in lines)
                {
                    string[] wordsInLine = line.Split(' '); // Split each line into words based on spaces
                    words.AddRange(wordsInLine); // Add the words from the line to the list
                }

                OnReadyComplete.Invoke(words);

            }
            else
            {
                Debug.Log("File not found: " + filePath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public string getRandomWord() 
    {
        if (words == null || words.Count < 0)
            return "";
        return words[UnityEngine.Random.Range(0, words.Count - 1)];
    }

}
