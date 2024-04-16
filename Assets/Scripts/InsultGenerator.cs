using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class InsultGenerator : MonoBehaviour
{
    public List<string> beginningsList;
    public List<string> middlesList;
    public List<string> endingsList;
    public List<string> otherList; // New list for "Other"
    public Transform buttonContainer;
    public GameObject buttonPrefab;
    public TextMeshProUGUI playerText; // Reference to the player's dialogue text
    public TextMeshProUGUI aiText; // Reference to the AI's dialogue text
    public TextMeshProUGUI turnText; // Text to display whose turn it is
    public int numberOfBeginningsButtons = 3;
    public int numberOfMiddlesButtons = 3;
    public int numberOfEndingsButtons = 3;
    public int numberOfOtherButtons = 3; // Control how many buttons spawn for "Other"
    public int maxTurns = 5; // Maximum number of turns for each side
    public float spacing = 1f; // Adjust this value for the desired spacing

    private bool isPlayerTurn = true;
    private bool isAIsTurnInProgress = false;
    public bool playerEndGame = false; // Flag to track if the game has ended for the player
    public bool aiEndGame = false; // Flag to track if the game has ended for the AI
    private int playerTurns = 0;
    private int aiTurns = 0;

    void Start()
    {
        GenerateButtons();
        UpdateTurnText();
    }

    void Update()
    {

        // Check if it's AI's turn and AI hasn't reached endgame
        if (!isPlayerTurn && !aiEndGame)
        {
            AIChooseWord();

            // Log AI's current turn number
            Debug.Log("AI's Turn: " + aiTurns);

            // Check if AI reached endgame or max turns
            if (aiEndGame || aiTurns >= maxTurns && !playerEndGame)
            {
                // Switch turn to player
                isPlayerTurn = true;
                UpdateTurnText();
            }
        }
        // Check if it's not the player's turn and the game is ongoing
        else if (!isPlayerTurn && playerEndGame && !aiEndGame)
        {
            // Handle AI's turn while the game is ongoing
            AIChooseWord();

            // Log AI's current turn number
            Debug.Log("AI's Turn: " + aiTurns);

            // Check if AI reached endgame or max turns
            if (aiEndGame || aiTurns >= maxTurns && !playerEndGame)
            {
                // Switch turn to player
                isPlayerTurn = true;
                UpdateTurnText();
            }
        }

        if (aiEndGame && playerEndGame)
        {
            isPlayerTurn = false;
            turnText.text = "Round Complete!";
        }
    }

    void UpdateTurnText()
    {
        turnText.text = isPlayerTurn ? "Player's Turn" : "AI's Turn";
    }

    void GenerateButtons()
    {
        // Calculate the total number of buttons
        int totalButtons = numberOfBeginningsButtons + numberOfMiddlesButtons +
                           numberOfEndingsButtons + numberOfOtherButtons;

        // Get the height of the button container
        RectTransform containerRect = buttonContainer.GetComponent<RectTransform>();
        float containerHeight = containerRect.rect.height;

        // Calculate the total height of the buttons
        float totalButtonHeight = totalButtons * (buttonPrefab.GetComponent<RectTransform>().sizeDelta.y + spacing) - spacing;

        // Calculate the initial yOffset to center the buttons vertically
        float yOffset = containerHeight / 2f + totalButtonHeight / 2f;

        // Shuffle each list individually
        Shuffle(beginningsList);
        Shuffle(middlesList);
        Shuffle(endingsList);
        Shuffle(otherList); // Shuffle the "Other" list

        // Select a certain number of buttons from each shuffled list
        List<string> selectedButtons = new List<string>();
        selectedButtons.AddRange(beginningsList.Take(numberOfBeginningsButtons));
        selectedButtons.AddRange(middlesList.Take(numberOfMiddlesButtons));
        selectedButtons.AddRange(endingsList.Take(numberOfEndingsButtons));
        selectedButtons.AddRange(otherList.Take(numberOfOtherButtons)); // Add buttons from "Other"

        // Shuffle the selected buttons again to mix them
        Shuffle(selectedButtons);

        // Generate buttons from the shuffled list
        GenerateButtonsForList(selectedButtons, yOffset);
    }

    void GenerateButtonsForList(List<string> list, float yOffset)
    {
        for (int i = 0; i < list.Count; i++)
        {
            // Get the selected button text
            string buttonText = list[i];

            // Create button
            GameObject buttonGO = Instantiate(buttonPrefab, buttonContainer);
            Button button = buttonGO.GetComponent<Button>();

            // Access TextMeshPro component instead of Text
            TMP_Text buttonTextComponent = buttonGO.GetComponentInChildren<TMP_Text>();
            buttonTextComponent.text = buttonText;

            // Calculate button position
            Vector3 buttonPosition = new Vector3(0f, yOffset - i * (buttonPrefab.GetComponent<RectTransform>().sizeDelta.y + spacing), 0f);
            buttonGO.GetComponent<RectTransform>().localPosition = buttonPosition;

            // Add button click listener
            button.onClick.AddListener(() => OnButtonClick(buttonText, buttonGO));
        }
    }

    void OnButtonClick(string word, GameObject buttonGO)
    {
        // Disable button interaction if the game has ended
        if (playerEndGame) return;

        // Append the selected word to the current text
        playerText.text += (string.IsNullOrEmpty(playerText.text) ? "" : " ") + word;

        // Destroy the button game object
        Destroy(buttonGO);

        // Increment player's turn count
        playerTurns++;

        // Log player's current turn number
        Debug.Log("Player's Turn: " + playerTurns);

        // Check if the player chose an ending word
        if (endingsList.Contains(word))
        {
            Debug.Log("Player chose an ending word. Game Over for Player");
            playerEndGame = true;
        }

        // Check if the game has ended for the player
        if (playerEndGame || playerTurns >= maxTurns)
        {
            // End the game
            playerEndGame = true;
            isPlayerTurn = false;
            UpdateTurnText();
        }

        // Switch turns if the game hasn't ended
        if (!playerEndGame && !aiEndGame) // Check if the game hasn't ended for the player
        {
            // Switch turn to AI
            isPlayerTurn = false;
            UpdateTurnText();
        }

        if (playerEndGame && !aiEndGame)
        {
            isPlayerTurn = false;
            UpdateTurnText();
        }

        if (!playerEndGame && aiEndGame)
        {
            isPlayerTurn = true;
            UpdateTurnText();
        }
    }

    void AIChooseWord()
    {
        // Check if AI's turn is already in progress
        if (!isAIsTurnInProgress && !aiEndGame)
        {
            // Set the flag to indicate AI's turn is in progress
            isAIsTurnInProgress = true;

            // Call the delegate with a 2-second delay
            Invoke(nameof(ChooseWordWithDelay), 2f);
        }
    }

    // variable to keep track of the current combo index
    int currentComboIndex = -1;
    internal System.Action<bool> OnGameEnd;

    void ChooseWordWithDelay()
    {

        string chosenWord = "";

        // If all words from the current combo index have been chosen,
        // or if no combo index has been selected yet, choose a new one
        if (currentComboIndex == -1 || IsComboComplete(currentComboIndex))
        {
            // Randomly select one of the combinations
            currentComboIndex = Random.Range(0, 5);
            Debug.Log("AI chose combo index: " + currentComboIndex);
        }

        // Debug log to check the contents of endingsList
        Debug.Log("Endings List: " + string.Join(", ", endingsList));

        // Use the current combo index to select the word
        switch (currentComboIndex)
        {
            case 0:
                // Beginning + Other + Beginning + Middle + Ending
                if (aiTurns == 0 || aiTurns == 2)
                {
                    chosenWord = GetWordFromList(beginningsList);
                }
                else if (aiTurns == 1)
                {
                    chosenWord = GetWordFromList(otherList);
                }
                else if (aiTurns == 3)
                {
                    chosenWord = GetWordFromList(middlesList);
                }
                else if (aiTurns == 4)
                {
                    chosenWord = GetWordFromList(endingsList);
                }
                break;
            case 1:
                // Beginning + Middle + Other + Ending
                if (aiTurns == 0)
                {
                    chosenWord = GetWordFromList(beginningsList);
                }
                else if (aiTurns == 1)
                {
                    chosenWord = GetWordFromList(middlesList);
                }
                else if (aiTurns == 2)
                {
                    chosenWord = GetWordFromList(otherList);
                }
                else if (aiTurns == 3)
                {
                    chosenWord = GetWordFromList(endingsList);
                }
                break;
            case 2:
                // Beginning + Middle + Ending
                if (aiTurns == 0)
                {
                    chosenWord = GetWordFromList(beginningsList);
                }
                else if (aiTurns == 1)
                {
                    chosenWord = GetWordFromList(middlesList);
                }
                else if (aiTurns == 2)
                {
                    chosenWord = GetWordFromList(endingsList);
                }
                break;
            case 3:
                // Beginning + Middle + Other + Beginning + Ending
                if (aiTurns == 0 || aiTurns == 3)
                {
                    chosenWord = GetWordFromList(beginningsList);
                }
                else if (aiTurns == 1)
                {
                    chosenWord = GetWordFromList(middlesList);
                }
                else if (aiTurns == 2)
                {
                    chosenWord = GetWordFromList(otherList);
                }
                else if (aiTurns == 4)
                {
                    chosenWord = GetWordFromList(endingsList);
                }
                break;
            case 4:
                // Beginning + Other + Middle + Ending
                if (aiTurns == 0)
                {
                    chosenWord = GetWordFromList(beginningsList);
                }
                else if (aiTurns == 1)
                {
                    chosenWord = GetWordFromList(otherList);
                }
                else if (aiTurns == 2)
                {
                    chosenWord = GetWordFromList(middlesList);
                }
                else if (aiTurns == 3)
                {
                    chosenWord = GetWordFromList(endingsList);
                }
                break;
        }

        // Display AI's chosen word
        aiText.text += (string.IsNullOrEmpty(aiText.text) ? "" : " ") + chosenWord;

        Debug.Log("AI chose word: " + chosenWord);

        // Increment AI's turn count
        aiTurns++;

        // Check if the AI chose an ending word
        if (endingsList.Contains(chosenWord))
        {
            Debug.Log("AI chose an ending word. Game Over for AI");
            aiEndGame = true;
        }
        else
        {
            Debug.Log("Chosen word is not an ending word: " + chosenWord);
        }

        // Check if AI reached endgame or max turns
        if (aiEndGame || aiTurns >= maxTurns)
        {
            // End the game
            aiEndGame = true;
        }

        // Remove the chosen word from the list of buttons
        RemoveWordFromButtons(chosenWord);

        if (aiEndGame && !playerEndGame)
        {
            isPlayerTurn = true;
            UpdateTurnText();
        }

        if (!aiEndGame && !playerEndGame)
        {
            isPlayerTurn = true;
            UpdateTurnText();
        }

        // Reset the flag to indicate AI's turn is complete
        isAIsTurnInProgress = false;
    }

    string GetWordFromList(List<string> wordList)
    {
        // If there are available words on active buttons, choose randomly from them
        foreach (string word in wordList)
        {
            if (IsWordOnActiveButton(word))
            {
                return word;
            }
        }

        // If no available words, return an empty string
        return "";
    }

    bool IsWordOnActiveButton(string word)
    {
        // Check if the word is present on any active button
        Button[] buttons = buttonContainer.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            TMP_Text buttonTextComponent = button.GetComponentInChildren<TMP_Text>();
            if (buttonTextComponent.text == word && button.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    // Method to check if all words from a combo index have been chosen
    bool IsComboComplete(int comboIndex)
    {
        switch (comboIndex)
        {
            case 0:
                return aiTurns >= 5; // For combo index 0, it needs 6 turns to complete
            case 1:
                return aiTurns >= 4; // For combo index 1, it needs 4 turns to complete
            case 2:
                return aiTurns >= 3; // For combo index 2, it needs 3 turns to complete
            case 3:
                return aiTurns >= 5; // For combo index 3, it needs 5 turns to complete
            case 4:
                return aiTurns >= 4; // For combo index 4, it needs 4 turns to complete
            default:
                return false;
        }
    }

    // Method to remove the chosen word from the buttons
    void RemoveWordFromButtons(string chosenWord)
    {
        Button[] buttons = buttonContainer.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            TMP_Text buttonTextComponent = button.GetComponentInChildren<TMP_Text>();
            if (buttonTextComponent.text == chosenWord)
            {
                Destroy(button.gameObject);
                break; // Assuming each word appears only once in the buttons
            }
        }
    }

    // Fisher-Yates shuffle algorithm
    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}