using Assets.Tiles.Scripts;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class TutorialController : MonoBehaviour
    {
        #region Private Fields

        [SerializeField] private Button _btnDie1;
        [SerializeField] private Button _btnDie2;
        [SerializeField] private Button _btnRollDice;
        private int _currentPageNumber;
        [SerializeField] private Tile _glowTile;
        [SerializeField] private string[] _tutorialPages;
        [SerializeField] private Text _txtTutorial;
        [SerializeField] private Text _txtTutorialCurrentPage;

        #endregion Private Fields


        #region Public Properties

        [NotNull] public string[] TutorialPages { get { return _tutorialPages; } set { _tutorialPages = value; } }

        #endregion Public Properties


        #region Public Methods

        /// <summary>
        /// Advances the tutorial page.
        /// </summary>
        /// <param name="increment">The increment.</param>
        public void AdvanceTutorialPage(int increment)
        {
            _currentPageNumber = _currentPageNumber + increment;
            UpdateTutorial(_currentPageNumber);
        }

        /// <summary>
        /// Updates the tutorial.
        /// </summary>
        /// <param name="newPageNumber">The new page number.</param>
        public void UpdateTutorial(int newPageNumber)
        {
            SetHighlightedComponents(_currentPageNumber, newPageNumber);
            _currentPageNumber = Mod(newPageNumber, TutorialPages.Length);
            _txtTutorial.text = (TutorialPages[_currentPageNumber]);
            _txtTutorialCurrentPage.text = string.Format("{0}/{1}", _currentPageNumber + 1, TutorialPages.Length);
        }

        #endregion Public Methods


        #region Private Methods

        private static int Mod(int x, int m) { return (x % m + m) % m; }

        /// <summary>
        /// Initializes the tutorial pages.
        /// </summary>
        private void InitTutorialPages()
        {
            TutorialPages[0] = "Hello.\n\nWelcome to Robbers\n\nPlease navigate the game tutorial using the buttons below to learn how to play.";
            TutorialPages[1] = "Your goal is to collect gold, to win, you must have 50 more gold than the player with the least gold.";
            TutorialPages[2] =
                    "There are two phases of your turn. Roll and Move\n\nTo roll, click the \"Roll Dice\" button on the left of your screen now.";
            TutorialPages[3] =
                    "Good.\nAs you can see, two d6 dice have been rolled for you, each die specifies the options you have for how many tiles you can moveWelcome to Robbers\n(eg. 1 and 3 means you can move either 1 tile of 3 tiles exactly";
            TutorialPages[4] =
                    "The current player is highlighted by a white outline, currently this should be you\nThe tiles you can move to are highlighted green, these are calculated from the dice you rolled";
            TutorialPages[5] =
                    "As your aim to to collect gold, you'll want to make your way towards one of the many gold spawners that are scattered around the map\nDo this now by clicking on a green tile that will get you closer to a gold spawner.";
            TutorialPages[6] =
                    "Well Done! The other players will now take their turns, they have the same goal as you, so they might try and beat you to a gold spawner!";
            TutorialPages[7] = "A player has won the game when they have gained have a gold difference of +50 compared with the player with the least gold";
            TutorialPages[8] = "If you have managed to get your character into a good location, instead of moving a number of squares indicated by the dice\nYou can skip your turn by pressing the \"Skip Turn\" button";
            TutorialPages[9] = "When a player lands on a gold spawner, they are forced to stay on the spawner until all the gold has been collected from it. Keep this in mind when going for the gold.";

            TutorialPages[10] =
                    "Additionally, some of the tiles on the map are rivers, these push you an extra tile in a specified direction when you land on them\nYou can see this direction by hovering over the river tile";
            TutorialPages[11] = "In the centre of the map is the jail.\nWhen a player moves to this tile, any players who are currently standing on a coin spawner will be caught stealing.";
            TutorialPages[12] = "Players who are caught stealing lose 20 gold, lose their next turn, and the gold spawner they were stealing from is also wiped clean \nUse this to overtake the other players";
            TutorialPages[13] = "Every so often, powerups will spawn around the map, whenever you walk over them, they will be added to your inventory\nYou can save these for later, but once you use them they are gone";
            TutorialPages[14] = "There are three types of powerups, each gives you a different bonus";
            TutorialPages[15] = "Lucky - Add 1 to both of your dice rolls";
            TutorialPages[16] = "Flying - You can move over walls for one turn";
            TutorialPages[17] = "Defence - If a player uses the jail to catch you stealing, this will get you out of jail";
            TutorialPages[18] = "And that's pretty much the whole game, feel free to keep playing! Have fun!";

        }

        /// <summary>
        /// Sets the highlighted components.
        /// </summary>
        /// <param name="currentPageNumber">The current page number.</param>
        /// <param name="newPageNumber">The new page number.</param>
        private void SetHighlightedComponents(int currentPageNumber, int newPageNumber)
        {
            UIController uiController = UIController.GetUIController();
            GameController gameController = GameController.GetGameController();
            if (gameController != null && uiController != null)
            {
                _glowTile = gameController.GetGameTile(1, 1);

                switch (newPageNumber)
                {
                    case 2:
                        if (gameController != null) _glowTile.StopGlowing();
                        uiController.ToggleButtonGlowing(_btnRollDice, true);
                        uiController.ToggleButtonGlowing(_btnDie1, false);
                        uiController.ToggleButtonGlowing(_btnDie2, false);
                        break;

                    case 3:
                        if (gameController != null) _glowTile.StopGlowing();
                        uiController.ToggleButtonGlowing(_btnRollDice, false);
                        uiController.ToggleButtonGlowing(_btnDie1, true);
                        uiController.ToggleButtonGlowing(_btnDie2, true);
                        break;

                    case 5:
                        if (gameController != null) _glowTile.Glow();
                        uiController.ToggleButtonGlowing(_btnRollDice, false);
                        uiController.ToggleButtonGlowing(_btnDie1, false);
                        uiController.ToggleButtonGlowing(_btnDie2, false);
                        break;

                    default:
                        if (gameController != null) _glowTile.StopGlowing();
                        uiController.ToggleButtonGlowing(_btnRollDice, false);
                        uiController.ToggleButtonGlowing(_btnDie1, false);
                        uiController.ToggleButtonGlowing(_btnDie2, false);
                        break;
                }
            }
        }

        // Use this for initialization
        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            InitTutorialPages();
            UpdateTutorial(0);
        }

        // Update is called once per frame
        private void Update() { }

        #endregion Private Methods
    }
}