using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeRecord : IHeapItem<NodeRecord> {

	public Node Node { get; set; }
	public NodeConnection Connection { get; set; }
	public float CostSoFar { get; set; }
	public float EstimatedTotalCost { get; set; }
	public float HeuristicCost { get; set; }
	public ListCategory Category { get; set; }

	public int HeapIndex { get; set; }

	public NodeRecord(Node node, NodeConnection connection, float costSoFar, float estimatedTotalCost, ListCategory category) {
		this.Node = node;
		this.Connection = connection;
		this.CostSoFar = costSoFar;
		this.EstimatedTotalCost = estimatedTotalCost;
		this.Category = category;
	}

	public int CompareTo(NodeRecord other) {
		int compare = EstimatedTotalCost.CompareTo(other.EstimatedTotalCost);
		if (compare == 0) {
			compare = HeuristicCost.CompareTo(other.HeuristicCost);
		}
		return -compare;
	}
}

public class NodeRecordComparator : IComparer<NodeRecord> {

	public int Compare(NodeRecord x, NodeRecord y) {
		return x.EstimatedTotalCost.CompareTo(y.EstimatedTotalCost);
	}

}
