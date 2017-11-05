using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EuclideanHeuristic : IHeuristic {

	/** Stores the goal node that this heuristic is estimating for
	 */
	private Node GoalNode { get; set; }

	/** Constructor, takes a goal node for estimating
	 */
	public EuclideanHeuristic(Node goalNode) {
		this.GoalNode = goalNode;
	}

	/** Generates an estimated cost to reach the stored goal from the given node
	 */
	public float Estimate(Node fromNode) {
		return Vector3.Distance(fromNode.WorldPosition, GoalNode.WorldPosition);
	}

}
