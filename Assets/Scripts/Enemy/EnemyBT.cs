using System.Collections.Generic;
using BehaviorTree;

public class EnemyBT : Tree
{
    public UnityEngine.Transform[] waypoints;

    public static float speed = 10f;
    public static float fovRange = 6f;
    public static float attackRange = 1f;

    public static int[,] map = new int[,]
    {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
        {0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
        {0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, }

    };


    protected override Node SetupTree()
    {
        map = Pathfinder.AStar.GenerateMap(10, 20);

        Node root = new Selector(new List<Node>
        {
            //new task here
            // ? add sequences or task
            // TODO check base method
            new Sequence(new List<Node>
            {
               new CheckEnemyInAttackRange(transform),
               new TaskAttack(transform),
            }),
            new Sequence(new List<Node>
            {
                new CheckEnemyInFOVRange(transform),
                new TaskGoToTarget(transform, map),
            }),
            new TaskPatrol(transform, waypoints, map),
        });

        return root;
    }
}
