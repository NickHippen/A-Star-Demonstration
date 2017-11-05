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

	public bool list = false;
	public bool heapEuclidean = true;
	public bool heapCluster = false;
	public bool nodeArray = false;

	public int repeatCount = 1;

	private Path path;
	private Node movementNode;

	void Start() {
		FindPath();
		//FollowPath();
	}

	public void FindPath() {
		StopCoroutine("FollowPathCoroutine");
		Node fromNode = graph.FindNodeFromWorldPosition(transform.position);
		Node goalNode = graph.FindNodeFromWorldPosition(goal.position);
		EuclideanHeuristic euclideanH = new EuclideanHeuristic(goalNode);

		Stopwatch timer = new Stopwatch();
		if (list) {
			// List + Euclidean
			timer.Start();
			for (int i = 0; i < repeatCount; i++) {
				path = new PathfinderList(graph).PathFindAStar(fromNode, goalNode, euclideanH);
			}
			timer.Stop();
			Debug.Log("[List + Euclidean] Pathfinding took: " + timer.ElapsedMilliseconds + "ms");
			timer.Reset();
		}

		if (heapEuclidean) {
			// Heap + Euclidean
			timer.Start();
			for (int i = 0; i < repeatCount; i++) {
				path = new PathfinderHeap(graph, true).PathFindAStar(fromNode, goalNode, euclideanH);
			}
			timer.Stop();
			Debug.Log("[Heap + Euclidean] Pathfinding took: " + timer.ElapsedMilliseconds + "ms");
			timer.Reset();
		}

		if (heapCluster) {
			// Heap + Cluster
			ClusterHeuristic clusterH = new ClusterHeuristic(goalNode, graph);
			timer.Start();
			for (int i = 0; i < repeatCount; i++) {
				path = new PathfinderHeap(graph, true).PathFindAStar(fromNode, goalNode, clusterH);
			}
			timer.Stop();
			Debug.Log("[Heap + Cluster] Pathfinding took: " + timer.ElapsedMilliseconds + "ms");
			timer.Reset();
		}

		if (nodeArray) {
			// Heap + Euclidean + Node Array
			timer.Start();
			for (int i = 0; i < repeatCount; i++) {
				path = new PathfinderHeapNodeArray(graph).PathFindAStar(fromNode, goalNode, euclideanH);
			}
			timer.Stop();
			Debug.Log("[Heap + Euclidean + Node Array] Pathfinding took: " + timer.ElapsedMilliseconds + "ms");
			timer.Reset();
		}
	}

	public void FollowPath() {
		if (path == null) {
			Debug.LogWarning("Unable to find path");
		} else {
			StopCoroutine("FollowPathCoroutine");
			StartCoroutine("FollowPathCoroutine");
		}
	}

	private IEnumerator FollowPathCoroutine() {
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

	void OnDrawGizmos() {
		if (displayGizmos && path != null) {
			path.DrawGizmos();
		}
	}

}
