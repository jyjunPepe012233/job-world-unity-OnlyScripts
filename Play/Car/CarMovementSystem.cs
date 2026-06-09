using UnityEngine;

namespace Jobworld.CarSystems
{
    /// <summary>
    /// 자동차 움직임과 물리를 독립적으로 처리하는 시스템
    /// </summary>
    public class CarMovementSystem : MonoBehaviour
    {
        [Header("이동 설정")]
        [SerializeField] private float maxSpeed = 15f;
        [SerializeField] private float maxReverseSpeed = 5f;
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float brakePower = 10f;

        [Header("조향 설정")]
        [SerializeField] private float turnSpeed = 25f;
        [SerializeField] private float steeringSmoothingCar = 4f;

        [Header("물리 설정")]
        [SerializeField] private float speedSmoothing = 3f;

        private Rigidbody carRigidbody;
        private float currentSpeed = 0f;
        private float targetSpeed = 0f;
        private float currentSteeringAngle = 0f;

        public float CurrentSpeed => currentSpeed;
        public float CurrentSteeringAngle => currentSteeringAngle;

        public void Initialize()
        {
            carRigidbody = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
            ConfigureRigidbody();
        }

        public void ProcessInput(CarInputData input)
        {
            currentSteeringAngle = input.SteeringInput;
            CalculateTargetSpeed(input);
            UpdateCurrentSpeed();
        }

        public void ApplyPhysics()
        {
            if (Mathf.Abs(currentSpeed) > 0.01f)
            {
                ApplyMovementForces();
                ApplySteeringForces();
            }
            else
            {
                ApplyStoppingForces();
            }
        }

        public void ResetState()
        {
            currentSpeed = 0f;
            targetSpeed = 0f;
            currentSteeringAngle = 0f;
            
            if (carRigidbody != null)
            {
                carRigidbody.velocity = Vector3.zero;
                carRigidbody.angularVelocity = Vector3.zero;
            }
        }

        public void ApplyNetworkData(float networkSpeed, float networkSteeringAngle)
        {
            currentSpeed = networkSpeed;
            currentSteeringAngle = networkSteeringAngle;
        }

        private void ConfigureRigidbody()
        {
            carRigidbody.mass = 100f;
            carRigidbody.drag = 0.1f;
            carRigidbody.angularDrag = 5f;
            carRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            carRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            carRigidbody.centerOfMass = Vector3.down * 0.5f;
        }

        private void CalculateTargetSpeed(CarInputData input)
        {
            if (input.AccelerateInput > 0f)
            {
                targetSpeed = maxSpeed * input.AccelerateInput;
            }
            else if (input.BrakeInput > 0f)
            {
                if (currentSpeed > 0.1f)
                {
                    targetSpeed = Mathf.Max(0f, currentSpeed - brakePower * input.BrakeInput);
                }
                else if (Mathf.Abs(currentSpeed) < 0.1f)
                {
                    targetSpeed = -maxReverseSpeed * input.BrakeInput * 0.6f;
                }
                else
                {
                    targetSpeed = -maxReverseSpeed * input.BrakeInput;
                }
            }
            else
            {
                targetSpeed = 0f;
            }
        }

        private void UpdateCurrentSpeed()
        {
            var speedChange = acceleration * Time.deltaTime;
            
            if (currentSpeed < targetSpeed)
            {
                currentSpeed = Mathf.Min(currentSpeed + speedChange, targetSpeed);
            }
            else if (currentSpeed > targetSpeed)
            {
                currentSpeed = Mathf.Max(currentSpeed - speedChange, targetSpeed);
            }

            currentSpeed = Mathf.Clamp(currentSpeed, -maxReverseSpeed, maxSpeed);
        }

        private void ApplyMovementForces()
        {
            var forwardMovement = transform.forward * currentSpeed;
            var newVelocity = new Vector3(forwardMovement.x, carRigidbody.velocity.y, forwardMovement.z);
            carRigidbody.velocity = Vector3.Lerp(carRigidbody.velocity, newVelocity, 10f * Time.fixedDeltaTime);
        }

        private void ApplySteeringForces()
        {
            if (Mathf.Abs(currentSteeringAngle) > 0.01f)
            {
                var speedFactor = Mathf.Abs(currentSpeed) / maxSpeed;
                var turnRate = currentSteeringAngle * turnSpeed * speedFactor;

                if (currentSpeed < 0) turnRate *= -1;

                var torque = Vector3.up * turnRate;
                carRigidbody.AddTorque(torque * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
            else
            {
                carRigidbody.angularVelocity = Vector3.Lerp(carRigidbody.angularVelocity, Vector3.zero, 
                    5f * Time.fixedDeltaTime);
            }
        }

        private void ApplyStoppingForces()
        {
            var stoppedVelocity = new Vector3(0f, carRigidbody.velocity.y, 0f);
            carRigidbody.velocity = Vector3.Lerp(carRigidbody.velocity, stoppedVelocity, 5f * Time.fixedDeltaTime);
            carRigidbody.angularVelocity = Vector3.Lerp(carRigidbody.angularVelocity, Vector3.zero, 
                8f * Time.fixedDeltaTime);
        }
    }
}