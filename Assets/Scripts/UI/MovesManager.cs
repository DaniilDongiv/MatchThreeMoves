using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Менеджер количества шагов
/// </summary>
public class MovesManager : MonoBehaviour
{
    public Action MovesOver;
    public UnityEvent<int> MovesDecreased;
    
    [SerializeField]
    private int _moves = 5;

    private void Awake()
    {
        MovesDecreased?.Invoke(_moves);
    }

    public void DecreaseMoves()
    {
        if (_moves <= 0)
        {
            return;
        }
        
        MovesDecreased?.Invoke(--_moves);
        
        if (_moves <= 0)
        {
            MovesOver.Invoke();
        }
    } 
}