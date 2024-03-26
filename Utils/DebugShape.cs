using UnityEngine;

namespace Pineapler.Utils;

public static class DebugShape {
    public static Material material;

    private static Mesh _sphere;
    private static Mesh _cube;
    private static Mesh _cylinder;
    private static Mesh _capsule;
    private static Mesh _plane;

    private static void CheckCache() {
        if (_sphere != null) return;
        
        // Default material should be set up by game specific code
        
        GameObject go;
        
        // Sphere
        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _sphere = go.GetComponent<MeshFilter>().sharedMesh;
        Object.Destroy(go);
       
        // Cube
        go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _cube = go.GetComponent<MeshFilter>().sharedMesh;
        Object.Destroy(go);
        
        // Cylinder
        go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        _cylinder = go.GetComponent<MeshFilter>().sharedMesh;
        Object.Destroy(go);
        
        // Capsule
        go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        _capsule = go.GetComponent<MeshFilter>().sharedMesh;
        Object.Destroy(go);
        
        // Plane
        go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        _plane = go.GetComponent<MeshFilter>().sharedMesh;
        Object.Destroy(go);
    }
    
    public static void DrawSphere(Vector3 centre, float radius) {
        CheckCache();
        float d = radius * 2;
        Matrix4x4 transform = Matrix4x4.TRS(centre, Quaternion.identity, new Vector3(d, d, d));
        Graphics.DrawMesh(_sphere, transform, material, 0);
    }

    // public static void DrawCube(Vector3 centre, Vector3 extents) {
    //     
    // }

    public static void DrawCylinder(Vector3 bottom, Vector3 top, float radius) {
        CheckCache();
        Vector3 spine = top - bottom;
        Vector3 midpoint = Vector3.Lerp(bottom, top, 0.5f);
        float height = spine.magnitude * 0.5f;
        float diameter = radius * 2;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, spine.normalized);

        Matrix4x4 transform = Matrix4x4.TRS(midpoint, rotation, new Vector3(diameter, height, diameter));
        Graphics.DrawMesh(_cylinder, transform, material, 0);
    }

    public static void DrawRay(Ray ray, float distance, float radius = 0.01f) {
        Vector3 begin = ray.origin;
        Vector3 end = ray.origin + ray.direction * distance;
        
        DrawSphere(begin, radius);
        DrawSphere(end, radius);
        DrawCylinder(begin, end, radius);
    }
}