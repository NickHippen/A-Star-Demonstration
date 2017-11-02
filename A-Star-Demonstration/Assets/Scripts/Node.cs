using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

	public GraphLocation Location { get; set; }
	public Vector3 WorldPosition { get; set; }
	public bool Walkable { get; set; }

	public NodeConnection Connection { get; set; }
	public float CostSoFar { get; set; }
	public float EstimatedTotalCost { get; set; }
	public float Cost { get; set; }

	public Node(GraphLocation location, Vector3 worldPosition, bool walkable) {
		this.Location = location;
		this.WorldPosition = worldPosition;
		this.Walkable = walkable;
	}

	public override bool Equals(object obj) {
		return obj is Node && this.Location.Equals(((Node)obj).Location);
	}

}
