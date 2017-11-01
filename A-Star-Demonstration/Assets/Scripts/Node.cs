using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

	public int ID { get; set; }
	public bool Walkable { get; set; }
	public NodeConnection Connection { get; set; }
	public float CostSoFar { get; set; }
	public float EstimatedTotalCost { get; set; }
	public float Cost { get; set; }

}
