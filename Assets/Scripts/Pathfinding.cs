using System;
using System.Collections.Generic;
using UnityEngine;


namespace Pathfinder
{
    public class AStar
    {
        public class Node
        {
            public int X, Y;
            public int G, H;
            public Node Parent;

            public Node(int x, int y)
            {
                X = x;
                Y = y;
                G = 0;
                H = 0;
                Parent = null;
            }

            public int F => G + H;
        }

        // A* algorithm implementation 
        // ? Unity - update return type to Vector3 ( before was List<Node> - this scope Node )
        public static List<Vector3> FindPath(int[,] grid, int startX, int startY, int endX, int endY)
        {
            // ? Error handling
            if (grid == null)
                return null; // No grid found
            if (endX >= grid.GetLength(0) ) {
                endX = grid.GetLength(0) - 1;
            }
            if (endY >= grid.GetLength(1) ) {
                endY = grid.GetLength(1) - 1;
            }
            if (endX < 0) {
                endX = 0;
            }
            if (endY < 0) {
                endY = 0;
            }
            if (grid[endX, endY] == 1)
                return null; // No path found
            // TODO add missing checks




            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            Node startNode = new(startX, startY);
            Node endNode = new(endX, endY);

            List<Node> openList = new();
            List<Node> closedList = new();

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                Node currentNode = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].F < currentNode.F || (openList[i].F == currentNode.F && openList[i].H < currentNode.H))
                    {
                        currentNode = openList[i];
                    }
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode.X == endNode.X && currentNode.Y == endNode.Y)
                {
                    // Found the path, reconstruct and return it
                    List<Node> path = new();
                    while (currentNode != null)
                    {
                        path.Add(currentNode);
                        currentNode = currentNode.Parent;
                    }
                    path.Reverse();

                    // ? Unity - convert path to vector3

                    List<Vector3> pathVector3 = new();
                    foreach (var node in path)
                    {
                        pathVector3.Add(new Vector3(node.X, 1, node.Y));
                    }
                    return pathVector3;
                }

                List<Node> neighbors = new();

                // Generate neighboring nodes in clockwise order
                if (currentNode.X - 1 >= 0 && grid[currentNode.X - 1, currentNode.Y] == 0)
                    neighbors.Add(new Node(currentNode.X - 1, currentNode.Y)); // Left

                // if (currentNode.X - 1 >= 0 && currentNode.Y - 1 >= 0 && grid[currentNode.X - 1, currentNode.Y - 1] == 0)
                //     neighbors.Add(new Node(currentNode.X - 1, currentNode.Y - 1)); // Top Left

                if (currentNode.Y - 1 >= 0 && grid[currentNode.X, currentNode.Y - 1] == 0)
                    neighbors.Add(new Node(currentNode.X, currentNode.Y - 1)); // Top

                // if (currentNode.X + 1 < width && currentNode.Y - 1 >= 0 && grid[currentNode.X + 1, currentNode.Y - 1] == 0)
                //     neighbors.Add(new Node(currentNode.X + 1, currentNode.Y - 1)); // Top Right

                if (currentNode.X + 1 < width && grid[currentNode.X + 1, currentNode.Y] == 0)
                    neighbors.Add(new Node(currentNode.X + 1, currentNode.Y)); // Right

                // if (currentNode.X + 1 < width && currentNode.Y + 1 < height && grid[currentNode.X + 1, currentNode.Y + 1] == 0)
                //     neighbors.Add(new Node(currentNode.X + 1, currentNode.Y + 1)); // Bottom Right

                if (currentNode.Y + 1 < height && grid[currentNode.X, currentNode.Y + 1] == 0)
                    neighbors.Add(new Node(currentNode.X, currentNode.Y + 1)); // Bottom

                // if (currentNode.X - 1 >= 0 && currentNode.Y + 1 < height && grid[currentNode.X - 1, currentNode.Y + 1] == 0)
                //     neighbors.Add(new Node(currentNode.X - 1, currentNode.Y + 1)); // Bottom Left



                foreach (var neighbor in neighbors)
                {
                    if (closedList.Contains(neighbor))
                        continue;

                    int tentativeG = currentNode.G + 1; // Assuming all moves have a cost of 1

                    if (!openList.Contains(neighbor) || tentativeG < neighbor.G)
                    {
                        neighbor.Parent = currentNode;
                        neighbor.G = tentativeG;
                        // use Manhattan distance as heuristic
                        // neighbor.H = Math.Abs(neighbor.X - endNode.X) + Math.Abs(neighbor.Y - endNode.Y);
                        // use Euclidean distance as heuristic
                        neighbor.H = (int) Math.Sqrt(Math.Pow(neighbor.X - endNode.X, 2) + Math.Pow(neighbor.Y - endNode.Y, 2));

                        if (!openList.Contains(neighbor))
                            openList.Add(neighbor);
                    }
                }
            }
            return null; // No path found
        }

        public static int[,] GenerateMap(int width, int height)
        {
            int[,] map = new int[width, height];

            //instance a plane with size width/10 x height/10 and position width/2 x height/2
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.position = new Vector3(width / 2, 0, height / 2);
            plane.transform.localScale = new Vector3(width / 10, 1, height / 10);


            GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
            foreach (GameObject obstacle in obstacles)
            {
                Vector3 position = obstacle.transform.position;
                map[(int)position.x, (int)position.z] = 1;
            }

            //print map
            string mapString = "";
            for (int y = 0; y < height; y++)
            {
                string line = "";
                for (int x = 0; x < width; x++)
                {
                    line += map[x, y];
                }
                mapString += "[" + line + "]\n";
            }

            Debug.Log(mapString);


            return map;

        }

    }

}