using UnityEngine;
using System.Collections.Generic;

public class ScoringSystem : MonoBehaviour
{
    public InsultGenerator insultGenerator; // Reference to the InsultGenerator script
    private int playerScore = 0;
    private int aiScore = 0;
    private bool scoresCalculated = false; // Flag to ensure scores are calculated only once

    private void Update()
    {
        // Check if both playerEndGame and aiEndGame are true
        if (insultGenerator.playerEndGame && insultGenerator.aiEndGame && !scoresCalculated)
        {
            HandleScoresCalculated();
        }
    }

    void HandleScoresCalculated()
    {
        // Calculate scores only once
        if (!scoresCalculated)
        {
            playerScore = CalculateScore(insultGenerator.playerText.text);
            aiScore = CalculateScore(insultGenerator.aiText.text);

            // Debug the final scores
            Debug.Log("Player Score: " + playerScore);
            Debug.Log("AI Score: " + aiScore);

            // Deduct scores from health
            // insultGenerator.playerHealth -= aiScore;
            // insultGenerator.aiHealth -= playerScore;

            // Set the flag to true to prevent recalculation
            scoresCalculated = true;
        }
    }

    public int CalculateScore(string text)
    {
        int score = 1; // Default score is 1 point

        // Convert the text to lower case for case-insensitive matching
        text = text.ToLower();

        // Count the number of words in the text
        // int wordCount = CountWords(text);

        // Add 2 points if the word count is 8 or more
        // if (wordCount >= 8)
        // {
        //     score += 2;
        // }

        // Check if the text contains specific combinations in the correct order
        if (HasCombination(text, insultGenerator.beginningsList, insultGenerator.otherList, insultGenerator.beginningsList, insultGenerator.middlesList, insultGenerator.endingsList))
        {
            score = 12;
        }
        else if (HasCombination(text, insultGenerator.beginningsList, insultGenerator.middlesList, insultGenerator.otherList, insultGenerator.beginningsList, insultGenerator.endingsList))
        {
            score = 12;
        }
        else if (HasCombination(text, insultGenerator.beginningsList, insultGenerator.middlesList, insultGenerator.beginningsList, insultGenerator.middlesList, insultGenerator.endingsList))
        {
            score = 12;
        }
        else if (HasCombination(text, insultGenerator.beginningsList, insultGenerator.middlesList, insultGenerator.beginningsList, insultGenerator.endingsList))
        {
            score = 8;
        }
        else if (HasCombination(text, insultGenerator.beginningsList, insultGenerator.otherList, insultGenerator.middlesList, insultGenerator.endingsList))
        {
            score = 8;
        }
        else if (HasCombination(text, insultGenerator.beginningsList, insultGenerator.middlesList, insultGenerator.otherList, insultGenerator.endingsList))
        {
            score = 8;
        }
        else if (HasCombination(text, insultGenerator.beginningsList, insultGenerator.middlesList, insultGenerator.endingsList))
        {
            score = 5;
        }
        else if (HasCombination(text, insultGenerator.beginningsList, insultGenerator.otherList, insultGenerator.beginningsList, insultGenerator.endingsList))
        {
            score = 3;
        }
        else if (HasCombination(text, insultGenerator.beginningsList, insultGenerator.middlesList, insultGenerator.middlesList, insultGenerator.endingsList))
        {
            score = -3;
        }
        else if (HasCombination(text, insultGenerator.beginningsList, insultGenerator.endingsList))
        {
            score = -3;
        }
        else if (HasCombination(text, insultGenerator.middlesList, insultGenerator.endingsList))
        {
            score = -3;
        }
        else if (HasCombination(text, insultGenerator.otherList, insultGenerator.endingsList))
        {
            score = -3;
        }
        else if (HasCombination(text, insultGenerator.beginningsList))
        {
            score = -3;
        }
        else if (HasCombination(text, insultGenerator.otherList))
        {
            score = -3;
        }
        else if (HasCombination(text, insultGenerator.middlesList))
        {
            score = -3;
        }
        else if (HasCombination(text, insultGenerator.endingsList))
        {
            score = -3;
        }

        return score;
    }

    bool HasCombination(string text, params List<string>[] lists)
    {
        int previousIndex = -1;

        foreach (var list in lists)
        {
            bool found = false;
            foreach (var word in list)
            {
                int index = text.IndexOf(word.ToLower(), previousIndex + 1);
                if (index != -1 && index > previousIndex)
                {
                    previousIndex = index;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                return false;
            }
        }

        return true;
    }

    // private int CountWords(string text)
    // {
    //     // Split the text into words by spaces
    //     string[] words = text.Split(' ');

    //     // Return the number of words
    //     return words.Length;
    // }
}

