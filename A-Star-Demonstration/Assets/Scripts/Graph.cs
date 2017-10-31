using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph {

	private Dictionary<int, List<NodeConnection>> connectionDict = new Dictionary<int, List<NodeConnection>>();

	/** Returns a list of connections outgoing from "fromNode"
	 */
	public List<NodeConnection> GetConnections(NodeRecord fromNode) {
		return connectionDict[fromNode.node];
	}

}
