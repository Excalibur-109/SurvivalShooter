using UnityEngine;

namespace Excalibur
{
    public sealed class CameraManager : Singleton<CameraManager>
    {
        private Camera _mainCamera;
        private Camera _uiCamera;

        public Camera mainCamera => _mainCamera;
        public Camera uiCamera => _uiCamera;

        public void CreateUICamera ()
        {
            GameObject cameraGO = new GameObject ("UICamera");
            cameraGO.transform.position = new Vector3(0f, -10000f, 0f);
            _uiCamera = cameraGO.AddComponent<Camera>();
            _uiCamera.clearFlags = CameraClearFlags.Depth;
            _uiCamera.orthographic = true;
            _uiCamera.depth = 100;
            _uiCamera.cullingMask = 1 << 5;
            //MonoExtension.InitializeObject(cameraGO);
            Object.DontDestroyOnLoad(_uiCamera);
        }

        public void CreateMainCamera()
        {
            GameObject cameraGO = new GameObject ("MainCamera");
            cameraGO.tag = "MainCamera";
            cameraGO.transform.position = Vector3.back;
            _mainCamera = cameraGO.AddComponent<Camera>();
            _mainCamera.orthographic = true;
            _mainCamera.clearFlags = CameraClearFlags.SolidColor;
            _mainCamera.backgroundColor = Color.black;
            _mainCamera.cullingMask &= ~(1 << 5);
            _mainCamera.orthographicSize = 5f;
            //MonoExtension.InitializeObject(cameraGO);
            Object.DontDestroyOnLoad(_mainCamera);
        }
    }
}