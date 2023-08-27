using UnityEngine;
using Pathfinding;
using System.Linq;

[RequireComponent(typeof(DynamicGridObstacle))]
public class PenaltyScript : MonoBehaviour
{
    public float penaltyRadius; // Nodes within this radius will be penalized
    public int maxPenalty; // Maximum penalty to add to a node

    private DynamicGridObstacle dynamicGridObstacle;

    private void Awake()
    {
        dynamicGridObstacle = GetComponent<DynamicGridObstacle>();
    }

    private void Update()
    {
        GridGraph gg;
        gg = AstarPath.active.data.gridGraph;
        gg.GetNodes(node =>
        {
            // Get the nodes around the player within the defined radius



            if (node != null)
            {
                // Calculate distance from player
                float distance = Vector3.Distance((Vector3)node.position, transform.position);

                // Calculate penalty based on distance
                int penalty = Mathf.Clamp(Mathf.RoundToInt(maxPenalty * (1f - (distance / penaltyRadius))), 0, maxPenalty);

                // Apply penalty
                node.Penalty = (uint)penalty;
            }
        });

        // Update the graph after modifying penalties
        dynamicGridObstacle.DoUpdateGraphs();
    }
}