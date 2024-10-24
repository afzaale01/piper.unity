using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextDataProcessor
{

    public Action<List<string>> OnReadyComplete;
    private List<string> words ;
    private List<string> wrongWordsList;

    private int counter =0;



    public TextDataProcessor() 
    {
        if (this.words == null)
            this.words = new List<string>();
        if (this.wrongWordsList == null)
            this.wrongWordsList = new List<string>();
    }
    public void ReadDataList()
    {
        string filePath = Application.streamingAssetsPath + "/data.txt";
        ReadFromPath(filePath, this.words);
    }
    public void ReadMisspelledList()
    {
        string misspelledWords = Application.streamingAssetsPath + "/misspelled.txt";
        ReadFromPath(misspelledWords, this.wrongWordsList);
    }
    public void AddToWrongWordList(string word)
    {
        if (this.wrongWordsList != null && !string.IsNullOrWhiteSpace(word) && !wrongWordsList.Contains(word))
            this.wrongWordsList.Add(word);        
    }
    private void ReadFromPath(string filePath, List<string> words)
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
    private void WriteToFile(List<string> words, string filePath)
    {
        try
        {
            // Create a new file or overwrite an existing one
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Iterate through the list of words and write each word to a new line
                foreach (string word in words)
                {
                    writer.WriteLine(word);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
    public string getRandomWord(bool dataSet) 
    {
        if (words == null || words.Count < 0)
            return "";

        string selectedWord = string.Empty;
        int index;
        try
        {

            if (dataSet && wrongWordsList.Count > 0)
            {
                index = UnityEngine.Random.Range(0, wrongWordsList.Count - 1);

                Debug.Log($"Wrong Words Selection Index {index}");

                selectedWord = wrongWordsList[index].TrimEnd('\r');
            }
            else if (!dataSet || string.IsNullOrEmpty(selectedWord))
            {
                index = UnityEngine.Random.Range(0, words.Count - 1);
                Debug.Log($"Words Selection Index {index}");
                selectedWord = words[index].TrimEnd('\r');
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"exception {ex} selectedWord {selectedWord}");
        }
            return selectedWord;
       
    }

    public string getNextWord(bool dataSet)
    {
        if (words == null || words.Count < 0)
            return "";

        string selectedWord = string.Empty;
        int index;
        try
        {

            index = counter;
            if (dataSet && wrongWordsList.Count > 0 && counter < wrongWordsList.Count)
            {
                //Debug.Log($"Wrong Words Selection Index {index}");
                selectedWord = wrongWordsList[index].TrimEnd('\r');
                counter++;
            }
            else if (!dataSet || string.IsNullOrEmpty(selectedWord) && counter < words.Count)
            {
                //Debug.Log($"Words Selection Index {index}");
                selectedWord = words[index].TrimEnd('\r');
                counter++;
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"exception {ex} selectedWord {selectedWord}");
        }
        return selectedWord;

    }

}
