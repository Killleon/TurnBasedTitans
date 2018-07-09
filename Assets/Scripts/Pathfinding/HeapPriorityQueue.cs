using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///  Based on http://www.eecs.wsu.edu/~ananth/CptS223/Lectures/heaps.pdf
///  and https://gist.github.com/trevordixon/10401462
/// </summary>
/// <typeparam name="T"></typeparam>
public class HeapPriorityQueue<T> {

    private List<Node<T>> Queue;

    public HeapPriorityQueue()
    {
        Queue = new List<Node<T>>();
    }

    public int Count { get {return Queue.Count;} }

    public void Enqueue(T _newNode, int _priority)
    {
        Queue.Add( new Node<T>(_newNode, _priority) );

        int ci = Queue.Count - 1; // Child Index. Since the first node can never be a child, we -1.

        // Binary Search
        while (ci > 0)
        {
            int pi = (ci - 1) / 2; // Parent Index. Again, first node is not considered so -1.
            if (Queue[ci].CompareTo(Queue[pi]) >= 0)
                break;
            Node<T> tmp = Queue[ci];
            Queue[ci] = Queue[pi];
            Queue[pi] = tmp;
            ci = pi;
        }
    }

    /// <summary>
    /// Returns the front item by percolating down the binary tree.
    /// </summary>
    /// <returns></returns>
    public T Dequeue()
    {
        int li = Queue.Count - 1; // List Index
        Node<T> frontItem = Queue[0];
        Queue[0] = Queue[li];
        Queue.RemoveAt(li);

        --li;
        int pi = 0;
        while (true)
        {
            int ci = pi * 2 + 1; // This equates to the left child of pi
            if (ci > li) break;
            int rc = ci + 1;    // +1 to find the right child
            if (rc <= li && Queue[rc].CompareTo(Queue[ci]) < 0)
                ci = rc;
            if (Queue[pi].CompareTo(Queue[ci]) <= 0) break;

            Node<T> tmp = Queue[pi];
            Queue[pi] = Queue[ci];
            Queue[ci] = tmp;
            pi = ci;
        }
        return frontItem.NodeObj;
    }
}
