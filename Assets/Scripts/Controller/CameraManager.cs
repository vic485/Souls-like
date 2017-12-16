using UnityEngine;

namespace Gazzotto.Controller
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager singleton;

        public bool lockon;
        public float followSpeed = 3;
        public float mouseSpeed = 2;
        public float controllerSpeed = 5;

        public Transform target;

        [HideInInspector] public Transform pivot;
        [HideInInspector] public Transform camTransform;

        float turnSmoothing = 0.1f;
        public float minAngle = -35f;
        public float maxAngle = 35f;

        float smoothX;
        float smoothY;
        float smoothXVel;
        float smoothYVel;
        public float lookAngle;
        public float tiltAngle;

        private void Awake()
        {
            if (singleton != null)
                Destroy(gameObject);

            singleton = this;
        }

        public void Init(Transform t)
        {
            target = t;

            camTransform = Camera.main.transform;
            pivot = camTransform.parent;
        }

        public void Tick(float d)
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");

            float c_h = Input.GetAxis("RightStick X");
            float c_v = Input.GetAxis("RightStick Y");

            float targetSpeed = mouseSpeed;

            if (c_h != 0 || c_v != 0)
            {
                h = c_h;
                v = c_v;
                targetSpeed = controllerSpeed;
            }

            FollowTarget(d);
            HandleRotations(d, v, h, targetSpeed);
        }

        void FollowTarget(float d)
        {
            float speed = d * followSpeed;
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            transform.position = targetPosition;
        }

        void HandleRotations(float d, float v, float h, float targetSpeed)
        {
            if (turnSmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXVel, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYVel, turnSmoothing);
            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            if (lockon)
            {
                // TODO: comeback later
            }

            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);

            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
        }
    }
}