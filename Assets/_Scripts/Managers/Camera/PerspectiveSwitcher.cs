using rene_roid;
using UnityEngine;

namespace _Scripts.Managers.Camera {
    [RequireComponent (typeof(MatrixBlender))]
    public class PerspectiveSwitcher : MonoBehaviour
    {
        private Matrix4x4 ortho,perspective;
        public float fov = 60f, near = .3f, far = 1000f, orthographicSize = 50f;
        private float aspect;
        private MatrixBlender blender;
        private bool orthoOn;
        public float speed = 1.0f;
 
        void Start()
        {
            aspect = (float) Screen.width / (float) Screen.height;
            ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
            perspective = Matrix4x4.Perspective(fov, aspect, near, far);
            Helpers.Camera.projectionMatrix = perspective;
            orthoOn = false;
            blender = (MatrixBlender) GetComponent(typeof(MatrixBlender));
        
            blender.BlendToMatrix(perspective, speed);
        }
        
        public void SetOrtho(){
            orthoOn = true;
            blender.BlendToMatrix(ortho, speed);
        }
        
        public void SetPerspective(){
            orthoOn = false;
            blender.BlendToMatrix(perspective, speed);
        }
    }
}
