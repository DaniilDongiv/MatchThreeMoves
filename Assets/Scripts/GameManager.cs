using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Класс - менеджер, инициализирующий и связывающий остальные сущности
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] 
    private LevelsSettingsProvider _levelsSettingsProvider;
    [SerializeField] 
    private ItemsSetProvider _itemsSetProvider;
    
    private GameBoardController _gameBoardController;
    private ItemsMover _itemsMover;
    private AudioManager _audioManager;
    private MatchFinder _matchFinder;
    private ScoreManager _scoreManager;
    private MovesManager _movesManager;
    
    private int _level;
    private Button _backButton;
    private GameOverPopup _gameOverPopup;

    private void Awake()
    {
        _levelsSettingsProvider.Initialize();
        DontDestroyOnLoad(this);
    }
    
    public void LoadGameScene(int level)
    {
        _level = level;
        StartCoroutine(LoadSceneAsync(GlobalConstants.GAME_SCENE));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        var asyncLoading = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoading.isDone)
        {
            yield return null;
        }

        InitializeGameObjects();
    }

    private void InitializeGameObjects()
    {
        _matchFinder = new MatchFinder();

        // Находим на игровой сцене все объекты
        _gameBoardController = FindObjectOfType<GameBoardController>();
        _itemsMover = FindObjectOfType<ItemsMover>();
        _audioManager = FindObjectOfType<AudioManager>();
        _backButton = FindObjectOfType<BackButton>().GetComponent<Button>();
        _scoreManager = FindObjectOfType<ScoreManager>();
        _movesManager = FindObjectOfType<MovesManager>();
        _gameOverPopup = FindObjectOfType<GameOverPopup>();
        _gameOverPopup.gameObject.SetActive(false);

        // Инициализируем их
        var itemTypesNumber = _levelsSettingsProvider.GetItemTypesCount(_level);
        var itemsSet = _itemsSetProvider.GetItemsSet();
        
        var gameBoardIndexProvider = new GameBoardIndexProvider(_gameBoardController);
        _gameBoardController.Initialize(itemsSet, itemTypesNumber, _matchFinder);

        var items = _gameBoardController.CreateGameBoard();
        _itemsMover.Initialize(items, gameBoardIndexProvider);
        
        _itemsMover.ItemsSwapped += OnItemsSwapped;
        _gameBoardController.ReFill += OnItemsFellDown;
        _gameBoardController.ItemsDestroyed += OnItemDestroyed;
        
        _backButton.onClick.AddListener(LoadMenuScene);
        _movesManager.MovesOver += OnMovesOver;
    }
    
    private void LoadMenuScene()
    {
        SceneManager.LoadSceneAsync(GlobalConstants.MENU_SCENE);
    }
    
    private void OnItemsSwapped(Item[,] items)
    {
        // Если на игровом поле совпадения есть - разрушаем совпавшие элементы
        if (_matchFinder.HasMatches(items))
        {
            _gameBoardController.DestroyMatches(_matchFinder.Matches);
        }
        else
        {
            // Если совпадений нет - возвращает элементы обратно на свои места
            _itemsMover.ReSwapItems();
            
            // Если совпадений нет - уменьшаем счетчик ходов
            _movesManager.DecreaseMoves();
        }
    }
    
    private void OnItemsFellDown(Item[,] items)
    {
        // Если на игровом поле совпадения есть - разрушаем совпавшие элементы
        if (_matchFinder.HasMatches(items))
        {
            _gameBoardController.DestroyMatches(_matchFinder.Matches);
        }
        else
        {
            // Если совпадений нет - уменьшаем счетчик ходов
            _movesManager.DecreaseMoves();
        }
    }
    
    private void OnItemDestroyed(int value)
    {
        _scoreManager.IncreaseScore(value);
        _audioManager.PlayItemDestroyingClip();
    }

    private void OnMovesOver()
    {
        _gameOverPopup.gameObject.SetActive(true);
        _gameOverPopup.SetFinalScore(_scoreManager.Score);
    }
    
    private void OnDestroy()
    {
        if (_itemsMover != null)
        {
            _itemsMover.ItemsSwapped -= OnItemsSwapped;
        }
        if (_gameBoardController != null)
        {
            _gameBoardController.ReFill -= OnItemsFellDown;
            _gameBoardController.ItemsDestroyed -= OnItemDestroyed;
        }
        if (_backButton != null)
        {
            _backButton.onClick.RemoveListener(LoadMenuScene);
        }
        if (_movesManager != null)
        {
            _movesManager.MovesOver -= OnMovesOver;
        }
    }
}