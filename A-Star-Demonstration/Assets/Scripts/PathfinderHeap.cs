using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderHeap : IPathfinder {

	private Graph graph;
	private bool skipClean;

	public PathfinderHeap(Graph graph) {
		this.graph = graph;
	}

	public PathfinderHeap(Graph graph, bool skipClean) {
		this.graph = graph;
		this.skipClean = skipClean;
	}

	public Path PathFindAStar(Node start, Node end, IHeuristic heuristic) {
		// Intiialize the open and closed lists
		Heap<Node> open = new Heap<Node>(graph.MaxSize);
		open.Add(start);
		HashSet<Node> closed = new HashSet<Node>();
		HashSet<Node> cleanup = new HashSet<Node>();

		Node current = null;
		bool pathFound = false;
		// Iterate through processing each node
		while (open.Count > 0) {
			// Find the smallest element in the open list (using the estimatedTotalCost)
			current = open.Pop();
			closed.Add(current);

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
					toNode.HeuristicCost = heuristicCost;
					toNode.EstimatedTotalCost = toNode.CostSoFar + heuristicCost;
					toNode.Connection = connection;
					cleanup.Add(toNode);

					if (!open.Contains(toNode)) {
						open.Add(toNode);
					} else {
						open.UpdateItem(toNode);
					}
				}
			}
		}

		if (!pathFound) {
			// We've run out of nodes without finding the goal, so there's no solution
			Cleanup(cleanup);
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
			Debug.Log("Done");
			Cleanup(cleanup);
			return new Path(path);
		}
	}

	private void Cleanup(HashSet<Node> cleanup) {
		if (skipClean) {
			return;
		}
		foreach (Node n in cleanup) {
			n.Clean();
		}
	}

}
