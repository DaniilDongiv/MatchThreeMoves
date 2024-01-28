using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Класс, отвечающий за игровое поле: создание элементов, удаление элементов
/// </summary>
public class GameBoardController : MonoBehaviour
{
    public Action<Item[,]> ReFill;
    public Action<int> ItemsDestroyed;
    
    public float CellSize => _tilePrefab.transform.localScale.x;
    
    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;
    [SerializeField]
    private GameObject _tilePrefab;
    
    [SerializeField] 
    private float _tweenDuration = 0.4f;

    private Item[] _itemPrefabs;
    private Item[,] _items;
    private MatchFinder _matchFinder;

    public void Initialize(Item[] itemsPrefabs, int itemTypesNumber, MatchFinder matchFinder)
    {
        _itemPrefabs = itemsPrefabs;
        _matchFinder = matchFinder;
        
        // Изменяем размер массива с префабами элементов на переданное количество
        Array.Resize(ref _itemPrefabs, itemTypesNumber);
    }

    public Item[,] CreateGameBoard()
    {
        _items = new Item[_width, _height];
        
        // Создаем игровое поле - сетку из префабов клеточек с элементами на ней
        for(var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                var tile = Instantiate(_tilePrefab, transform);
                var tilePosition = new Vector2(x, y);
                tile.transform.position = tilePosition;
            }
        }
        
        FillGameBoard();
        return _items;
    }
    
    /// <summary>
    /// Метод, уничтожающий совпавшие элементы
    /// </summary>
    public void DestroyMatches(IReadOnlyList<Item> items)
    {
        var sequence = DOTween.Sequence();
        
        foreach (var item in items)
        {
            sequence.Join(item.Hide(_tweenDuration));
            ItemsDestroyed?.Invoke(item.Value);
        }

        // После завершения анимации исчезновения - делаем небольшую паузу и сдвигаем элементы вниз
        sequence.AppendInterval(_tweenDuration / 3);
        sequence.OnComplete(() =>
        {
            var movingSequence = MoveItemsDown();
            
            // Когда все элементы спустились - заполняем пустые клетки сверху игрового поля новыми элементами
            movingSequence.OnComplete(() =>
            {
                var sequence = FillGameBoard();
                // После заполнения вызываем событие ReFill
                sequence.OnComplete(() => ReFill?.Invoke(_items));
            });
        });
    }

    /// <summary>
    /// Метод заполняет игровое поле элементами
    /// </summary>
    private Sequence FillGameBoard()
    {
        var sequence = DOTween.Sequence();
        
        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                if (_items[x, y] == null)
                {
                    // Создаем элемент в клетке
                    SpawnItem(_items, new Vector2(x, y), sequence);
                }
            }
        }
        
        return sequence;
    }

    private void SpawnItem(Item[,] items, Vector2 position, Sequence sequence)
    {
        // Выбираем случайный элемент
        var index = Random.Range(0, _itemPrefabs.Length);
        var item = _itemPrefabs[index];
        
        // Пока есть совпадения типов элементов - генерируем новый индекс
        // Для того, чтобы на сгенерированной сетке не было элементов по 3 в ряд
        while (_matchFinder.HasMatchesWithAdjacentItems(items, position, item))
        {
            index = Random.Range(0, _itemPrefabs.Length);
            item = _itemPrefabs[index];
        }
        
        item = Instantiate(_itemPrefabs[index], transform);
        item.transform.position = position;

        // Добавляем созданный элемент в массив
        items[(int)position.x, (int)position.y] = item;
        
        sequence.Join(item.Show(_tweenDuration));
    }
    
    /// <summary>
    /// Метод перемещает элементы вниз на освободившиеся клетки (после удаления совпадений)
    /// </summary>
    private Sequence MoveItemsDown()
    {
        // Счетчик количества освободившихся столбцов
        var emptyRowsCounter = 0;
        
        var sequence = DOTween.Sequence();
        
        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                // Если клетка пустая - увеличиваем счетчик пустых столбцов на 1
                if (_items[x, y] == null)
                {
                    emptyRowsCounter++;
                } 
                // Если уже есть пустые столбцы
                else if (emptyRowsCounter > 0)
                {
                    var item = _items[x, y];
                    var itemPosition = item.transform.position;
                    
                    // Спускаем элемент на количество освободившихся клеток вниз
                    itemPosition.y -= emptyRowsCounter;

                    sequence.Join(item.transform.DOMove(itemPosition, _tweenDuration));

                    // Перемещаем элемент вниз по массиву
                    _items[x, y - emptyRowsCounter] = _items[x, y];
                    _items[x, y] = null;
                }
            }
            emptyRowsCounter = 0;
        }

        return sequence;
    }
}