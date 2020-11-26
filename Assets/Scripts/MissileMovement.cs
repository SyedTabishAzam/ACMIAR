using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileMovement : MonoBehaviour {

    private GameObject _target;
    public GameObject missileManager;
    public GameObject deadSymbolCubePrefab;
    private GameObject _missileCamera;
    private GameObject _launchingAircraft;
    public GameObject _resultTextObject;

    private float _velocity = 1.0f; //m/s
    private float _burnTime =-1.0f; //seconds
    private float _rotateSpeed = 50; //rad / s
    private float totalTerrainLength = 0.0f; //meters
    private float delta; //seconds
    private float timeSinceBirth = 0.0f; //seconds
    const float _decelerationRate = 0.9f;
    private float lastRecordedY = 0.0f;
    private float _focusDistance = 5;
    private bool isFollowingTarget = true;

    private Rigidbody rb;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        missileManager = GameObject.Find("MissileManager");

         _missileCamera = GameObject.Find("BottomPanel").transform.Find("CameraRenderTextureView").gameObject;
        if (_missileCamera)
            _missileCamera.SetActive(true);
    }

   

    // Update is called once per frame
    void FixedUpdate () {

        timeSinceBirth += Time.fixedDeltaTime;
        if (_target == null)
        {
            rb.velocity = transform.forward * delta;
            if (timeSinceBirth >= _burnTime)
            {
                Decelerate(Time.fixedDeltaTime);
                SwitchOffFlate();
                CheckDeath();
            }
            return;
        }

        var targetPoint = _target.transform.position;
        if (timeSinceBirth >= _burnTime)
        {
            Decelerate(Time.fixedDeltaTime);
            SwitchOffFlate();
            CheckDeath();
            targetPoint.y = lastRecordedY * 0.995f;
            lastRecordedY = targetPoint.y;
        }
        else
        {
            lastRecordedY = targetPoint.y ;
        }


        float distance = Vector3.Distance(transform.position, _target.transform.position);
        if (distance < _focusDistance)
        {
            isFollowingTarget = false;
        }

        if (isFollowingTarget)
        {
           
            var targetRotation = Quaternion.LookRotation(targetPoint - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime);
        }


        rb.velocity = transform.forward * delta ;
    }

    public void SwitchOffFlate()
    {
        ParticleSystem flare = transform.GetChild(1).GetComponent<ParticleSystem>();
        flare.Stop();
    }

    private void CheckDeath()
    {
        if ( transform.localPosition.y < 0.2f)
        {
            missileManager.GetComponent<MissileManager>().DisplayResultOnMap(false, null);
            Destroy(gameObject);
        }
    }
    public void Decelerate(float deltaTime)
    {
        Vector3 gravity = 9.8f * 0.01f * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.gameObject.Equals(_launchingAircraft) || other.tag != "Aircrafts")
            return;
        ApplyDeadSymbol(other.gameObject);


        if (_missileCamera)
            _missileCamera.SetActive(false);

        missileManager.GetComponent<MissileManager>().DisplayResultOnMap(true, other.transform.parent.gameObject);
        Destroy(gameObject);

    }

    private void ApplyDeadSymbol(GameObject hitTarget)
    {
        GameObject deadSymbolCube = Instantiate(deadSymbolCubePrefab, hitTarget.transform);
        Bounds bound = _target.GetComponent<MeshFilter>().mesh.bounds;
        deadSymbolCube.transform.localScale = new Vector3(bound.size.x, bound.size.y, bound.size.z);
        deadSymbolCube.name = "DeadSymbol";
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
    }

    public void SetVelocityPerSecond(float velocity)
    {
        _velocity = velocity;
        totalTerrainLength = MathCalculations.MaxBounds()[0];
        
        delta = (_velocity / totalTerrainLength) * 2f;
    }

    public void SetMinimumFocus(float meters)
    {
        _focusDistance = (meters / totalTerrainLength) * 2f;
    }

    public void SetBurnTime(float seconds)
    {
        _burnTime = seconds;
    }

    public void SetLaunchingAircraft(GameObject launchingAircraft)
    {
        _launchingAircraft = launchingAircraft;
    }
}
