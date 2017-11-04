using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PathfindingUnit : MonoBehaviour {

	public Graph graph;
	public Transform goal;
	public bool moving;
	public float speed = 5f;

	public bool displayGizmos;

	private Path path;
	private Node movementNode;

	void Start() {
		Node fromNode = graph.FindNodeFromWorldPosition(transform.position);
		Node goalaNode = graph.FindNodeFromWorldPosition(goal.position);
		Stopwatch timer = new Stopwatch();
		timer.Start();
		path = PathFindAStar(fromNode, goalaNode, new Heuristic(goalaNode));
		timer.Stop();
		Debug.Log("Pathfinding took: " + timer.ElapsedMilliseconds + "ms");
		if (path == null) {
			Debug.LogWarning("Unable to find path");
		} else {
			StartCoroutine("FollowPath");
		}
	}

	public IEnumerator FollowPath() {
		foreach (NodeConnection connection in path.PathConnections) {
			Node currentNode = graph.FindNodeFromWorldPosition(transform.position);
			while (!currentNode.Equals(connection.ToNode)) {
				transform.position = Vector3.MoveTowards(transform.position, connection.ToNode.WorldPosition, speed * Time.deltaTime);
				currentNode = graph.FindNodeFromWorldPosition(transform.position);
				yield return null;
			}
		}
		Debug.Log("Finished path");
	}

	public Path PathFindAStar(Node start, Node end, Heuristic heuristic) {
		// Initialize the record for the start node
		//NodeRecord startRecord = new NodeRecord();
		//startRecord.node = start;
		//startRecord.connection = null;
		//startRecord.costSoFar = 0f;
		//startRecord.estimatedTotalCost = heuristic.estimate(startRecord);

		// Intiialize the open and closed lists
		//List<Node> open = new List<Node>();
		Heap<Node> open = new Heap<Node>(graph.MaxSize);
		open.Add(start);
		HashSet<Node> closed = new HashSet<Node>();

		Node current = null;
		bool pathFound = false;
		// Iterate through processing each node
		while (open.Count > 0) {
			// Find the smallest element in the open list (using the estimatedTotalCost)
			current = open.RemoveFirst();
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

	//public Path PathFindAStar(Node start, Node end, Heuristic heuristic) {
	//	// Initialize the record for the start node
	//	//NodeRecord startRecord = new NodeRecord();
	//	//startRecord.node = start;
	//	//startRecord.connection = null;
	//	//startRecord.costSoFar = 0f;
	//	//startRecord.estimatedTotalCost = heuristic.estimate(startRecord);

	//	// Intiialize the open and closed lists
	//	List<Node> open = new List<Node>();
	//	open.Add(start);
	//	List<Node> closed = new List<Node>();

	//	Node current = null;
	//	bool pathFound = false;
	//	// Iterate through processing each node
	//	while (open.Count > 0) {
	//		// Find the smallest element in the open list (using the estimatedTotalCost)
	//		current = FindSmallestElement(open);

	//		// If at the end, stop
	//		if (current.Equals(end)) {
	//			pathFound = true;
	//			break;
	//		}

	//		List<NodeConnection> connections = graph.GetConnections(current);

	//		// Loop through each connection in turn
	//		foreach (NodeConnection connection in connections) {
	//			Node toNode = connection.ToNode;
	//			if (closed.Contains(toNode)) {
	//				continue;
	//			}

	//			float newMovementCost = current.CostSoFar + connection.Cost;
	//			if (newMovementCost < toNode.CostSoFar || !open.Contains(toNode)) {
	//				toNode.CostSoFar = newMovementCost;
	//				float heuristicCost = heuristic.Estimate(toNode);
	//				toNode.EstimatedTotalCost = toNode.CostSoFar + heuristicCost;
	//				toNode.Connection = connection;

	//				if (!open.Contains(toNode)) {
	//					open.Add(toNode);
	//				}
	//			}
	//		}
	//		// We've finished looking at the connections for the current node, so add it to the
	//		// closed list and remove it from the open list
	//		open.Remove(current);
	//		closed.Add(current);
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
	//			current = current.Connection.FromNode;
	//		}
	//		// Reverse the path, and return it
	//		path.Reverse();
	//		return new Path(path);
	//	}
	//}

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
