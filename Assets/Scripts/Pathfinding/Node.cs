using UnityEngine;
using System;

/// <summary>
/// Represents a Node in Priority Queue.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Node<T> : IComparable {

    public T NodeObj { get; private set; }
    public int NodePriority { get; private set; }

    public Node(T _obj, int _priority) {
        NodeObj = _obj;
        NodePriority = _priority;
    }

    /// <summary>
    /// Compare priority values of 2 nodes
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int CompareTo(object obj) {
        return NodePriority.CompareTo( (obj as Node<T>).NodePriority );
    }
}
