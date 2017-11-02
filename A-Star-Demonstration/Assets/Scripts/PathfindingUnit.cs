using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingUnit : MonoBehaviour {

	public Graph graph;
	public Transform goal;

	public bool displayGizmos;

	private Path path;

	void Start() {
		Node fromNode = graph.FindNodeFromWorldPosition(transform.position);
		Node goalaNode = graph.FindNodeFromWorldPosition(goal.position);
		path = PathFindAStar(fromNode, goalaNode, new Heuristic(goalaNode));
		if (path == null) {
			Debug.LogWarning("Unable to find path");
		}
	}

	public Path PathFindAStar(Node start, Node end, Heuristic heuristic) {
		// Initialize the record for the start node
		//NodeRecord startRecord = new NodeRecord();
		//startRecord.node = start;
		//startRecord.connection = null;
		//startRecord.costSoFar = 0f;
		//startRecord.estimatedTotalCost = heuristic.estimate(startRecord);

		// Intiialize the open and closed lists
		List<Node> open = new List<Node>();
		open.Add(start);
		List<Node> closed = new List<Node>();

		Node current = null;
		bool pathFound = false;
		// Iterate through processing each node
		while (open.Count > 0) {
			// Find the smallest element in the open list (using the estimatedTotalCost)
			current = FindSmallestElement(open);

			// If at the end, stop
			if (current.Equals(end)) {
				pathFound = true;
				break;
			}

			List<NodeConnection> connections = graph.GetConnections(current);

			// Loop through each connection in turn
			foreach (NodeConnection connection in connections) {
				// Get the cost estimate for the end node
				Node toNode = connection.ToNode;
				float toNodeCost = current.CostSoFar + connection.Cost;

				// If the node is closed we may have to skip, or remove it from the closed list.
				float toNodeHeuristic;
				if (closed.Contains(toNode)) {
					// If we didn't find a shorter route, skip
					if (toNode.CostSoFar <= toNodeCost) {
						continue;
					}
					closed.Remove(toNode);

					// We can use the node's old cost values to calculate its heuristic without
					// calling the possibly expensive heuristic function
					toNodeHeuristic = toNode.Cost - toNode.CostSoFar;
					Debug.Log("New heuristic (from CLOSED): " + toNodeHeuristic);
				} else if (open.Contains(toNode)) {
					// Skip if the node is open and we've not found a better route

					// If our route is no better, then skip
					if (toNode.CostSoFar <= toNodeCost) {
						continue;
					}

					// We can use the node's old cost values to calculate its heuristic without
					// calling the possibly expensive heuristic function
					toNodeHeuristic = toNode.Cost - toNode.CostSoFar;
					Debug.Log("New heuristic: (from OPEN):" + toNodeHeuristic);
				} else {
					// We'll need to calculate the heuristic value using the function, since we
					// don't have an existing record to use
					toNodeHeuristic = heuristic.Estimate(toNode);
				}

				// We're here if we need to update the node
				// Update the cost, estimate and connection
				toNode.Cost = toNodeCost;
				toNode.Connection = connection;
				toNode.EstimatedTotalCost = toNodeCost + toNodeHeuristic;

				// And add it to the open list
				if (!open.Contains(toNode)) {
					open.Add(toNode);
				}
			}
			// We've finished looking at the connections for the current node, so add it to the
			// closed list and remove it from the open list
			open.Remove(current);
			closed.Add(current);
		}

		if (!pathFound) {
			// We've run out of nodes without finding the goal, so there's no solution
			return null;
		} else {
			// Compile the list of connections in the path
			List<NodeConnection> path = new List<NodeConnection>();

			// Work back along the path, accumulating connections
			while (!current.Equals(start)) {
				path.Add(current.Connection);
				current = current.Connection.FromNode;
			}
			// Reverse the path, and return it
			path.Reverse();
			return new Path(path);
		}
	}

	private Node FindSmallestElement(List<Node> list) {
		if (list.Count <= 0) {
			throw new System.Exception("Cannot find smallest element in empty list");
		}
		Node smallest = list[0];
		for (int i = 1; i < list.Count; i++) {
			Node record = list[i];
			if (record.EstimatedTotalCost < smallest.EstimatedTotalCost) {
				smallest = record;
			}
		}
		return smallest;
	}

	private NodeRecord FindRecordByNode(List<NodeRecord> list, int node) {
		foreach (NodeRecord record in list) {
			if (record.node.Equals(node)) {
				return record;
			}
		}
		throw new System.Exception("Cannot find record by the given node");
	}

	private bool ContainsRecordByNode(List<NodeRecord> list, int node) {
		foreach (NodeRecord record in list) {
			if (record.node.Equals(node)) {
				return true;
			}
		}
		return false;
	}

	void OnDrawGizmos() {
		if (displayGizmos && path != null) {
			path.DrawGizmos();
		}
	}

}
