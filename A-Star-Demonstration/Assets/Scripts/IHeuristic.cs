using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHeuristic {

	/** Generates an estimated cost to reach the stored goal from the given node
	 */
	float Estimate(Node fromNode);

}