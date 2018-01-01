using UnityEngine;
using Gazzotto.Enemies;

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
        public EnemyTarget lockonTarget;
        public Transform lockonTransform;

        [HideInInspector] public Transform pivot;
        [HideInInspector] public Transform camTransform;
        StateManager states;

        float turnSmoothing = 0.1f;
        public float minAngle = -35f;
        public float maxAngle = 35f;

        float smoothX;
        float smoothY;
        float smoothXVel;
        float smoothYVel;
        public float lookAngle;
        public float tiltAngle;

        bool usedRightAxis;

        bool changeTargetLeft;
        bool changeTargetRight;

        private void Awake()
        {
            if (singleton != null)
                Destroy(gameObject);

            singleton = this;
        }

        public void Init(StateManager st)
        {
            states = st;
            target = st.transform;

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

            changeTargetLeft = Input.GetKeyUp(KeyCode.V);
            changeTargetRight = Input.GetKeyUp(KeyCode.V);

            if (lockonTarget != null)
            {
                if (lockonTransform == null)
                {
                    lockonTransform = lockonTarget.GetTarget();
                    states.lockOnTransform = lockonTransform;
                }

                if (Mathf.Abs(c_h) > 0.6f)
                {
                    if (!usedRightAxis)
                    {
                        lockonTransform = lockonTarget.GetTarget((c_h > 0));
                        states.lockOnTransform = lockonTransform;
                        usedRightAxis = true;
                    }
                }

                if (changeTargetLeft || changeTargetRight)
                {
                    lockonTransform = lockonTarget.GetTarget(changeTargetLeft);
                    states.lockOnTransform = lockonTransform;
                }
            }

            if (usedRightAxis)
            {
                if (Mathf.Abs(c_h) < 0.6f)
                    usedRightAxis = false;
            }

            if (c_h != 0 || c_v != 0)
            {
                h = c_h;
                v = -c_v;
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

            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

            if (lockon && lockonTarget != null)
            {
                Vector3 targetDir = lockonTransform.position - transform.position;
                targetDir.Normalize();
                //targetDir.y = 0;

                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, d * 9);
                lookAngle = transform.eulerAngles.y;
                return;
            }

            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);
        }
    }
}