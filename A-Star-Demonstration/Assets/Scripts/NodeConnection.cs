using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeConnection {

	/*
	 * Returns the non-negative cost of the connection
	 */
	public float Cost { get; set; }

	/*
	 * Returns the node that this connection came from
	 */
	public Node FromNode { get; set; }

	/*
	 * Returns the node that this connection leads to
	 */
	public Node ToNode { get; set; }

	public NodeConnection(Node fromNode, Node toNode) {
		this.FromNode = fromNode;
		this.ToNode = toNode;
		this.Cost = CalculateNewCost();
	}

	private float CalculateNewCost() {
		//int distX = Mathf.Abs(FromNode.Location.x - ToNode.Location.x);
		//int distY = Mathf.Abs(FromNode.Location.y - ToNode.Location.y);

		//if (distX > distY) {
		//	return 14 * distY + 10 * (distX - distY);
		//} else {
		//	return 14 * distX + 10 * (distY - distX);
		//}
		return new Heuristic(ToNode).Estimate(FromNode);
	}

	public void DrawGizmos() {
		Gizmos.DrawCube(FromNode.WorldPosition + Vector3.up, Vector3.one);
	}

}
