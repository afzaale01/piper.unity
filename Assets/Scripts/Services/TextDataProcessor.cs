using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextDataProcessor
{

    public Action<List<string>> OnReadyComplete;
   
    private bool isReadCompleted;
    bool isFirstTime = false;
    private int counter =0;
    List<string> filenames = new List<string>();
    private Dictionary<string, List<string>> wordsList = new Dictionary<string, List<string>>();
    private Dictionary<string, int> fileNamesIndex = new Dictionary<string, int>();
    string wrongWordFileName = "misspelled";

    public TextDataProcessor() 
    {
        
    }
    
    public void AddToWrongWordList(string word)
    {
        if (wordsList[wrongWordFileName] != null && !wordsList[wrongWordFileName].Contains(word))
        {
            wordsList[wrongWordFileName].Add(word);
            Debug.Log($"{nameof(TextDataProcessor)}: Count of words list {wordsList[wrongWordFileName].Count}");
            WriteDataForSavingWrongWords(word);
        }
    }

    #region Private Function


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
    private List<string> ReadFromPath(string filePath)
    {
        try
        {
            List<string> words = new List<string>();
            if (File.Exists(filePath))
            {
                string text = File.ReadAllText(filePath);
                string[] lines = text.Split('\n'); // Split the text into lines based on newlines

                foreach (string line in lines)
                {
                    string[] wordsInLine = line.Split(' '); // Split each line into words based on spaces
                    words.AddRange(wordsInLine); // Add the words from the line to the list
                }
                return words;
            }
            else
            {
                Debug.Log("File not found: " + filePath);
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return null;
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


    public void AppendWordToFile(string filePath, string word)
    {
        try
        {
            // Append the word to the file, creating it if it doesn't exist
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                Debug.Log("Writing the word");
                writer.WriteLine(word);

            }
        }
        catch (IOException e)
        {
            Debug.LogError("Error appending to file: " + e.Message);
        }
    }
    /*private void AppendWordToFile(string filePath, string word)
    {
        try
        {
            // Append the word to the file
            File.AppendAllText(filePath, Environment.NewLine + word );
        }
        catch (IOException e)
        {
            Debug.LogError("Error appending to file: " + e.Message);
        }
    }*/

    #endregion

    public string getNextWordFrom(string dataSetName)
    {
        string selectedWord = string.Empty;
        if (!isFirstTime)
        {
            counter = fileNamesIndex[dataSetName];
            isFirstTime = true;
        }

        try
        {
            List<string> words = wordsList[dataSetName];
            if (words == null || words.Count < 0)
                return "";

            selectedWord = words[counter].TrimEnd('\r');
            counter++;
            fileNamesIndex[dataSetName] = counter;
            if (counter >= words.Count)
                counter = 0;

            return selectedWord;
        }
        catch (Exception ex)
        {
            Debug.Log($"exception {ex} selectedWord {selectedWord}");
        }

        Debug.Log($"{nameof(TextDataProcessor)}: Counter state {counter}");

        return selectedWord;

    }
    public void ReadFilesFromStreamingAssets() 
    {
        string[] filePaths = Directory.GetFiles(Application.streamingAssetsPath, "*.txt", SearchOption.TopDirectoryOnly);
        
        foreach (string filePath in filePaths)
        {
            // Extract filename without the path
            string filename = Path.GetFileNameWithoutExtension(filePath);
            filenames.Add(filename);
            wordsList.Add(filename, ReadFromPath(filePath));
            fileNamesIndex.Add(filename,PlayerPrefs.GetInt(filename));
            Debug.Log($"{nameof(TextDataProcessor)}: Reading file {filename} from File Path {filePath}");
        }

        isReadCompleted = true;


    }
    public List<string> GetFileNames()
    {
        return filenames != null ? filenames : null;
    }
    public List<string> GetWordsOfFile(string name) 
    {
        if (wordsList.ContainsKey(name) && isReadCompleted)
        {
            return wordsList[name];
        }
        else 
        {
            return null;
        }
    }
    public void SaveCurrentIndex(string fileName)
    {
        try
        {
            PlayerPrefs.SetInt(fileName,counter);
        }
        catch (Exception ex)
        {
            Debug.Log($"Problem {ex}");
        }
    }
    public void WriteDataForSavingWrongWords(string wrongeWord)
    {
        //AppendWordToFile(wrongeWord, Path.Combine(Application.streamingAssetsPath,wrongWordFileName+".txt"));
        WriteToFile(wordsList[wrongWordFileName], Application.streamingAssetsPath + "/" + wrongWordFileName+".txt");
        //AppendWordToFile(wrongeWord, Application.streamingAssetsPath + "/" + wrongWordFileName+".txt");
    }

}
