using UnityEngine;
using UnityEngine.AI;

// helper methods for picking navmesh positions a minimum distance away from a center point
public static class SpawnPositionUtility
{
    // tries to return a sampled navmesh point within the supplied ring
    public static bool TryGetPosition(
        Vector3 center,
        float minDistance,
        float maxDistance,
        out Vector3 position,
        int maxAttempts = 8,
        float navMeshSampleRadius = 2f)
    {
        position = center;

        if (maxDistance <= 0f)
        {
            return false;
        }

        minDistance = Mathf.Clamp(minDistance, 0f, maxDistance);

        for (int attempt = 0; attempt < Mathf.Max(1, maxAttempts); attempt++)
        {
            Vector2 direction = Random.insideUnitCircle;
            if (direction == Vector2.zero)
            {
                direction = Vector2.right;
            }

            direction.Normalize();
            float distance = Random.Range(minDistance, maxDistance);
            Vector3 candidate = center + new Vector3(direction.x, 0f, direction.y) * distance;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, navMeshSampleRadius, NavMesh.AllAreas))
            {
                position = hit.position;
                return true;
            }
        }

        return false;
    }
}
