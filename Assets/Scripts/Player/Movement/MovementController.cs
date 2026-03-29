using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player.Movement
{
    public class MovementController : MonoBehaviour
    {
        [Header("References")]
        public static PlayerInput playerInput;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask clickableLayers;
        [SerializeField] private HumanoidAnimationManager anim;

        [Header("Settings")]
        [SerializeField] private float lookRotationSpeed = 5f;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float interactionRange = 2f;

        private Interactable targetInteractable;
        private bool isMoving;

        void Awake()
        {
            agent.updateRotation = false;
            agent.stoppingDistance = interactionRange;

            agent.speed = moveSpeed;
        }

        void Update()
        {
            FaceTarget();
            CheckInteraction();
            UpdateMovementAnimation();
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
                    targetInteractable = interactable;
                    anim.SitDown(false);
                    if (interactable.seat)
                        agent.SetDestination(interactable.seat.transform.position);
                    else if (interactable.standSlot)
                        agent.SetDestination(interactable.standSlot.transform.position);
                }
                else
                {
                    targetInteractable = null;
                    anim.SitDown(false);
                    agent.SetDestination(hit.point);

                    DrawClickIndicator(hit);
                }
            }
        }

        private void CheckInteraction()
        {
            if (!targetInteractable)
                return;

            if (!agent.pathPending && agent.remainingDistance <= interactionRange)
            {
                agent.ResetPath();

                if (targetInteractable.seat)
                {
                    transform.position = new Vector3(targetInteractable.seat.position.x, transform.position.y, targetInteractable.seat.position.z);
                    transform.rotation = targetInteractable.seat.rotation;

                    anim.SitDown(true);
                }
                if (targetInteractable.standSlot)
                {
                    transform.position = new Vector3(targetInteractable.standSlot.position.x, transform.position.y, targetInteractable.standSlot.position.z);
                    transform.rotation = targetInteractable.standSlot.rotation;
                }

                anim.PlayAnimation(EnumAnimations.Idle);

                targetInteractable.Interact();
                targetInteractable = null;
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

        private void UpdateMovementAnimation()
        {
            bool movingNow = agent.hasPath && !agent.pathPending && agent.remainingDistance > agent.stoppingDistance;

            if (movingNow && !isMoving)
            {
                isMoving = true;

                anim.SitDown(false);
                anim.PlayAnimation(EnumAnimations.Walk);
            }

            else if (!movingNow && isMoving)
            {
                isMoving = false;

                anim.PlayAnimation(EnumAnimations.Idle);
            }
        }
    }
}