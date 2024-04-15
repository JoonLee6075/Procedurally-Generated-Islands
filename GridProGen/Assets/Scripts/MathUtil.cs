using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtil 
{
    /// <summary>
    /// value - The calculated value to process
    /// innerRadius - The distance from center to start feathering
    /// outerRadius - The distance from center to fully fall off
    /// x - The x-coordinate of the value position
    /// y - The y-coordinate of the value position
    /// cx - The x-coordinate of the center position
    /// cy - The y-coordinate of the center position
    /// </summary>
    /// <param name="value"></param>
    /// <param name="innerRadius"></param>
    /// <param name="outerRadius"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="cx"></param>
    /// <param name="cy"></param>
    /// <returns></returns>
    public static float FeatheredRadialFallOff(float value, float innerRadius, float outerRadius, int x, int y, float cx, float cy)
    {
        float dx = cx - x;
        float dy = cy - y;
        float distSqr = dx * dx + dy * dy;
        float iRadSqr = innerRadius * innerRadius;
        float oRadSqr = outerRadius * outerRadius;

        if (distSqr >= oRadSqr) return 0f;
        if (distSqr <= iRadSqr) return value;

        float dist = Mathf.Sqrt(distSqr);
        float t = Mathf.InverseLerp(innerRadius, outerRadius, dist);
        // Use t with whatever easing you want here, or leave it as is for linear easing
        return value * t;
    }

    /// <summary>
    /// value - The calculated value to process
    /// radius - The distance from center to calculate falloff distance
    /// x - The x-coordinate of the value position
    /// y - The y-coordinate of the value position
    /// cx - The x-coordinate of the center position
    /// cy - The y-coordinate of the center position
    /// </summary>
    /// <param name="value"></param>
    /// <param name="radius"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="cx"></param>
    /// <param name="cy"></param>
    /// <returns></returns>
    public static float RadialFallOff(float value, float radius, int x, int y, float cx, float cy)
    {
        float dx = cx - x;
        float dy = cy - y;
        float distSqr = dx * dx + dy * dy;
        float radSqr = radius * radius;

        if (distSqr > radSqr) return 0f;
        return value;
    }
}
