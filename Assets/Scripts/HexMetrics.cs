using UnityEngine;

public enum HexDirection {
    NE, E, SE, SW, W, NW
}

public enum HexEdgeType {
    Flat, Slope, Cliff
}
public struct EdgeVertices {
    public Vector3 v1, v2, v3, v4;
    public EdgeVertices (Vector3 corner1, Vector3 corner2) {
        v1 = corner1;
        v2 = Vector3.Lerp(corner1, corner2, 1f / 3f);
        v3 = Vector3.Lerp(corner1, corner2, 2f / 3f);
        v4 = corner2;
    }
    
    public static EdgeVertices TerraceLerp (
        EdgeVertices a, EdgeVertices b, int step)
    {
        EdgeVertices result;
        result.v1 = HexMetrics.TerraceLerp(a.v1, b.v1, step);
        result.v2 = HexMetrics.TerraceLerp(a.v2, b.v2, step);
        result.v3 = HexMetrics.TerraceLerp(a.v3, b.v3, step);
        result.v4 = HexMetrics.TerraceLerp(a.v4, b.v4, step);
        return result;
    }
}
public static class HexMetrics {
    
    static Vector3[] corners = 
    {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };
    
    public static Texture2D noiseSource;
    
    public const float outerRadius = 10f;

    public const float innerRadius = outerRadius * 0.866025404f;
    
    public const float solidFactor = 0.75f;
	
    public const float blendFactor = 1f - solidFactor;
    
    public const float elevationStep = 4f;
    
    public const int terracesPerSlope = 2;

    public const int terraceSteps = terracesPerSlope * 2 + 1;
    
    public const float horizontalTerraceStepSize = 1f / terraceSteps;
	
    public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);
    
    public const float cellPerturbStrength = 5f;
    
    public const float noiseScale = 0.003f;
    
    public const float elevationPerturbStrength = 1.5f;
    public static HexEdgeType GetEdgeType (int elevation1, int elevation2) {
        if (elevation1 == elevation2) {
            return HexEdgeType.Flat;
        }
        int delta = elevation2 - elevation1;
        if (delta == 1 || delta == -1) {
            return HexEdgeType.Slope;
        }
        return HexEdgeType.Cliff;
    }
    public static Vector3 TerraceLerp (Vector3 a, Vector3 b, int step) {
        float h = step * horizontalTerraceStepSize;
        a.x += (b.x - a.x) * h;
        a.z += (b.z - a.z) * h;
        float v = ((step + 1) / 2) * verticalTerraceStepSize;
        a.y += (b.y - a.y) * v;
        return a;
    }
    public static Color TerraceLerp (Color a, Color b, int step) {
        float h = step * horizontalTerraceStepSize;
        return Color.Lerp(a, b, h);
    }
    public static Vector3 GetFirstCorner (HexDirection direction) 
    {
        return corners[(int)direction];
    }
    public static Vector3 GetSecondCorner (HexDirection direction) 
    {
        return corners[(int)direction + 1];
    }
    public static Vector3 GetFirstSolidCorner (HexDirection direction) 
    {
        return corners[(int)direction] * solidFactor;
    }
    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return corners[(int)direction + 1] * solidFactor;
    }
    public static Vector3 GetBridge (HexDirection direction) {
        return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
    }
    public static Vector4 SampleNoise (Vector3 position) {
        return noiseSource.GetPixelBilinear(position.x * noiseScale, position.z * noiseScale);
    }
}