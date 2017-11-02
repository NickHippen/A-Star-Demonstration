using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heuristic {

	/** Stores the goal node that this heuristic is estimating for
	 */
	private Node GoalNode { get; set; }

	/** Constructor, takes a goal node for estimating
	 */
	public Heuristic(Node goalNode) {
		this.GoalNode = goalNode;
	}

	/** Generates an estimated cost to reach the stored goal from the given node
	 */
	public float estimate(Node fromNode) {
		int distX = Mathf.Abs(fromNode.Location.x - GoalNode.Location.x);
		int distY = Mathf.Abs(fromNode.Location.y - GoalNode.Location.y);

		if (distX > distY) {
			return 14 * distY + 10 * (distX - distY);
		} else {
			return 14 * distX + 10 * (distY - distX);
		}
	}

}
