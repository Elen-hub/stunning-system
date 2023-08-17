using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
public class DebugUtility : MonoBehaviour
{
    public bool DebugMode;
    public bool EnableAttachmentRuntimeGizmo;
    public bool EnableDebugActorStatus;
    public static DebugUtility Instance;
    private void Awake()
    {
        Instance = this;
    }
#region Ray
    [global::System.Diagnostics.Conditional("UNITY_EDITOR"), global::System.Diagnostics.Conditional("TRACE_ON")]
    public static void RayTracer(Vector3 point, Vector3 dir, float durationTime= 1f)
    {
        if (!Instance.DebugMode) return;

        UnityEngine.Debug.DrawRay(point, dir, Color.red, durationTime);
    }
    public static void RayTracer(Vector3 point, Vector3 dir, Color color, float durationTime = 1f)
    {
        if (!Instance.DebugMode) return;

        UnityEngine.Debug.DrawRay(point,dir, color, durationTime);
    }
#endregion
#region Line
    public static void Line(Vector3 start, Vector3 end, float durationTime = 1f)
    {
        if (!Instance.DebugMode) return;

        UnityEngine.Debug.DrawLine(start, end, Color.red, durationTime);
    }
    public static void Line(Vector3 start, Vector3 end, Color color, float durationTime = 1f)
    {
        if (!Instance.DebugMode) return;

        UnityEngine.Debug.DrawLine(start, end, color, durationTime);
    }
    #endregion
#region Gizmos
    public class GizmoConfigure
    {
        public enum eGizmoMeshType
        {
            Sphere,
            Cube,
        }
        public eGizmoMeshType GizmoMeshType;
        public Vector3 Position;
        public Vector3 HalfExtents;
        public float Radius;
        public float Time;
        public Color Color;
        public bool IsWire;
    }
    static float _gizmoDeltaTime;
    static Queue<GizmoConfigure> _gizmoQueue = new Queue<GizmoConfigure>();
    [System.Diagnostics.Conditional("UNITY_SERVER")]
    public static void Log(object message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#else
        System.Console.WriteLine(message);
#endif
    }
    public static void DrawSphere(Vector3 position, float radius, bool isWire, float time = 1f)
    {
        _gizmoQueue.Enqueue(new GizmoConfigure()
        {
            GizmoMeshType = GizmoConfigure.eGizmoMeshType.Sphere,
            Position = position,
            Radius = radius,
            IsWire = isWire,
            Time = time,
            Color = Color.red,
        });
    }
    public static void DrawCube(Vector3 pivot, Vector3 halfExtent, bool isWire, float time = 1f)
    {
        _gizmoQueue.Enqueue(new GizmoConfigure()
        {
            GizmoMeshType = GizmoConfigure.eGizmoMeshType.Cube,
            Position = pivot,
            HalfExtents = halfExtent,
            IsWire = isWire,
            Time = time,
            Color = Color.red,
        });
    }
    public static void DrawSphere(Vector3 position, float radius, bool isWire, Color color, float time = 1f)
    {
        _gizmoQueue.Enqueue(new GizmoConfigure()
        {
            Position = position,
            Radius = radius,
            IsWire = isWire,
            Time = time,
            Color = color,
        });
    }
    private void OnDrawGizmos()
    {
        int count = _gizmoQueue.Count;
        while(count > 0)
        {
            GizmoConfigure gizmoConfigure = _gizmoQueue.Dequeue();
            gizmoConfigure.Time -= _gizmoDeltaTime;
            Gizmos.color = gizmoConfigure.Color;
            switch (gizmoConfigure.GizmoMeshType)
            {
                case GizmoConfigure.eGizmoMeshType.Sphere:
                    if (gizmoConfigure.IsWire) Gizmos.DrawWireSphere(gizmoConfigure.Position, gizmoConfigure.Radius);
                    else Gizmos.DrawSphere(gizmoConfigure.Position, gizmoConfigure.Radius);
                    break;
                case GizmoConfigure.eGizmoMeshType.Cube:
                    if (gizmoConfigure.IsWire) Gizmos.DrawWireCube(gizmoConfigure.Position, gizmoConfigure.HalfExtents);
                    else Gizmos.DrawCube(gizmoConfigure.Position, gizmoConfigure.HalfExtents);
                    break;
            }
            if (gizmoConfigure.Time > 0f)
                _gizmoQueue.Enqueue(gizmoConfigure);
            --count;
        }

        _gizmoDeltaTime = 0f;
    }
    private void Update()
    {
        _gizmoDeltaTime += Time.deltaTime;
    }
#endregion
#if UNITY_EDITOR
#region SceneText
    public static void DrawString(string text, Vector3 worldPos, float oX = 0, float oY = 0, Color? colour = null)
    {
        UnityEditor.Handles.BeginGUI();

        var restoreColor = GUI.color;

        if (colour.HasValue) GUI.color = colour.Value;
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
        
        if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
        {
            GUI.color = restoreColor;
            UnityEditor.Handles.EndGUI();
            return;
        }


        UnityEditor.Handles.Label(TransformByPixel(worldPos,  oX, oY), text);

        GUI.color = restoreColor;
        UnityEditor.Handles.EndGUI();
    }
    static Vector3 TransformByPixel(Vector3 position, float x, float y)
    {
        return TransformByPixel(position, new Vector3(x, y));
    }
    static Vector3 TransformByPixel(Vector3 position, Vector3 translateBy)
    {
        Camera cam = UnityEditor.SceneView.currentDrawingSceneView.camera;
        if (cam)
            return cam.ScreenToWorldPoint(cam.WorldToScreenPoint(position) + translateBy);
        else
            return position;
    }
#endregion
#endif
}