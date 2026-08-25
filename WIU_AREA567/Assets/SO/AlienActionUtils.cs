using UnityEngine;

// Small shared helpers for the category-specific StateActions
public static class AlienActionUtils
{
    public static Collider2D FindNearest(Vector2 origin, float radius, LayerMask layer, GameObject self)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, layer);
        Collider2D nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.gameObject == self) continue;

            float dist = Vector2.Distance(origin, hit.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = hit;
            }
        }

        return nearest;
    }

    public static Vector2 GetAllyCentre(Vector2 origin, float radius, LayerMask allyLayer, GameObject self)
    {
        Collider2D[] allies = Physics2D.OverlapCircleAll(origin, radius, allyLayer);
        if (allies.Length == 0) return origin;

        Vector2 sum = Vector2.zero;
        int count = 0;

        foreach (var ally in allies)
        {
            if (ally.gameObject == self) continue;
            sum += (Vector2)ally.transform.position;
            count++;
        }

        return count > 0 ? sum / count : origin;
    }
}