using UnityEngine;

public static class PhysicsCalc
{
    public static float SlowDownDistance(float from, float to)
    {
        if (from < to)
            return 0;

        float deltaV = from - to;
        const float brakeDeceleration = 4.5f;
        float deltaT = deltaV / brakeDeceleration;
        return to * deltaT + deltaT * deltaV / 2;
    }

    public static bool IsToRight(Vector3 point, Vector3 lineDir, Vector3 pointOnLine)
    {
        var pointOnLine2D = new Vector2(pointOnLine.x, pointOnLine.z);
        var normal = new Vector2(lineDir.z, -lineDir.x);
        var point2D = new Vector2(point.x, point.z);

        return Vector2.Dot(point2D - pointOnLine2D, normal) > 0;
    }

    public static bool IsBehind(Vector3 here, Vector3 forwardDir, Vector3 point)
    {
        var right = new Vector3(forwardDir.z, 0, -forwardDir.x);
        return IsToRight(point, right, here);
    }

    public static Vector3 ProjectPointOnLine(Vector3 point, Vector3 lineDir, Vector3 pointOnLine)
    {
        Vector3 linePointToPoint = point - pointOnLine;
        float t = Vector3.Dot(linePointToPoint, lineDir);
        return pointOnLine + lineDir * t;
    }

    public static float OvertakeDistance(float v1, float v2)
    {
        float deltav = Mathf.Abs(v1 - v2);
        if (deltav == 0f)
            return float.PositiveInfinity;

        const float s = 5 * 8f;
        float t = s / deltav;
        return Mathf.Max(v1, v2) * t;
    }
}
