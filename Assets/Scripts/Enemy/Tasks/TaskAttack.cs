using UnityEngine;

using BehaviorTree;

public class TaskAttack : Node
{
    // private Animator _animator;

    private Transform _lastTarget;
    private PlayerManager _enemyManager;

    private float _attackTime = 1f;
    private float _attackCounter = 0f;

    public TaskAttack(Transform transform)
    {
    }

    public override NodeState Evaluate()
    {
        // Debug.Log("START EVALUATE TASK ATTACK");
        Transform target = (Transform)GetData("target");
        if (target != _lastTarget)
        {
            _enemyManager = target.GetComponent<PlayerManager>();
            _lastTarget = target;
        }

        _attackCounter += Time.deltaTime;
        if (_attackCounter >= _attackTime)
        {
            Debug.Log("ATTACK");
            bool enemyIsDead = _enemyManager.TakeHit();
            if (enemyIsDead)
            {
                ClearData("target");
            }
            else
            {
                _attackCounter = 0f;
            }
        }

        state = NodeState.RUNNING;
        return state;
    }

}
