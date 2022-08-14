using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class Movement : NetworkBehaviour
    {
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private float dashSpeed;
        [SerializeField] private AnimationCurve dashSpeedCurve;
        [SerializeField] private float dashTime = 0.5f;
        [SerializeField] private PlayerInfo playerInfo;
        [SerializeField] private Transform model;
        [SerializeField] private UnityEvent OnStartDash;
        [SerializeField] private UnityEvent OnEndDash;

        private Rigidbody _rb;
        private bool _isDashing;

        #region MONO

        private void Start()
        {
            if (!playerInfo.hasAuthority)
            {
                Destroy(this);
            }
            else
            {
                _rb = GetComponent<Rigidbody>();
            }
        }

        #endregion

        private void Update()
        {
            var directionX = Input.GetAxis("Horizontal");
            var directionZ = Input.GetAxis("Vertical");
            var moveDirection = model.TransformDirection(new Vector3(directionX, 0, directionZ));

            Move(moveDirection, movementSpeed);

            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(Dash(model.forward));
            }
        }

        private void Move(Vector3 direction, float speed)
        {
            if (direction == Vector3.zero) return;
            if (_isDashing) return;

            ApplyVelocity(direction, speed);
        }

        private IEnumerator Dash(Vector3 direction)
        {
            
            if (_isDashing) yield break;

            _isDashing = true;
            OnStartDash.Invoke();
            var elapsedTime = 0f;
            while (elapsedTime < dashTime)
            {
                var velocityMultiplier = dashSpeed * dashSpeedCurve.Evaluate(elapsedTime);

                ApplyVelocity(direction, velocityMultiplier);

                elapsedTime += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            _isDashing = false;
            OnEndDash.Invoke();
            yield break;
        }

        private void ApplyVelocity(Vector3 desiredVelocity, float multiplier)
        {
            var velocity = _rb.velocity;

            velocity.y = desiredVelocity.y == 0 ? velocity.y : desiredVelocity.y * multiplier;
            velocity.x = desiredVelocity.x * multiplier;
            velocity.z = desiredVelocity.z * multiplier;

            _rb.velocity = velocity;
        }
    }
}

