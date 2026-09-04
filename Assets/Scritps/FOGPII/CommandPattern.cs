using System;
using UnityEngine;


[DisallowMultipleComponent]
public class CommandPattern : MonoBehaviour
{
    public float moveDistance = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


public interface ICommand
{
    void Execute();
    void UnDo();
}

public class MoveCommand : ICommand
{
    private readonly Action executeAction;
    public void Execute()
    {
        
    }

    public void UnDo()
    {
        
    }
}