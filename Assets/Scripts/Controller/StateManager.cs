using UnityEngine;
using Gazzotto.Enemies;
using Gazzotto.Managers;
using Gazzotto.Stats;

namespace Gazzotto.Controller
{
    public class StateManager : MonoBehaviour
    {
        [Header("Inits")]
        public GameObject activeModel;

        [Header("Stats")]
        public Attributes attributes;
        public CharacterStats characterStats;

        [Header("Inputs")]
        public float vertical;
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDir;
        public bool rt, rb, lt, lb;
        public bool rollInput;
        public bool itemInput;

        [Header("Stats")]
        public float moveSpeed = 3.5f;
        public float runSpeed = 5.5f;
        public float rotateSpeed = 9f;
        public float toGround = 0.5f;
        public float rollSpeed = 15f;
        public float parryOffset = 1.4f;
        public float backStabOffest = 1.4f;

        [Header("States")]
        public bool onGround;
        public bool run;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        public bool isTwoHanded;
        public bool usingItem;
        public bool canBeParried;
        public bool parryIsOn;
        public bool isBlocking;
        public bool isLeftHand;

        [Header("Other")]
        public EnemyTarget lockOnTarget;
        public Transform lockOnTransform;
        public AnimationCurve roll_curve;
        //public EnemyStates parryTarget;

        [HideInInspector] public Animator anim;
        [HideInInspector] public Rigidbody rigid;
        [HideInInspector] public AnimatorHook a_hook;
        [HideInInspector] public ActionManager actionManager;
        [HideInInspector] public InventoryManager inventoryManager;

        [HideInInspector] public float delta;
        [HideInInspector] public LayerMask ignoreLayers;
        [HideInInspector] public Action currentAction;

        float _actionDelay = 0;

        public void Init()
        {
            SetupAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            inventoryManager = GetComponent<InventoryManager>();
            inventoryManager.Init(this);

            actionManager = GetComponent<ActionManager>();
            actionManager.Init(this);

            a_hook = activeModel.GetComponent<AnimatorHook>();
            if (a_hook == null)
                a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this, null);

            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);

            anim.SetBool(StaticStrings.onGround, true);
        }

        void SetupAnimator()
        {
            if (activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                if (anim == null)
                    Debug.LogError("No model found");
                else
                    activeModel = anim.gameObject;
            }

            if (anim == null)
                anim = activeModel.GetComponent<Animator>();

            anim.applyRootMotion = false;
        }

        public void FixedTick(float d)
        {
            delta = d;

            isBlocking = false;
            usingItem = anim.GetBool(StaticStrings.interacting);

            DetectAction();
            DetectItemAction();
            
            inventoryManager.rightHandWeapon.instance.weaponModel.SetActive(!usingItem);

            anim.SetBool(StaticStrings.blocking, isBlocking);
            anim.SetBool(StaticStrings.isLeft, isLeftHand);

            if (inAction)
            {
                anim.applyRootMotion = true;

                _actionDelay += delta;
                if (_actionDelay > 0.3f)
                {
                    inAction = false;
                    _actionDelay = 0;
                }
                else
                {
                    return;
                }
            }

            canMove = anim.GetBool(StaticStrings.canMove);

            if (!canMove)
                return;

            //a_hook.rm_multi = 1;
            a_hook.CloseRoll();
            HandleRolls();

            anim.applyRootMotion = false;

            rigid.drag = (moveAmount > 0 || !onGround) ? 0 : 4;

            float targetSpeed = moveSpeed;
            if (usingItem)
            {
                run = false;
                moveAmount = Mathf.Clamp(moveAmount, 0, 0.45f);
            }

            if (run)
                targetSpeed = runSpeed;

            if (onGround)
                rigid.velocity = moveDir * (targetSpeed * moveAmount);

            if (run)
                lockOn = false;

            Vector3 targetDir = (!lockOn) ? moveDir : (lockOnTransform != null) ? lockOnTransform.position - transform.position : moveDir;
            targetDir.y = 0;
            if (targetDir == Vector3.zero)
                targetDir = transform.forward;
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;

            anim.SetBool(StaticStrings.lockon, lockOn);

            if (!lockOn)
                HandleMovementAnimations();
            else
                HandleLockOnAnimations(moveDir);
        }

        public void DetectItemAction()
        {
            if (!canMove || usingItem || isBlocking)
                return;

            if (!itemInput)
                return;

            ItemAction slot = actionManager.consumableItem;
            string targetAnim = slot.targetAnim;
            if (string.IsNullOrEmpty(targetAnim))
                return;

            //inventoryManager.curWeapon.weaponModel.SetActive(false);
            usingItem = true;
            anim.Play(targetAnim);
        }

        public void DetectAction()
        {
            if (!canMove || usingItem)
                return;

            if (!rb && !rt && !lb && !lt)
                return;

            Action slot = actionManager.GetActionSlot(this);
            if (slot == null)
                return;

            switch (slot.type)
            {
                case ActionType.attack:
                    AttackAction(slot);
                    break;
                case ActionType.block:
                    BlockAction(slot);
                    break;
                case ActionType.parry:
                    ParryAction(slot);
                    break;
                case ActionType.spells:
                    break;
                default:
                    print("You did something wrong on the action type");
                    break;
            }
        }

        void AttackAction(Action slot)
        {
            if (CheckForParry(slot))
                return;

            if (CheckForBackstab(slot))
                return;

            currentAction = slot;

            string targetAnim = null;

            targetAnim = slot.targetAnim;

            if (string.IsNullOrEmpty(targetAnim))
                return;

            canMove = false;
            inAction = true;

            float targetSpeed = 1f;
            if (slot.changeSpeed)
            {
                targetSpeed = slot.animSpeed;
                if (targetSpeed == 0)
                    targetSpeed = 1;
            }

            canBeParried = slot.canBeParried;
            anim.SetFloat(StaticStrings.animSpeed, targetSpeed);
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.CrossFade(targetAnim, 0.2f);
        }

        void BlockAction(Action slot)
        {
            isBlocking = true;
            isLeftHand = slot.mirror;
        }

        bool CheckForParry(Action slot)
        {
            if (!slot.canParry)
                return false;

            EnemyStates parryTarget = null;
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDir = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, rayDir, out hit, 3, ignoreLayers))
            {
                parryTarget = hit.transform.GetComponentInParent<EnemyStates>();
            }

            if (parryTarget == null)
                return false;

            if (parryTarget.parriedBy == null)
                return false;

            /*float dis = Vector3.Distance(parryTarget.transform.position, transform.position);

            if (dis > 3)
                return false;*/

            Vector3 dir = parryTarget.transform.position - transform.position;
            dir.Normalize();
            dir.y = 0;
            float angle = Vector3.Angle(transform.forward, dir);

            if (angle < 60)
            {
                Vector3 targetPosition = -dir * parryOffset;
                targetPosition += parryTarget.transform.position;
                transform.position = targetPosition;

                if (dir == Vector3.zero)
                    dir = -parryTarget.transform.forward;

                Quaternion eRotation = Quaternion.LookRotation(-dir);
                Quaternion ourRot = Quaternion.LookRotation(dir);

                parryTarget.transform.rotation = eRotation;
                transform.rotation = ourRot;
                parryTarget.IsGettingParried(slot);
                canMove = false;
                inAction = true;
                anim.SetBool(StaticStrings.mirror, slot.mirror);
                anim.CrossFade(StaticStrings.parry_attack, 0.2f);
                lockOnTarget = null;
                return true;
            }

            return false;
        }

        bool CheckForBackstab(Action slot)
        {
            if (!slot.canBackstab)
                return false;

            EnemyStates backstabTarget = null;
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDir = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, rayDir, out hit, 1, ignoreLayers))
            {
                backstabTarget = hit.transform.GetComponentInParent<EnemyStates>();
            }

            if (backstabTarget == null)
                return false;

            Vector3 dir = transform.position - backstabTarget.transform.position;
            dir.Normalize();
            dir.y = 0;
            float angle = Vector3.Angle(backstabTarget.transform.forward, dir);

            if (angle > 150)
            {
                Vector3 targetPosition = dir * backStabOffest;
                targetPosition += backstabTarget.transform.position;
                transform.position = targetPosition;

                backstabTarget.transform.rotation = transform.rotation;
                backstabTarget.IsGettingBackstabbed(slot);
                canMove = false;
                inAction = true;
                anim.SetBool(StaticStrings.mirror, slot.mirror);
                anim.CrossFade(StaticStrings.parry_attack, 0.2f);
                lockOnTarget = null;
                return true;
            }

            return false;
        }

        void ParryAction(Action slot)
        {
            string targetAnim = null;

            targetAnim = slot.targetAnim;

            if (string.IsNullOrEmpty(targetAnim))
                return;

            float targetSpeed = 1f;
            if (slot.changeSpeed)
            {
                targetSpeed = slot.animSpeed;
                if (targetSpeed == 0)
                    targetSpeed = 1;
            }

            anim.SetFloat(StaticStrings.animSpeed, targetSpeed);

            canBeParried = slot.canBeParried;
            canMove = false;
            inAction = true;
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.CrossFade(targetAnim, 0.2f);
        }

        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();
            anim.SetBool(StaticStrings.onGround, onGround);
        }

        void HandleRolls()
        {
            if (!rollInput || usingItem)
                return;

            float v = vertical;
            float h = horizontal;
            v = (moveAmount > 0.3f) ? 1 : 0;
            h = 0;

            /*if (!lockOn)
            {
                v = (moveAmount > 0.3f) ? 1 : 0;
                h = 0;
            }
            else
            {
                if (Mathf.Abs(v) < 0.3f)
                    v = 0;
                if (Mathf.Abs(h) < 0.3f)
                    h = 0;
            }*/
            if (v != 0)
            {
                if (moveDir == Vector3.zero)
                    moveDir = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = targetRot;
                a_hook.rm_multi = rollSpeed;
                a_hook.InitForRoll();
            }
            else
            {
                a_hook.rm_multi = 1.3f;
            }

            anim.SetFloat(StaticStrings.vertical, v);
            anim.SetFloat(StaticStrings.horizontal, h);

            canMove = false;
            inAction = true;
            anim.CrossFade(StaticStrings.Rolls, 0.2f);
        }

        void HandleMovementAnimations()
        {
            anim.SetBool(StaticStrings.run, run);
            anim.SetFloat(StaticStrings.vertical, moveAmount, 0.4f, delta);
        }

        void HandleLockOnAnimations(Vector3 moveDir)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
            float h = relativeDir.x;
            float v = relativeDir.z;

            anim.SetFloat(StaticStrings.vertical, v, 0.2f, delta);
            anim.SetFloat(StaticStrings.horizontal, h, 0.2f, delta);
        }

        public bool OnGround()
        {
            bool r = false;

            Vector3 origin = transform.position + (Vector3.up * toGround);
            Vector3 dir = -Vector3.up;
            float dis = toGround + 0.3f;
            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, dis, ignoreLayers))
            {
                r = true;
                Vector3 targetPosition = hit.point;
                transform.position = targetPosition;
            }

            return r;
        }

        public void HandleTwoHanded()
        {
            bool isRight = true;
            Weapon w = inventoryManager.rightHandWeapon.instance;
            if (w == null)
            {
                w = inventoryManager.leftHandWeapon.instance;
                isRight = false;
            }

            if (w == null)
            {
                return;
            }

            if (isTwoHanded)
            {
                anim.CrossFade(w.th_idle, 0.2f);
                actionManager.UpdateActionsTwoHanded();

                if (isRight)
                {
                    if (inventoryManager.leftHandWeapon)
                        inventoryManager.leftHandWeapon.instance.weaponModel.SetActive(false);
                }
                else
                {
                    if (inventoryManager.rightHandWeapon)
                        inventoryManager.rightHandWeapon.instance.weaponModel.SetActive(false);
                }
            }
            else
            {
                //string targetAnim = w.oh_idle;
                //targetAnim += (isRight) ? StaticStrings._r : StaticStrings._l;
                //anim.CrossFade(targetAnim, 0.2f);
                anim.Play(StaticStrings.equipWeapon_oh);
                anim.Play(StaticStrings.emptyBoth);
                actionManager.UpdateActionsOneHanded();

                if (isRight)
                {
                    if (inventoryManager.leftHandWeapon)
                        inventoryManager.leftHandWeapon.instance.weaponModel.SetActive(true);
                }
                else
                {
                    if (inventoryManager.rightHandWeapon)
                        inventoryManager.rightHandWeapon.instance.weaponModel.SetActive(true);
                }
            }
        }

        public void IsGettingParried()
        {

        }
    }
}