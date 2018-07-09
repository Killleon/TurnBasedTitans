using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PathFinder {

    public List<T> FindPath<T>(Dictionary< T, Dictionary<T, int>> edges, T origin, T destination) where T : Tile
    {
        HeapPriorityQueue<T> HPQueue = new HeapPriorityQueue<T>();
        HPQueue.Enqueue(origin, 0);

        Dictionary<T, T> cameFrom = new Dictionary<T, T>();
        cameFrom.Add(origin, default(T));
        Dictionary<T, int> costSoFar = new Dictionary<T, int>();
        costSoFar.Add(origin, 0);

        while (HPQueue.Count != 0)
        {
            T current = HPQueue.Dequeue();
            if (current.Equals(destination)) break;

            List<T> neighbours = GetNeigbours(edges, current);
            foreach (T next in neighbours)
            {
                int newCost = costSoFar[current] + edges[current][next];

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
                    costSoFar[next] = newCost;
                    int priority = newCost + Heuristic(destination, next);
                    HPQueue.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        List<T> path = new List<T>();
        if ( !cameFrom.ContainsKey(destination) )
            return path;

        path.Add(destination);
        T temp = destination;

        while ( !cameFrom[temp].Equals(origin) ) {
            T currentPathElement = cameFrom[temp];
            path.Add(currentPathElement);
            temp = currentPathElement;
        }
        return path;
    }

    protected List<T> GetNeigbours<T>(Dictionary<T, Dictionary<T, int>> edges, T node) where T : Tile {
        if (!edges.ContainsKey(node)) {
            return new List<T>();
        }
        return edges[node].Keys.ToList();
    }

    private int Heuristic(Tile a, Tile b) {

        if (a == null || b == null)
            return 0;

        return a.GetTileDistance(b);
    }
}
