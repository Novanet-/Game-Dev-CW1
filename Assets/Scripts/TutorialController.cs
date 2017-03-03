﻿using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    #region Private Fields

    private int _currentPageNumber;

    [SerializeField] private string[] _tutorialPages;

    [SerializeField] private Text _txtTutorial;

    [SerializeField] private Text _txtTutorialCurrentPage;

    #endregion Private Fields

    #region Public Properties

    [NotNull] public string[] TutorialPages
    {
        get { return _tutorialPages; }
        set { _tutorialPages = value; }
    }

    #endregion Public Properties

    #region Public Methods

    public void AdvanceTutorialPage(int increment)
    {
        _currentPageNumber = _currentPageNumber + increment;
        UpdateTutorial(_currentPageNumber);
    }

    public void UpdateTutorial(int newPageNumber)
    {
        _currentPageNumber = Mod(newPageNumber, TutorialPages.Length);
        _txtTutorial.text = newLineReplace(TutorialPages[_currentPageNumber]);
        _txtTutorialCurrentPage.text = string.Format("{0}/{1}", _currentPageNumber + 1, TutorialPages.Length);
    }

    #endregion Public Methods

    #region Private Methods

    private string newLineReplace(string inText)
    {
        bool newLinesRemaining = true;

        while (newLinesRemaining)
        {
            int cIndex = inText.IndexOf("\\n", StringComparison.Ordinal); // Gets the Index of "\n"


            if (cIndex == -1)
            {
                newLinesRemaining = false;
            }
            else
            {
                inText = inText.Remove(cIndex, 2); // Removes "\n from original String"
                inText = inText.Insert(cIndex, "\r\n"); // Adds the actual New Line symbol
            }
        }

        return inText;
    }

    private static int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    // Use this for initialization
    private void Start()
    {
        InitTutorialPages();
        UpdateTutorial(0);
    }

    private void InitTutorialPages()
    {
        TutorialPages[0] = "Hello.\n\nWelcome to Robbers\n\nPlease navigate the game tutorial using the buttons below to learn how to play.";
        TutorialPages[1] = "Your goal is to collect gold, to win, you must have 20 more gold than the player with the least gold.";
        TutorialPages[2] = "There are two phases of your turn. Roll and Move\n\nTo roll, click the \"Roll Dice\" button on the left of your screen now.";
        TutorialPages[3] = "Good.\nAs you can see, two d6 dice have been rolled for you, each die specifies the options you have for how many tiles you can moveWelcome to Robbers\n(eg. 1 and 3 means you can move either 1 tile of 3 tiles exactly";
        TutorialPages[4] = "The current player is highlighted by a white outline, currently this should be you\nThe tiles you can move to are highlighted green, these are calculated from the dice you rolled";
        TutorialPages[5] = "As your aim to to collect gold, you'll want to make your way towards one of the many gold spawners that are scattered around the map\nDo this now by clicking on a green tile that will get you closer to a gold spawner.";
        TutorialPages[6] = "Well Done! The other players will now take their turns, they have the same goal as you, so they might try and beat you to a gold spawner!";
        TutorialPages[7] = "Additionally, some of the tiles on the map are rivers, these push you an extra tile in a specified direction when you land on them";
        TutorialPages[8] = "And that's pretty much the whole game, feel free to keep playing! Have fun!";
    }

    // Update is called once per frame
    private void Update()
    {
    }

    #endregion Private Methods
}