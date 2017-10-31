using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

	public List<NodeConnection> PathFindAStar(Graph graph, int start, int end, Heuristic heuristic) {
		// Initialize the record for the start node
		NodeRecord startRecord = new NodeRecord();
		startRecord.node = start;
		startRecord.connection = null;
		startRecord.costSoFar = 0f;
		startRecord.estimatedTotalCost = heuristic.estimate(startRecord);

		// Intiialize the open and closed lists
		List<NodeRecord> open = new List<NodeRecord>();
		open.Add(startRecord);
		List<NodeRecord> closed = new List<NodeRecord>();

		NodeRecord current = new NodeRecord();

		// Iterate through processing each node
		while (open.Count > 0) {
			// Find the smallest element in the open list (using the estimatedTotalCost)
			current = FindSmallestElement(open);

			// If at the end, stop
			if (current.node.Equals(end)) {
				break;
			}

			List<NodeConnection> connections = graph.GetConnections(current);

			// Loop through each connection in turn
			foreach (NodeConnection connection in connections) {
				// Get the cost stimate for the end node
				NodeRecord endNode = connection.ToNode;
				float endNodeCost = current.costSoFar + connection.Cost;

				// If the node is closed we may have to skip, or remove it from the closed list.
				NodeRecord endNodeRecord;
				float endNodeHeuristic;
				if (ContainsRecordByNode(closed, endNode.node)) {
					// Here we find the record in the closed list corresponding to the endNode
					endNodeRecord = FindRecordByNode(closed, endNode.node);

					// If we didn't find a shorter route, skip
					if (endNodeRecord.costSoFar <= endNodeCost) {
						continue;
					}
					closed.Remove(endNodeRecord);

					// We can use the node's old cost values to calculate its heuristic without
					// calling the possibly expensive heuristic function
					endNodeHeuristic = endNodeRecord.cost - endNodeRecord.costSoFar;
				} else if (ContainsRecordByNode(open, endNode.node)) {
					// Skip if the node is open and we've not found a better route

					// Here we find the record in the open list corresponding to the endNode.
					endNodeRecord = FindRecordByNode(open, endNode.node);

					// If our route is no better, then skip
					if (endNodeRecord.costSoFar <= endNodeCost) {
						continue;
					}

					// We can use the node's old cost values to calculate its heuristic without
					// calling the possibly expensive heuristic function
					endNodeHeuristic = endNodeRecord.costSoFar - endNodeRecord.costSoFar;
				} else {
					// We have an unvisited node; make a record for it
					endNodeRecord = new NodeRecord();
					endNodeRecord.node = endNode.node;

					// We'll need to calculate the heuristic value using the function, since we
					// don't have an existing record to use
					endNodeHeuristic = heuristic.estimate(endNode);
				}

				// We're here if we need to update the node
				// Update the cost, estimate and connection
				endNodeRecord.cost = endNodeCost;
				endNodeRecord.connection = connection;
				endNodeRecord.estimatedTotalCost = endNodeCost + endNodeHeuristic;

				// And add it to the open list
				if (!ContainsRecordByNode(open, endNode.node)) {
					open.Add(endNodeRecord);
				}
			}
			// We've finished looking at the connections for the current node, so add it to the
			// closed list and remove it from the open list
			open.Remove(current);
			closed.Add(current);
		}

		if (!current.node.Equals(end)) {
			// We've run out of nodes without finding the goal, so there's no solution
			return null;
		} else {
			// Compile the list of connections in the path
			List<NodeConnection> path = new List<NodeConnection>();

			// Work back along the path, accumulating connections
			while (!current.node.Equals(start)) {
				path.Add(current.connection);
				current = current.connection.FromNode;
			}
			// Reverse the path, and return it
			path.Reverse();
			return path;
		}
	}

	private NodeRecord FindSmallestElement(List<NodeRecord> list) {
		if (list.Count <= 0) {
			throw new System.Exception("Cannot find smallest element in empty list");
		}
		NodeRecord smallest = list[0];
		for (int i = 1; i < list.Count; i++) {
			NodeRecord record = list[i];
			if (record.estimatedTotalCost < smallest.estimatedTotalCost) {
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

}

//public struct NodeRecord {

//	public Node node;
//	public NodeConnection connection;
//	public float costSoFar;
//	public float estimatedTotalCost;
//	public float cost;

//}
