using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Player.SpiceUI
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private float PanSensitivity,
            RotateSensitivity,
            ZoomSensitivity;

        [SerializeField]
        private AnimationCurve ZoomPositionCurve,
            ZoomFovCurve;

        [SerializeField]
        private float ZoomSmoothTime;

        [SerializeField]
        private Transform RigTransform,
            CameraTransform;

        [SerializeField]
        private Camera Camera;

        private Vector2 desiredRotation;

        private float desiredZoomLevel,
            currentZoomLevel;

        private Tween zoomTween;

        private void Start()
        {
            desiredZoomLevel = currentZoomLevel = 0.5f;
            desiredRotation = RigTransform.localEulerAngles;
            ApplyZoomLevel();
        }

        [UsedImplicitly]
        public void OnCameraPan(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            var delta = context.ReadValue<Vector2>();
            RigTransform.localPosition -= RigTransform.rotation * delta * PanSensitivity;

            // TODO: clamp pan https://dynalist.io/d/kxguqztlKqFtRz6IeZQXFajg#z=eDwWMLTBMBht64SeDlzGgd58
        }

        [UsedImplicitly]
        public void OnCameraRotate(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            var delta = context.ReadValue<Vector2>();

            desiredRotation += new Vector2(
                -delta.y * RotateSensitivity,
                delta.x * RotateSensitivity
            );
            desiredRotation.x = Mathf.Clamp(desiredRotation.x, -90, 90);

            RigTransform.localEulerAngles = desiredRotation;
        }

        [UsedImplicitly]
        public void OnCameraZoom(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            var delta = context.ReadValue<float>();

            if (context.control.path.Contains("Mouse"))
                // because different platforms use different scroll-wheel deltas (eg Windows
                // uses +-120 and macOS uses a value in [-1, 1]), just read the sign
                delta = Mathf.Sign(delta);

            desiredZoomLevel = Mathf.Clamp01(desiredZoomLevel + delta * ZoomSensitivity);

            if (zoomTween != null)
                zoomTween.Kill();

            zoomTween = DOTween
                .To(
                    () => currentZoomLevel,
                    x => currentZoomLevel = x,
                    desiredZoomLevel,
                    ZoomSmoothTime
                )
                .SetEase(Ease.Linear)
                .OnKill(() => zoomTween = null)
                .OnUpdate(() => ApplyZoomLevel());
        }

        [UsedImplicitly]
        public void OnCameraReset(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            RigTransform.localPosition = Vector3.zero;
        }

        private void ApplyZoomLevel()
        {
            float z = ZoomPositionCurve.Evaluate(currentZoomLevel);
            CameraTransform.localPosition = Vector3.forward * z;

            float fov = ZoomFovCurve.Evaluate(currentZoomLevel);
            Camera.fieldOfView = fov;
        }
    }
}
