using UnityEngine;

namespace Excalibur
{
    public sealed class CameraManager : Singleton<CameraManager>
    {
        private Camera _mainCamera;
        private Camera _uiCamera;

        public Camera mainCamera { get { if (_mainCamera == null) { _mainCamera = Camera.main; } return _mainCamera; } }
        public Camera uiCamera => _uiCamera;

        public void CreateUICamera ()
        {
            GameObject uiCameraGO = new GameObject ("UICamera");
            uiCameraGO.transform.position = new Vector3 (0f, -10000f, 0f);
            _uiCamera = uiCameraGO.AddComponent<Camera> ();
            _uiCamera.clearFlags = CameraClearFlags.Depth;
            _uiCamera.orthographic = true;
            _uiCamera.depth = 100;
            _uiCamera.cullingMask = 1 << 5;
            Object.DontDestroyOnLoad (_uiCamera);
        }
    }
}