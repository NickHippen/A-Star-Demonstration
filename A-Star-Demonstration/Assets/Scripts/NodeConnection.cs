using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeConnection {

	/*
	 * Returns the non-negative cost of the connection
	 */
	public float Cost { get; set; }

	/*
	 * Returns the node that this connection came from
	 */
	public NodeRecord FromNode { get; set; }

	/*
	 * Returns the node that this connection leads to
	 */
	public NodeRecord ToNode { get; set; }

}
