using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player.Movement
{
    public class MovementController : MonoBehaviour
    {
        public static PlayerInput PlayerInput;
        public UnityEvent onStartedMoving;
        public float ForwardSpeed => agent.velocity.magnitude;
        public float RotationSpeed => CalculateRotationSpeed();
        public float interactionRange = 2f;
        public bool isTurning;

        [SerializeField] private HumanoidAnimationManager animationManager;

        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask clickableLayers;
        [SerializeField] private float lookRotationSpeed = 5f;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float runSpeed = 8f;

        private Quaternion _lastRotation;
        private Interactable _targetInteractable;
        private float _rotationSpeed;
        private bool _wasMoving;
        private bool _isRunning = false;

        void Awake()
        {
            agent.updateRotation = false;
            agent.stoppingDistance = interactionRange;
        }

        private void Start()
        {
            if (!PlayerInput)
                PlayerInput = GetComponent<PlayerInput>();
        }

        void Update()
        {
            agent.speed = _isRunning ? runSpeed : moveSpeed;

            FaceTarget();

            bool isMoving = AgentIsMoving();

            isTurning = RotationSpeed > 0.1f;

            if (isMoving && !_wasMoving)
                onStartedMoving?.Invoke();

            _wasMoving = isMoving;

            CheckInteraction();
        }

        public void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                ClickToMove();
        }

        private void ClickToMove()
        {
            if (!mainCamera)
                return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, clickableLayers))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();

                if (interactable)
                {
                    _targetInteractable = interactable;
                    agent.SetDestination(interactable.seat.transform.position);
                }
                else
                {
                    _targetInteractable = null;
                    agent.SetDestination(hit.point);

                    DrawClickIndicator(hit);
                }

                if (!IsMoving)
                    animationManager.PlayAnimation(EnumAnimations.Walk);
            }
        }

        private void CheckInteraction()
        {
            if (!_targetInteractable)
                return;

            if (!agent.pathPending && agent.remainingDistance <= interactionRange)
            {
                agent.ResetPath();

                if (_targetInteractable.seat)
                {
                    transform.position = _targetInteractable.seat.position;
                    transform.rotation = _targetInteractable.seat.rotation;
                }

                _targetInteractable.Interact();
                _targetInteractable = null;
            }
        }

        private void DrawClickIndicator(RaycastHit hit)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.transform.position = hit.point + Vector3.up * 0.05f;
            marker.transform.localScale = Vector3.one * 0.2f;
            Destroy(marker.GetComponent<Collider>());
            Destroy(marker, 1f);
        }


        private void FaceTarget()
        {
            if (!agent.hasPath || agent.pathPending)
                return;

            if (agent.remainingDistance <= agent.stoppingDistance)
                return;

            Vector3 flatDir = agent.destination - transform.position;
            flatDir.y = 0f;

            if (flatDir.sqrMagnitude < 0.0001f)
                return;

            Quaternion lookRotation = Quaternion.LookRotation(flatDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }

        private bool AgentIsMoving()
        {
            return agent.hasPath && agent.velocity.sqrMagnitude > 0.01f;
        }

        private float CalculateRotationSpeed()
        {
            // Angle between current rotation and last rotation
            float angle = Quaternion.Angle(transform.rotation, _lastRotation);
            _lastRotation = transform.rotation;

            // Convert to degrees per second
            return angle / Time.deltaTime;
        }

        public bool IsMoving => AgentIsMoving();
    }
}