using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {

	public GraphLocation Location { get; set; }
	public Vector3 WorldPosition { get; set; }
	public bool Walkable { get; set; }
	public int ClusterID { get; set; }

	public NodeConnection Connection { get; set; }
	public float CostSoFar { get; set; }
	public float EstimatedTotalCost { get; set; }
	public float Cost { get; set; }
	public float HeuristicCost { get; set; }
	public ListCategory Category { get; set; }

	public int HeapIndex { get; set; }

	public Node(GraphLocation location, Vector3 worldPosition, bool walkable) {
		this.Location = location;
		this.WorldPosition = worldPosition;
		this.Walkable = walkable;
		this.Category = ListCategory.UNVISITED;
	}

	public int CompareTo(Node other) {
		int compare = EstimatedTotalCost.CompareTo(other.EstimatedTotalCost);
		if (compare == 0) {
			compare = HeuristicCost.CompareTo(other.HeuristicCost);
		}
		return -compare;
	}

	public void Clean() {
		this.Connection = null;
		this.CostSoFar = 0f;
		this.EstimatedTotalCost = 0f;
		this.Cost = 0f;
		this.HeuristicCost = 0f;
		this.Category = ListCategory.UNVISITED;
		this.HeapIndex = 0;
	}

}

public class NodeComparator : IComparer<Node> {

	public int Compare(Node x, Node y) {
		return x.EstimatedTotalCost.CompareTo(y.EstimatedTotalCost);
	}

}

public enum ListCategory {
	OPEN, CLOSED, UNVISITED
}