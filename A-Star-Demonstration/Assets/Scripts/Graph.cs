using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph {

	private Dictionary<int, List<NodeConnection>> connectionDict = new Dictionary<int, List<NodeConnection>>();

	/** Returns a list of connections outgoing from "fromNode"
	 */
	public List<NodeConnection> GetConnections(Node fromNode) {
		return connectionDict[fromNode.ID];
	}

}
