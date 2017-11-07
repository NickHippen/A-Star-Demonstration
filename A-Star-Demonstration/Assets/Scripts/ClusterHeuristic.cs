using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterHeuristic : IHeuristic {

	/** Stores the goal node that this heuristic is estimating for
	 */
	private Node GoalNode { get; set; }
	/** [Start,End]=pathCost
	 */
	private float[,] clusterMap;
	private Graph graph;

	/** Constructor, takes a goal node for estimating
	 */
	public ClusterHeuristic(Node goalNode, Graph graph) {
		this.GoalNode = goalNode;
		this.graph = graph;
		PrepareMap(graph);
		PrintClusterMap();
	}

	/** Generates an estimated cost to reach the stored goal from the given node
	 */
	public float Estimate(Node fromNode) {
		// Euclidean distance if within the same cluster
		if (fromNode.ClusterID == GoalNode.ClusterID || fromNode.ClusterID == 0 || GoalNode.ClusterID == 0) {
			return new EuclideanHeuristic(GoalNode).Estimate(fromNode);
		}
		return GetMappedDistance(fromNode);
	}

	private float GetMappedDistance(Node fromNode) {
		try {
			return this.clusterMap[fromNode.ClusterID-1, GoalNode.ClusterID-1];
		} catch (Exception e) {
			Debug.Log(fromNode.ClusterID + "," + GoalNode.ClusterID);
			throw e;
		}
		
	}

	private void PrepareMap(Graph graph) {
		Debug.Log("Preparing for " + graph.clusterCenters.Count + " clusters");
		this.clusterMap = new float[graph.clusterCenters.Count, graph.clusterCenters.Count];
		IPathfinder pathfinder = new PathfinderHeap(graph);
		foreach (int clusterID_Start in graph.clusterCenters.Keys) {
			foreach (int clusterID_End in graph.clusterCenters.Keys) {
				if (clusterID_Start == clusterID_End) {
					continue;
				}
				Node start = graph.clusterCenters[clusterID_Start];
				Node end = graph.clusterCenters[clusterID_End];
				Path path = pathfinder.PathFindAStar(start, end, new EuclideanHeuristic(end));
				float pathCost = 0f;
				if (path != null) {
					foreach (NodeConnection connection in path.PathConnections) {
						pathCost += connection.Cost;
					}
				} else {
					Debug.Log("Can't reach cluster " + clusterID_End + " from " + clusterID_Start);
					pathCost = -1f;
				}
				if (clusterID_Start > 0 && clusterID_End > 0) {
					this.clusterMap[clusterID_Start - 1, clusterID_End - 1] = pathCost;
				}
			}
		}
	}

	public bool CanReach(Node fromNode) {
		Debug.Log(fromNode.ClusterID + ", " + GoalNode.ClusterID);
		return this.clusterMap[fromNode.ClusterID-1, GoalNode.ClusterID-1] != -1f;
	}

	public void PrintClusterMap() {
		string printMap = "-\t";
		for (int i = 1; i <= graph.clusterCenters.Count; i++) {
			printMap += i + "\t";
		}
		printMap += "\n";
		for (int x = 0; x < graph.clusterCenters.Count; x++) {
			printMap += (x+1) + "\t";
			for (int y = 0; y < graph.clusterCenters.Count; y++) {
				printMap += this.clusterMap[x, y] + "\t";
			}
			printMap += "\n";
		}
		Debug.Log(printMap);
	}

}
