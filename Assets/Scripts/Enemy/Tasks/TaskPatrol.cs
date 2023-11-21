using UnityEngine;
using BehaviorTree;
using System.Collections.Generic;
using System;

public class TaskPatrol : Node
{
    private Transform _transform;
    private Transform[] _patrolPoints;
    private List<Vector3> _currentPath;
    private int _currentPatrolPointIndex = 0;
    private int _lastPatrolPointIndex = 0;
    private int[,] _map;

    private readonly float _waitTime = 0.1f; // in seconds
    private float _waitCounter = 0f;
    private bool _waiting = false;

    public TaskPatrol(Transform transform, Transform[] waypoints, int[,] map)
    {
        _transform = transform;
        _patrolPoints = waypoints;
        _map = map;
    }

    public override NodeState Evaluate()
    {
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

                Vector3 currentPatrolPoint = new(_patrolPoints[_currentPatrolPointIndex].position.x, 1, _patrolPoints[_currentPatrolPointIndex].position.z);
                Vector3 lastPatrolPoint = new(_patrolPoints[_lastPatrolPointIndex].position.x, 1, _patrolPoints[_lastPatrolPointIndex].position.z);

                float distanceToCurrentWaypoint = Vector3.Distance(_transform.position, currentPatrolPoint);
                float distanceLastToCurrent = Vector3.Distance(lastPatrolPoint, currentPatrolPoint);

                if (distanceToCurrentWaypoint > distanceLastToCurrent + 1)
                {
                    Debug.Log("unlock path out of patrol");
                    _currentPath = CalculatePathToNextWaypointWithAStar();
                }
                else
                {
                    Debug.Log("unlock path direct in patrol");
                    _currentPath = CalculatePathToNextWaypointDirect();
                }

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

    private List<Vector3> CalculatePathToNextWaypointDirect()
    {
        if (_currentPatrolPointIndex >= _patrolPoints.Length)
        {
            _lastPatrolPointIndex = _patrolPoints.Length - 1;
            _currentPatrolPointIndex = 0;

        }

        List<Vector3> path = new()
        { 
            new(_patrolPoints[_currentPatrolPointIndex].position.x, 1, _patrolPoints[_currentPatrolPointIndex].position.z)
        };

        _lastPatrolPointIndex = _currentPatrolPointIndex;

        _currentPatrolPointIndex = (_currentPatrolPointIndex + 1) % _patrolPoints.Length;

        return path;
    }

    private List<Vector3> CalculatePathToNextWaypointWithAStar()
    {
        if (_currentPatrolPointIndex >= _patrolPoints.Length)
        {
            _lastPatrolPointIndex = _patrolPoints.Length - 1;
            _currentPatrolPointIndex = 0;

        }

        Transform nextWaypoint = _patrolPoints[_currentPatrolPointIndex];
        List<Vector3> path = Pathfinder.AStar.FindPath(_map, (int)_transform.position.x, (int)_transform.position.z, (int)nextWaypoint.position.x, (int)nextWaypoint.position.z);

        Debug.Log($"Path Patrol: {PathToString(path)}");

        _lastPatrolPointIndex = _currentPatrolPointIndex;

        _currentPatrolPointIndex = (_currentPatrolPointIndex + 1) % _patrolPoints.Length;

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
