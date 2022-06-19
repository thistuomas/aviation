using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controls : MonoBehaviour
{
    public float weightAirelions
    {
        get { return _weightAirelions; }
        set { _weightAirelions = Mathf.Clamp(value, -1, 1); }
    }
    private float _weightAirelions;

    public float weightElevator
    {
        get { return _weightElevator; }
        set { _weightElevator = Mathf.Clamp(value, -1, 1); }
    }
    private float _weightElevator;

    public float weightRudder
    {
        get { return _weightRudder; }
        set { _weightRudder = Mathf.Clamp(value, -1, 1); }
    }
    private float _weightRudder;

    [SerializeField]
    public float acceleration
    {
        get { return _acceleration; }
        set { _acceleration = Mathf.Clamp(value, 0, 100); }
    }
    private float _acceleration;

    public float thrust
    {
        get { return _thrust; }
        set { _thrust = Mathf.Clamp(value, 0, 20); }
    }
    private float _thrust;

    [SerializeField]
    private SkinnedMeshRenderer airelions;
    [SerializeField]
    private SkinnedMeshRenderer elevator;
    [SerializeField]
    private SkinnedMeshRenderer rudder;
    [SerializeField]
    private GameObject propeller;

    private Animation flaps;
    
    private Rigidbody rb;
    
    private float pitch;
    private float yaw;
    private float roll;
    private float throttle;

    private int CurrentSpeed;
    private float LiftNotToFall = 1;
    private float GravityApplied;
    private float angle;

    private bool flapsActivated;
    
    private float lift;
    private float liftMultiplier = 1f;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        flaps = gameObject.GetComponent<Animation>();
    }

    void Start()
    {
        rb.useGravity = true;
        flapsActivated = false;
    }

    void Update()
    {
        Inputs();

        CalculateWeights();
        SetWeights();

        CalculateLift();
        CalculateForces();
        SetForces();
    }

    void Inputs()
    {
        pitch = Input.GetAxis("Mouse Y");
        yaw = Input.GetAxis("Horizontal");
        roll = Input.GetAxis("Mouse X");
        throttle = Input.GetAxisRaw("Vertical");
    }

    void CalculateWeights()
    {
        weightElevator = (pitch != 0) ? weightElevator + pitch * 0.1f : weightElevator - weightElevator * Time.deltaTime;
        weightRudder = (yaw != 0) ? weightRudder + yaw * 0.1f : weightRudder - weightRudder * Time.deltaTime;
        weightAirelions = (roll != 0) ? weightAirelions + roll * 0.1f : weightAirelions - weightAirelions * Time.deltaTime;
    }

    void SetWeights()
    {
        airelions.SetBlendShapeWeight(0, weightAirelions * 100);
        elevator.SetBlendShapeWeight(0, weightElevator * 100);
        rudder.SetBlendShapeWeight(0, weightRudder * 100);
    }

    void CalculateLift()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !flaps.isPlaying)
        {
            if (!flapsActivated)
            {
                liftMultiplier = 1.2f;
                flaps.Play("flaps|activate");
                flapsActivated = true;
            }
            else
            {
                liftMultiplier = 1.0f;
                flaps.Play("flaps|deactivate");
                flapsActivated = false;
            }
        }
    }

    void CalculateForces()
    {
        thrust += throttle * Time.deltaTime * 4;
        angle -= transform.forward.y * Time.deltaTime * 9.8f;
        acceleration = thrust + angle;
        lift = acceleration * 0.1f * liftMultiplier;
    }

    
    void SetForces()
    {
        transform.position += transform.forward * acceleration * Time.deltaTime;
        transform.position += transform.up * lift * Time.deltaTime;
        transform.Rotate(pitch * acceleration * 0.01f, yaw * acceleration * 0.01f, roll * acceleration * 0.01f);
        propeller.transform.Rotate(0, 0, thrust);
    }
}