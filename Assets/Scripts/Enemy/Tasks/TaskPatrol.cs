using UnityEngine;
using BehaviorTree;
using System.Collections.Generic;

public class TaskPatrol : Node
{
    private Transform _transform;
    private Transform[] _marks;
    private List<Vector3> _currentPath;
    private int _currentWaypointIndex = 0;
    private int[,] _map;

    private float _waitTime = 1f; // in seconds
    private float _waitCounter = 0f;
    private bool _waiting = false;

    public TaskPatrol(Transform transform, Transform[] waypoints, int[,] map)
    {
        _transform = transform;
        _marks = waypoints;
        _map = map;
    }

    public override NodeState Evaluate()
    {
        if (_waiting)
        {
            Debug.Log("WAITING");
            _waitCounter += Time.deltaTime;
            if (_waitCounter >= _waitTime)
            {
                _waiting = false;
                _currentPath = CalculatePathToNextWaypoint();
            }
        }
        else
        {
            if (_currentPath == null || _currentPath.Count == 0)
            {
                _waitCounter = 0f;
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

    private List<Vector3> CalculatePathToNextWaypoint()
    {
        if (_currentWaypointIndex >= _marks.Length)
        {
            _currentWaypointIndex = 0;

        }

        Transform nextWaypoint = _marks[_currentWaypointIndex];
        List<Vector3> path = Pathfinder.AStar.FindPath(_map, (int)_transform.position.x, (int)_transform.position.z, (int)nextWaypoint.position.x, (int)nextWaypoint.position.z);

        Debug.Log($"Path: {PathToString(path)}");

        _currentWaypointIndex = (_currentWaypointIndex + 1) % _marks.Length;

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
