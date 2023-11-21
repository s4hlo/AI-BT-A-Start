using UnityEngine;

using BehaviorTree;

public class CheckEnemyInFOVRange : Node
{
    private static int _enemyLayerMask = 1 << 6;

    private Transform _transform;

    public CheckEnemyInFOVRange(Transform transform)
    {
        _transform = transform;
    }

    public override NodeState Evaluate()
    {
            Collider[] colliders = Physics.OverlapSphere(
                _transform.position, EnemyBT.fovRange, _enemyLayerMask);

            if (colliders.Length > 0)
            {

                parent.parent.SetData("target", colliders[0].transform);
                parent.parent.SetData("isChasing", true);
                state = NodeState.SUCCESS;
                return state;
            }


            //return something in parent to check that there is no target( must be an object)
            parent.parent.SetData("isChasing", false);

            state = NodeState.SUCCESS;
            return state;
    }

}
