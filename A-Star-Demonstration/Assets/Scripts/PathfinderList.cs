using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderList : IPathfinder {

	private Graph graph;

	public PathfinderList(Graph graph) {
		this.graph = graph;
	}

	public Path PathFindAStar(Node start, Node end, IHeuristic heuristic) {
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
				Node toNode = connection.ToNode;
				if (closed.Contains(toNode)) {
					continue;
				}

				float newMovementCost = current.CostSoFar + connection.Cost;
				if (newMovementCost < toNode.CostSoFar || !open.Contains(toNode)) {
					toNode.CostSoFar = newMovementCost;
					float heuristicCost = heuristic.Estimate(toNode);
					toNode.EstimatedTotalCost = toNode.CostSoFar + heuristicCost;
					toNode.Connection = connection;

					if (!open.Contains(toNode)) {
						open.Add(toNode);
					}
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

}
