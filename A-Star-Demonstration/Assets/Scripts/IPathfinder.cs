using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathfinder {

	Path PathFindAStar(Node start, Node end, IHeuristic heuristic);

}
