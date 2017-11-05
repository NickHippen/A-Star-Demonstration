using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Graph : MonoBehaviour {

	private readonly Dictionary<GraphLocation, List<NodeConnection>> connectionDict = new Dictionary<GraphLocation, List<NodeConnection>>();

	public LayerMask unwalkableLayer;
	public Vector2 graphWorldSize = new Vector2(100, 100);
	public float nodeRadius = 0.5f;

	public bool displayGizmos = false;
	public bool displayOnlyChecked = true;

	private float nodeDiameter;
	private int graphCountX;
	private int graphCountY;
	public Node[,] graph;
	public Dictionary<int, Node> clusterCenters = new Dictionary<int, Node>();

	void Awake() {
		nodeDiameter = nodeRadius * 2f;
		graphCountX = Mathf.RoundToInt(graphWorldSize.x / nodeDiameter);
		graphCountY = Mathf.RoundToInt(graphWorldSize.y / nodeDiameter);
		CreateGraph();
	}

	public int MaxSize {
		get {
			return graphCountX * graphCountY;
		}
	}

	/** Returns a list of connections outgoing from "fromNode"
	 */
	public List<NodeConnection> GetConnections(Node fromNode) {
		return connectionDict[fromNode.Location];
	}

	public void CreateGraph() {
		graph = new Node[graphCountX, graphCountY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * graphWorldSize.x / 2 - Vector3.forward * graphWorldSize.y / 2;

		for (int x = 0; x < graphCountX; x++) {
			for (int y = 0; y < graphCountY; y++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableLayer));

				//int clusterID = 0;
				//Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
				//RaycastHit hit;
				//if (Physics.Raycast(ray, out hit, 100)) {
				//	ClusterID clusterIDObj = hit.transform.GetComponent<ClusterID>();
				//	if (clusterIDObj != null) {
				//		clusterID = clusterIDObj.ID;
				//		clusterCenters.Add(clusterID, FindNodeFromWorldPosition(hit.transform.position));
				//	}
				//}
				
				graph[x, y] = new Node(new GraphLocation(x, y), worldPoint, walkable);
			}
		}
		StoreConnections();
		SetupClusters();
	}

	private void SetupClusters() {
		for (int x = 0; x < graphCountX; x++) {
			for (int y = 0; y < graphCountY; y++) {
				Node node = graph[x, y];
				int clusterID = 0;
				Ray ray = new Ray(node.WorldPosition + Vector3.up * 50, Vector3.down);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 100)) {
					ClusterID clusterIDObj = hit.transform.GetComponent<ClusterID>();
					if (clusterIDObj != null) {
						clusterID = clusterIDObj.ID;
						if (!clusterCenters.ContainsKey(clusterID)) {
							if (hit.transform.childCount > 0) {
								clusterCenters.Add(clusterID, FindNodeFromWorldPosition(hit.transform.GetChild(0).position));
							} else {
								clusterCenters.Add(clusterID, FindNodeFromWorldPosition(hit.transform.position));
							}
						}
					}
				}
				node.ClusterID = clusterID;
			}
		}
	}

	private void StoreConnections() {
		for (int x = 0; x < graphCountX; x++) {
			for (int y = 0; y < graphCountY; y++) {
				Node node = graph[x, y];
				connectionDict.Add(node.Location, FindConnections(node));
			}
		}
	}

	public List<NodeConnection> FindConnections(Node fromNode) {
		List<NodeConnection> connections = new List<NodeConnection>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0) {
					continue;
				}
				int checkX = fromNode.Location.x + x;
				int checkY = fromNode.Location.y + y;

				if (checkX >= 0 && checkX < graphCountX && checkY >= 0 && checkY < graphCountY) { // Within bounds
					Node toNode = graph[checkX, checkY];
					// Only add if the connection is walkable
					if (toNode.Walkable) {
						connections.Add(new NodeConnection(fromNode, toNode));
					}
				}
			}
		}
		return connections;
	}

	public Node FindNodeFromWorldPosition(Vector3 worldPosition) {
		float percentX = (worldPosition.x - transform.position.x) / graphWorldSize.x + 0.5f;
		float percentY = (worldPosition.z - transform.position.z) / graphWorldSize.y + 0.5f;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((graphCountX - 1) * percentX);
		int y = Mathf.RoundToInt((graphCountY - 1) * percentY);
		return graph[x, y];
	}

	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position, new Vector3(graphWorldSize.x, 1, graphWorldSize.y));

		if (graph != null && displayGizmos) {
			foreach (Node n in graph) {
				if (displayOnlyChecked && n.EstimatedTotalCost <= 0) {
					continue;
				}
				switch (n.ClusterID) {
					case 0:
						Gizmos.color = Color.gray;
						break;
					case 1:
						Gizmos.color = Color.cyan;
						break;
					case 2:
						Gizmos.color = Color.yellow;
						break;
					case 3:
						Gizmos.color = Color.magenta;
						break;
					case 4:
						Gizmos.color = Color.blue;
						break;
					case 5:
						Gizmos.color = new Color(111, 0, 0);
						break;
					case 6:
						Gizmos.color = new Color(111, 101, 0);
						break;
					case 7:
						Gizmos.color = new Color(111, 101, 166);
						break;
				}
				Gizmos.color = n.Walkable ? Gizmos.color : Color.red;
				Vector3 size = (Vector3.one * (nodeDiameter)) / 2;
				Gizmos.DrawCube(n.WorldPosition, size);
				if (clusterCenters.ContainsValue(n)) {
					Gizmos.DrawCube(n.WorldPosition + (Vector3.up * 2), size * 2);
				}
			}
		}
	}

}

public struct GraphLocation {
	public int x;
	public int y;

	public GraphLocation(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public override bool Equals(object obj) {
		if (obj is GraphLocation) {
			GraphLocation a = (GraphLocation)obj;
			return this.x == a.x && this.y == a.y;
		} else {
			return false;
		}
	}
}