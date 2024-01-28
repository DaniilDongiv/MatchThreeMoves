using DG.Tweening;
using UnityEngine;

/// <summary>
/// Класс, отвечающий за игровой элемент
/// </summary>
public class Item : MonoBehaviour
{
    [field: SerializeField]
    public ItemType Type { get; private set; }
    [field: SerializeField] 
    public int Value { get; } = 1;

    public Tween Show(float tweenDuration)
    {
        // Анимация увеличения размера элемента
        var itemScale = transform.localScale;
        transform.localScale = Vector3.zero;
        var tween = transform.DOScale(itemScale, tweenDuration);
        return tween;
    }
    
    public Tween Hide(float tweenDuration)
    {
        // Анимация уменьшения размера элемента
        var tween = transform.DOScale(0, tweenDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // В конце анимации уничтожаем элемент
                Destroy(gameObject);
            });

        return tween;
    }
    
    public Tween Move(Vector2Int position, float tweenDuration)
    {
        // Анимация перемещения элемента
        var tween = transform.DOMove(new Vector3(position.x, position.y), tweenDuration);
        return tween;
    }
}