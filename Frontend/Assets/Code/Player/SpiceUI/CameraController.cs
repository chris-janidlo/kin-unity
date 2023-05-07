using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Player.SpiceUI
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float panSensitivity,
            rotateSensitivity,
            zoomSensitivity;

        [SerializeField] private AnimationCurve zoomPositionCurve,
            zoomFovCurve;

        [SerializeField] private float zoomSmoothTime;

        [SerializeField] private Transform rigTransform,
            cameraTransform;

        [SerializeField] private new Camera camera;

        private Vector2 desiredRotation;

        private float desiredZoomLevel,
            currentZoomLevel;

        private Tween zoomTween;

        private void Start()
        {
            desiredZoomLevel = currentZoomLevel = 0.5f;
            desiredRotation = rigTransform.localEulerAngles;
            ApplyZoomLevel();
        }

        public void OnCameraPan(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            var delta = context.ReadValue<Vector2>();
            rigTransform.localPosition -= rigTransform.rotation * delta * panSensitivity;

            // TODO: clamp pan https://dynalist.io/d/kxguqztlKqFtRz6IeZQXFajg#z=eDwWMLTBMBht64SeDlzGgd58
        }

        public void OnCameraRotate(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            var delta = context.ReadValue<Vector2>();

            desiredRotation += new Vector2(
                -delta.y * rotateSensitivity,
                delta.x * rotateSensitivity
            );
            desiredRotation.x = Mathf.Clamp(desiredRotation.x, -90, 90);

            rigTransform.localEulerAngles = desiredRotation;
        }

        public void OnCameraZoom(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            var delta = context.ReadValue<float>();

            if (context.control.path.Contains("Mouse"))
                // because different platforms use different scroll-wheel deltas (eg Windows
                // uses +-120 and macOS uses a value in [-1, 1]), just read the sign
                delta = Mathf.Sign(delta);

            desiredZoomLevel = Mathf.Clamp01(desiredZoomLevel + delta * zoomSensitivity);

            if (zoomTween != null)
                zoomTween.Kill();

            zoomTween = DOTween
                .To(
                    () => currentZoomLevel,
                    x => currentZoomLevel = x,
                    desiredZoomLevel,
                    zoomSmoothTime
                )
                .SetEase(Ease.Linear)
                .OnKill(() => zoomTween = null)
                .OnUpdate(() => ApplyZoomLevel());
        }

        public void OnCameraReset(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            rigTransform.localPosition = Vector3.zero;
        }

        private void ApplyZoomLevel()
        {
            var z = zoomPositionCurve.Evaluate(currentZoomLevel);
            cameraTransform.localPosition = Vector3.forward * z;

            var fov = zoomFovCurve.Evaluate(currentZoomLevel);
            camera.fieldOfView = fov;
        }
    }
}