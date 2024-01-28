using UnityEngine;

/// <summary>
/// Класс, возвращающий индекс элемента на игровом поле по его позиции
/// </summary>
public class GameBoardIndexProvider
{
    private GameBoardController _gameBoardController;

    public GameBoardIndexProvider(GameBoardController gameBoardController)
    {
        _gameBoardController = gameBoardController;
    }
    
    public Vector2Int GetIndex(Vector3 worldPosition)
    {
        // Переводим мировую позицию в локальную позицию игрового поля
        var tilePositionInMap = _gameBoardController.transform.InverseTransformPoint(worldPosition);

        // Высчитываем половину размера клетки
        var halfCellSize = _gameBoardController.CellSize / 2;
        
        // Получаем целочисленные координаты клетки
        var x = Mathf.FloorToInt(tilePositionInMap.x + halfCellSize);
        var y = Mathf.FloorToInt(tilePositionInMap.y + halfCellSize);

        return new Vector2Int(x, y);
    }
}