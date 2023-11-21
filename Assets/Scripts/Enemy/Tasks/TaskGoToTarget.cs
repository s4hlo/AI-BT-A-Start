using UnityEngine;
using BehaviorTree;
using System.Collections.Generic;

public class TaskGoToTarget : Node
{
    private Transform _transform;
    private Vector3 _target;
    private List<Vector3> _currentPath;
    private int[,] _map;

    private readonly float _waitTime = 0.5f; // in seconds
    private float _waitCounter = 0f;
    private bool _waiting = false;

    public TaskGoToTarget(Transform transform, int[,] map)
    {
        _transform = transform;
        _map = map;
    }

    public override NodeState Evaluate()
    {
        _currentPath.Clear();
        if (GetData("target").Equals(null) )
        {
            Debug.Log("No target, path cleared");
            _currentPath.Clear();
            state = NodeState.FAILURE;
            return state;
        }

        Transform targetTransform = (Transform)GetData("target");

        Debug.Log("target: " + targetTransform.position);

        _target = targetTransform.position;
        if (_waiting)
        {
            _waitCounter += Time.deltaTime;
            if (_waitCounter >= _waitTime)
            {
                _waiting = false;
            }
        }
        else
        {
            if (_currentPath == null || _currentPath.Count == 0)
            {
                _waitCounter = 0f;
                _currentPath = CalculatePathToTarget();
                _waiting = true;
            }
            else

            if (_currentPath != null && _currentPath.Count > 0)
            {
                Vector3 nextWaypoint = _currentPath[0];

                if (Vector3.Distance(_transform.position, nextWaypoint) < 0.01f)
                {
                    _currentPath.RemoveAt(0);
                }
                else
                {
                    _transform.position = Vector3.MoveTowards(_transform.position, nextWaypoint, EnemyBT.speed * Time.deltaTime);
                    _transform.LookAt(nextWaypoint);
                }
            }

        }
        state = NodeState.RUNNING;
        return state;
    }

    private List<Vector3> CalculatePathToTarget()
    {
        List<Vector3> path = Pathfinder.AStar.FindPath(_map, (int)_transform.position.x, (int)_transform.position.z, (int)_target.x, (int)_target.z);
        Debug.Log($"Path Go To Target: {PathToString(path)}");
        return path;
    }

    private string PathToString(List<Vector3> path)
    {
        string pathString = "";
        foreach (var node in path)
        {
            pathString += $"({node.x}, {node.z})";
        }
        return pathString;
    }
}

