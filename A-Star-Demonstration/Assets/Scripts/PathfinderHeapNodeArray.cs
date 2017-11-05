using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PathfinderHeapNodeArray : IPathfinder {

	private Graph graph;

	private Dictionary<GraphLocation, NodeRecord> recordDict;

	public PathfinderHeapNodeArray(Graph graph) {
		this.graph = graph;
	}

	//public Path PathFindAStar(Node start, Node end, IHeuristic heuristic) {
	//	PrepareNodeDictionary();
	//	NodeRecord startRecord = recordDict[start.Location];
	//	// Intiialize the open and closed lists
	//	Heap<NodeRecord> open = new Heap<NodeRecord>(graph.MaxSize);
	//	open.Add(startRecord);
	//	startRecord.Category = ListCategory.OPEN;

	//	NodeRecord current = null;
	//	bool pathFound = false;
	//	// Iterate through processing each node
	//	while (open.Count > 0) {
	//		// Find the smallest element in the open list (using the estimatedTotalCost)
	//		current = open.Pop();
	//		current.Category = ListCategory.CLOSED;

	//		// If at the end, stop
	//		if (current.Equals(end)) {
	//			pathFound = true;
	//			break;
	//		}

	//		List<NodeConnection> connections = graph.GetConnections(current.Node);

	//		// Loop through each connection in turn
	//		foreach (NodeConnection connection in connections) {
	//			Node toNode = connection.ToNode;
	//			float toNodeCost = current.CostSoFar + connection.Cost;

	//			NodeRecord toNodeRecord = recordDict[toNode.Location];
	//			float toNodeHeuristic;
	//			if (toNodeRecord.Category == ListCategory.CLOSED) {
	//				if (toNodeRecord.CostSoFar <= toNodeCost) {
	//					continue;
	//				}

	//				// You no longer need to remove from the closed list, since it doesn't exist. It will be added to the open list
	//				// when it is found to not be in it below
	//				//closed.Remove(toNode);

	//				toNodeHeuristic = toNodeRecord.EstimatedTotalCost - toNodeRecord.CostSoFar;
	//			} else if (toNodeRecord.Category == ListCategory.OPEN) {
	//				if (toNodeRecord.CostSoFar <= toNodeCost) {
	//					continue;
	//				}
	//				toNodeHeuristic = toNodeRecord.EstimatedTotalCost - toNodeRecord.CostSoFar;
	//			} else { // UNVISITED
	//				toNodeHeuristic = heuristic.Estimate(toNode);
	//			}
	//			toNodeRecord.CostSoFar = toNodeCost;
	//			toNodeRecord.Connection = connection;
	//			toNodeRecord.EstimatedTotalCost = toNodeCost + toNodeHeuristic;

	//			if (toNodeRecord.Category != ListCategory.OPEN) {
	//				toNodeRecord.Category = ListCategory.OPEN;
	//				open.Add(toNodeRecord);
	//			} else {
	//				open.UpdateItem(toNodeRecord);
	//			}
	//		}
	//	}

	//	if (!pathFound) {
	//		// We've run out of nodes without finding the goal, so there's no solution
	//		return null;
	//	} else {
	//		// Compile the list of connections in the path
	//		List<NodeConnection> path = new List<NodeConnection>();

	//		// Work back along the path, accumulating connections
	//		while (!current.Equals(start)) {
	//			path.Add(current.Connection);
	//			current = recordDict[current.Connection.FromNode.Location];
	//		}
	//		// Reverse the path, and return it
	//		path.Reverse();
	//		return new Path(path);
	//	}
	//}

	public Path PathFindAStar(Node start, Node end, IHeuristic heuristic) {
		// Intiialize the open and closed lists
		Heap<Node> open = new Heap<Node>(graph.MaxSize);
		HashSet<Node> cleanup = new HashSet<Node>();
		open.Add(start);
		cleanup.Add(start);
		start.Category = ListCategory.OPEN;

		Node current = null;
		bool pathFound = false;
		// Iterate through processing each node
		while (open.Count > 0) {
			// Find the smallest element in the open list (using the estimatedTotalCost)
			current = open.Pop();
			current.Category = ListCategory.CLOSED;

			// If at the end, stop
			if (current.Equals(end)) {
				pathFound = true;
				break;
			}

			List<NodeConnection> connections = graph.GetConnections(current);

			// Loop through each connection in turn
			foreach (NodeConnection connection in connections) {
				Node toNode = connection.ToNode;
				float toNodeCost = current.CostSoFar + connection.Cost;

				float toNodeHeuristic;
				if (toNode.Category == ListCategory.CLOSED) {
					if (toNode.CostSoFar <= toNodeCost) {
						continue;
					}

					// You no longer need to remove from the closed list, since it doesn't exist. It will be added to the open list
					// when it is found to not be in it below
					//closed.Remove(toNode);

					toNodeHeuristic = toNode.EstimatedTotalCost - toNode.CostSoFar;
				} else if (toNode.Category == ListCategory.OPEN) {
					if (toNode.CostSoFar <= toNodeCost) {
						continue;
					}
					toNodeHeuristic = toNode.EstimatedTotalCost - toNode.CostSoFar;
				} else {
					toNodeHeuristic = heuristic.Estimate(toNode);
				}
				toNode.CostSoFar = toNodeCost;
				toNode.Connection = connection;
				toNode.EstimatedTotalCost = toNodeCost + toNodeHeuristic;

				if (toNode.Category != ListCategory.OPEN) {
					toNode.Category = ListCategory.OPEN;
					open.Add(toNode);
					cleanup.Add(toNode);
				} else {
					open.UpdateItem(toNode);
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
			Cleanup(cleanup);
			return new Path(path);
		}
	}

	void PrepareNodeDictionary() {
		Stopwatch sw = new Stopwatch();
		sw.Start();
		recordDict = new Dictionary<GraphLocation, NodeRecord>();
		foreach (Node node in graph.graph) {
			recordDict.Add(node.Location, new NodeRecord(node, null, 0f, 0f, ListCategory.UNVISITED));
		}
		sw.Stop();
		Debug.Log("Dictionary took " + sw.ElapsedMilliseconds + "ms");
	}

	private void Cleanup(HashSet<Node> cleanup) {
		//if (skipClean) {
		//	return;
		//}
		foreach (Node n in cleanup) {
			n.Clean();
		}
	}

}
