using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Класс, отвечающий за поиск совпадений элементов
/// </summary>
public class MatchFinder
{
    public IReadOnlyList<Item> Matches => _matches;
    private List<Item> _matches = new();
    
    /// <summary>
    /// Метод проверяет, есть ли совпадения по типам элементов с двумя соседними элементами
    /// Используется при создании элементов игрового поля
    /// </summary>
    public bool HasMatchesWithAdjacentItems(Item[,] items, Vector2 positionToCheck, Item itemToCheck)
    {
        var position = Vector2Int.RoundToInt(positionToCheck);
        
        // Проверка по горизонтали
        if (position.x > 1) // элемент послений в тройке
        {
            if(HasTypeMatches(itemToCheck,
                   items[position.x - 1, position.y], items[position.x - 2, position.y]))
            {
                return true;
            }
        }
        if (position.x < items.GetLength(1) - 2) // элемент первый в тройке
        {
            if(HasTypeMatches(itemToCheck,
                   items[position.x + 1, position.y], items[position.x + 2, position.y]))
            {
                return true;
            }
        }
        if (position.x > 0 && position.x < items.GetLength(1) - 1) // элемент посередине
        {
            if (HasTypeMatches(itemToCheck,
                    items[position.x - 1, position.y], items[position.x + 1, position.y]))
            {
                return true;
            }
        }

        // Проверка по вертикали
        if (positionToCheck.y > 1) // элемент послений в тройке
        {
            if(HasTypeMatches(itemToCheck,
                   items[position.x, position.y - 1], items[position.x, position.y - 2]))
            {
                return true;
            }
        }
        if (positionToCheck.y < items.GetLength(0) - 2) // элемент первый в тройке
        {
            if(HasTypeMatches(itemToCheck,
                   items[position.x, position.y + 1], items[position.x, position.y + 2]))
            {
                return true;
            }
        }
        if (position.y > 0 && position.y < items.GetLength(0) - 1) // элемент посередине
        {
            if (HasTypeMatches(itemToCheck,
                    items[position.x, position.y - 1], items[position.x, position.y + 1]))
            {
                return true;
            }
        }

        return false;
    }

    public bool HasMatches(Item[,] items)
    {
        _matches.Clear();
        
        var width = items.GetLength(0);
        var height = items.GetLength(1);

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                FindAdjacentMatches(items, x, y);
            }
        }
        
        if(_matches.Count > 0)
        {
            // Удаляем одинаковые элементы из списка
            _matches = _matches.Distinct().ToList();
            return true;
        }

        return false;
    }
    
    /// <summary>
    /// Метод ищет совпадения на всем игровом поле
    /// </summary>
    private void FindAdjacentMatches(Item[,] items, int x, int y)
    {
        var currentItem = items[x, y];

        // Проверка по горизонтали
        if (x > 0 && x < items.GetLength(0) - 1)
        {
            var leftItem = items[x - 1, y];
            var rightItem = items[x + 1, y];
            
            // Если есть сопадения типов с правым и левым элементом - добавляем их в список
            if (HasTypeMatches(currentItem, leftItem, rightItem))
            {
                AddMatchesToList(currentItem, leftItem, rightItem);
            }
        }

        // Проверка по вертикали
        if (y > 0 && y < items.GetLength(1) - 1)
        {
            var aboveItem = items[x, y + 1];
            var belowItem = items[x, y - 1];

            // Если есть вопадения типов с верхним и нижним элементом - добавляем их в список
            if (HasTypeMatches(currentItem, aboveItem, belowItem))
            {
                AddMatchesToList(currentItem, aboveItem, belowItem);
            }
        }
    }

    private bool HasTypeMatches(Item currentItem, Item previousItem, Item nextItem)
    {
        if (previousItem == null || nextItem == null)
        {
            return false;
        }
        
        return previousItem.Type == currentItem.Type && nextItem.Type == currentItem.Type;
    }

    private void AddMatchesToList(Item currentItem, Item previousItem, Item nextItem)
    {
        _matches.Add(currentItem);
        _matches.Add(previousItem);
        _matches.Add(nextItem);
    }
}