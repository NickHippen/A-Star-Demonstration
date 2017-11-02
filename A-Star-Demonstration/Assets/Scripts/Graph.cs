using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour {

	private readonly Dictionary<GraphLocation, List<NodeConnection>> connectionDict = new Dictionary<GraphLocation, List<NodeConnection>>();

	public Vector2 gridWorldSize;
	public float nodeRadius;

	private float nodeDiameter;
	private int gridCountX;
	private int gridCountY;
	private Node[,] graph;

	void Awake() {
		nodeDiameter = nodeRadius * 2f;
		gridCountX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridCountY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
		CreateGrid();
	}

	/** Returns a list of connections outgoing from "fromNode"
	 */
	public List<NodeConnection> GetConnections(Node fromNode) {
		return connectionDict[fromNode.Location];
	}

	public void CreateGrid() {
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

		for (int x = 0; x < gridCountX; x++) {
			for (int y = 0; y < gridCountY; y++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

				graph[x, y] = new Node();
			}
		}
	}

}

public struct GraphLocation {
	public int x;
	public int y;
}