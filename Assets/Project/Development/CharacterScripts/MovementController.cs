using System.Collections;
using UnityEngine;

namespace Project.Development.CharacterScripts
{
    public class MovementController : MonoBehaviour
    {
        public float MoveSpeed { get; set; }
        public float TurnSpeed { get; set; }
        
        public Vector3 Motion { get; set; }
        public Vector3 Rotation { get; set; }
        
        public Vector3 Gravity { get; set; }

        public bool IsMoving { get; private set; }
        public bool AlwaysGrounded { get; set; }

        public Vector3 Velocity => this.rigidbody.velocity;
        
        public bool InWater { get; private set; }
        
        public bool CanMove { get; set; }

        public bool IsGrounded
        {
            get => this.grounded || this.AlwaysGrounded;
            set => this.grounded = value;
        }
        
        public bool OnSlope { get; private set; }
        
        private bool grounded;
        
        private new Rigidbody rigidbody;

        private new Collider collider;

        private LayerMask checkGroundedMask;




        private void Awake()
        {
            this.rigidbody = GetComponent<Rigidbody>();
            this.collider = GetComponent<Collider>();
            this.rigidbody.freezeRotation = true;
            this.rigidbody.useGravity = false;
            this.MoveSpeed = CharacterRepository.Instance.MoveSpeed;
            this.TurnSpeed = CharacterRepository.Instance.TurnSpeed;
            this.Gravity = new Vector3(0, -30f, 0f);
            this.checkGroundedMask.value = ~(1 << LayerMask.NameToLayer("Character"));
        }
        
        private void FixedUpdate ()
        {
            if (Physics.Raycast(this.transform.position + Vector3.up * 0.5f, -Vector3.up, out var hit, 0.6f,
                this.checkGroundedMask))
            {
                OnSlope = Mathf.Abs(hit.normal.y) < 1;
            
                IsGrounded = true;
            }
            else
            {
                OnSlope = false;
            
                IsGrounded = false;
            }
            
            if (IsGrounded) 
            {
                if (this.Motion != Vector3.zero)
                {
                    Move(this.Motion, this.MoveSpeed);
                }
            }
            
            IsMoving = CanMove && this.Motion != Vector3.zero;

            if (!IsGrounded)
            {
                ApplyGravity();
            }

            if (!IsMoving && IsGrounded)
            {
                this.rigidbody.velocity = Vector3.zero;
                
                this.rigidbody.angularVelocity = Vector3.zero;
            }
            
            SetKinematic(!this.IsMoving && OnSlope);
            
            InWater = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                InWater = true;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
            {
                StartCoroutine(IgnoreCollision(0.1f,0.5f, other.collider));
            }
        }

        private void Update()
        {
            Rotate(this.Rotation, this.TurnSpeed);
        }

        private void OnDisable()
        {
            this.IsMoving = false;
        }




        public void Move(Vector3 dir, float speed)
        {
            if (!CanMove)
            {
                return;
            }
        
            var targetVelocity = dir * speed;
            
            var velocity = this.rigidbody.velocity;
            var velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -speed, speed);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -speed, speed);
            velocityChange.y = 0;
            this.rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        public void Rotate(Vector3 dir, float speed)
        {
            if (!CanMove || dir == Vector3.zero)
            {
                return;
            }
        
            var lookRotation = Quaternion.LookRotation(dir, Vector3.up);
			
            this.transform.rotation =
                Quaternion.Lerp(this.transform.rotation, lookRotation, speed * Time.deltaTime);
        }

        public void SetKinematic(bool state)
        {
            if (this.rigidbody.isKinematic == state)
            {
                return;
            }
                
            if (state)
            {
                this.rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
            
            this.rigidbody.isKinematic = state;

            if (!state)
            {
                this.rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            }
            
            this.rigidbody.velocity = Vector3.zero;
            this.rigidbody.angularVelocity = Vector3.zero;
        }

        public void SetColliderRadius(float radius)
        {
            if (this.collider is CapsuleCollider capsuleCollider)
            {
                capsuleCollider.radius = radius;
            }
        }
        
        

        private void ApplyGravity()
        {
            this.rigidbody.AddForce(this.Gravity * this.rigidbody.mass);
        }

        private IEnumerator IgnoreCollision(float delay, float duration, Collider collider)
        {
            yield return new WaitForSeconds(delay);
        
            Physics.IgnoreCollision(this.collider, collider, true);
            
            yield return new WaitForSeconds(duration);
            
            Physics.IgnoreCollision(this.collider, collider, false);
        }
    }
}
