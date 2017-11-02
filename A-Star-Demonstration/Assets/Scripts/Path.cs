using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path {

	public List<NodeConnection> PathConnections { get; set; }

	public Path(List<NodeConnection> pathConnections) {
		this.PathConnections = pathConnections;
	}

	public void DrawGizmos() {
		Gizmos.color = Color.green;
		foreach (NodeConnection connection in PathConnections) {
			connection.DrawGizmos();
		}
	}

}
