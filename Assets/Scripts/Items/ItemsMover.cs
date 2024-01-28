using System;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Класс, отвечающий за перемещение элементов мышью
/// </summary>
public class ItemsMover : MonoBehaviour
{
    public Action<Item[,]> ItemsSwapped;
    
    [SerializeField]
    private float _swapDuration = 0.5f;
    
    private Item[,] _items;
    private GameBoardIndexProvider _gameBoardIndexProvider;
    
    private Vector2Int _firstItemPosition;
    private Vector2Int _secondItemPosition;

    public void Initialize(Item[,] items, GameBoardIndexProvider gameBoardIndexProvider)
    {
        _items = items;
        _gameBoardIndexProvider = gameBoardIndexProvider;
    }

    private void Update()
    {
        // Когда ЛКМ нажата - получаем позицию первого элемента
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                _firstItemPosition = _gameBoardIndexProvider.GetIndex(mousePosition);
            }
        }

        // Когда ЛКМ отпущена - получаем позицию второго элемента
        if (Input.GetMouseButtonUp(0))
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                _secondItemPosition = _gameBoardIndexProvider.GetIndex(mousePosition);
                
                // Проверяем, что элементы в соседних клетках
                if (AreItemsAdjacent(_firstItemPosition, _secondItemPosition))
                {
                    SwapItems();
                }
            }
        }
    }

    public void ReSwapItems()
    {
        SwapItems(true);
    }
    
    /// <summary>
    /// Метод проверяет, что переданные элементы располагаются в соседних клетках
    /// (исключая диагонали)
    /// </summary>
    private bool AreItemsAdjacent(Vector2Int firstItemPosition, Vector2Int secondItemPosition)
    {
        var xDifference = Mathf.Abs(firstItemPosition.x - secondItemPosition.x);
        var yDifference = Mathf.Abs(firstItemPosition.y - secondItemPosition.y);

        return xDifference + yDifference == 1;
    }

    private void SwapItems(bool isReSwap = false)
    {
        var firstItem = _items[_firstItemPosition.x, _firstItemPosition.y];
        var secondItem = _items[_secondItemPosition.x, _secondItemPosition.y];

        // Меняем местами элементы в массиве _items:
        _items[_firstItemPosition.x, _firstItemPosition.y] = secondItem;
        _items[_secondItemPosition.x, _secondItemPosition.y] = firstItem;

        var movingSequence = DOTween.Sequence();
        movingSequence.Join(firstItem.Move(_secondItemPosition, _swapDuration));
        movingSequence.Join(secondItem.Move(_firstItemPosition, _swapDuration));
        movingSequence.OnComplete(() =>
        {
            if (!isReSwap)
            {
                ItemsSwapped?.Invoke(_items);
            }
        });

        // Сохраняем новые позиции в переменных:
        (_firstItemPosition, _secondItemPosition) = (_secondItemPosition, _firstItemPosition);
    }
}