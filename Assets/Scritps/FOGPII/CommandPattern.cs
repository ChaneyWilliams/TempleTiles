using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


[DisallowMultipleComponent]
public class CommandPattern : MonoBehaviour
{
    public float moveDistance = 1.0f;
    private readonly Stack<ICommand> commandHistory = new Stack<ICommand>();
    public int UndoCount = 0;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
            ExecuteaMoveCommand(Vector3.up * moveDistance);

        if(Input.GetKeyDown(KeyCode.A))
            ExecuteaMoveCommand(Vector3.left * moveDistance);

        if(Input.GetKeyDown(KeyCode.S))
            ExecuteaMoveCommand(Vector3.down * moveDistance);

        if(Input.GetKeyDown(KeyCode.D))
            ExecuteaMoveCommand(Vector3.right * moveDistance);

        if(Input.GetKeyDown(KeyCode.Space))
            Undo();

    }

    public void Undo()
    {
        if(commandHistory.Count == 0) return;
        ICommand command = commandHistory.Pop();
        command.Undo();
        UndoCount--;
    }

    void ExecuteaMoveCommand(Vector3 _amount)
    {
        ICommand command = new MoveCommand(transform, _amount);
        command.Execute();
        commandHistory.Push(command);
        UndoCount++;
    }
}


public interface ICommand
{
    void Execute();
    void Undo();
}

public class MoveCommand : ICommand
{
    private readonly Action execute;
    private readonly Action undo;

    private Vector3 startPos;
    
    public MoveCommand(Transform _transform, Vector3 _moveAround)
    {
        execute = () =>
        {
            startPos = _transform.position;
            _transform.position += _moveAround;
        };
        undo = () =>
        {
            _transform.position = startPos;
        };
    }

    public void Execute()
    {
        execute();
    }

    public void Undo()
    {
        undo();
    }
}

#if UNITY_EDITOR

#endif