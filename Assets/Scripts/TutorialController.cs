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
        _txtTutorial.text = TutorialPages[_currentPageNumber];
        _txtTutorialCurrentPage.text = string.Format("{0}/{1}", _currentPageNumber + 1, TutorialPages.Length);
    }

    #endregion Public Methods

    #region Private Methods

    private static int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    // Use this for initialization
    private void Start()
    {
        UpdateTutorial(0);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    #endregion Private Methods
}