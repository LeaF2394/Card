using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.General
{
    public static class MouseUtils
    {
        private static Camera camera = Camera.main;
        
        public static Vector3 GetMousePositionInWorldSpace(float z = 0f)
        {
            Plane dragPlane = new Plane(camera.transform.forward, new Vector3(0f, 0f, z));
            Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (dragPlane.Raycast(ray, out float dist))
            {
                return ray.GetPoint(dist);
            }
            return Vector3.zero;
        }

        public static Vector3 GetMousePositionFromCamera(float z = 0f)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = z;
            Vector3 res = camera.ScreenToWorldPoint(mousePos);
            res.z = z;
            return res;
        }
    }
}