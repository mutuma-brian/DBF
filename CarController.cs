using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;

public class CarController : MonoBehaviour
{
    public WheelCollider frontLeft, frontRight;
    public WheelCollider rearLeft, rearRight;
    public AudioSource introSong;
    public AudioSource collisionAudio;
    public AudioSource clickAudio;
    public AudioSource coinAudio;
    public AudioSource moanAudio;
    public AudioSource zombieAudio;
    public GameObject pausedTextMesh;
    public EngineSound engineSound;
    public TMP_Text scoreTextMesh;
    public Rigidbody rigidBody;
    public CinemachineVirtualCamera defaultCamera;
    public CinemachineVirtualCamera frontCamera;
    public CinemachineVirtualCamera reverseCamera;
    public CinemachineVirtualCamera upCamera;
    public AdsInitializer adss;

    public CinemachineVirtualCamera[] virtualCameras;

    public Transform Fr, Fl;
    public Transform Rr, Rl;

    private float m_horizontalInput;
    private float m_VerticalInput;
    private float m_steeringAngle;

    public string collisionTag = "collissionObject";
    public string coinTag = "CollectableGoldCoin";
    public string manTag = "Man";
    public string zombieTag = "Zombie";

    public float maxSteeringAngle = 6;
    public float motorForce = 500;
    public int speed = 0;
    private int maximumSpeed = 25;
    private float zeroBrake = 0f;
    private float zeroTorque = 0f;
    private int zeroTimeScale = 0;
    private int oneTimeScale = 1;

    public int cameraCount = 0;
    public int score = 0;
    public bool isBraking = false;
    private float speedLimit = 0.4f;


    public void Awake()
    {
        pausedTextMesh.SetActive(false);
        introSong.Play();
        clickAudio.Play();
    }

    public void Start()
    {
        virtualCameras = new CinemachineVirtualCamera[] { frontCamera, defaultCamera, reverseCamera, upCamera};
        frontCamera.MoveToTopOfPrioritySubqueue();
        
    }

    public void GetInput()
    {
        m_horizontalInput = SimpleInput.GetAxis("Horizontal");
        m_VerticalInput = SimpleInput.GetAxis("Vertical");
    }

    private void Steer()
    {
        if(m_horizontalInput >= speedLimit || m_horizontalInput <= speedLimit )
        {
        m_steeringAngle = maxSteeringAngle * m_horizontalInput;
        frontLeft.steerAngle = m_steeringAngle;
        frontRight.steerAngle = m_steeringAngle;
        }
    }

    private void Accelerate()
    {
        frontLeft.brakeTorque = zeroBrake;
        frontRight.brakeTorque = zeroBrake;
        rearRight.brakeTorque = zeroBrake;
        rearLeft.brakeTorque = zeroBrake;
        speed = (int)rigidBody.velocity.magnitude;
        if( speed < maximumSpeed)
        {
        frontLeft.motorTorque = m_VerticalInput * motorForce;
        frontRight.motorTorque = m_VerticalInput * motorForce;
        rearLeft.motorTorque = m_VerticalInput * motorForce;
        rearRight.motorTorque = m_VerticalInput * motorForce;
        }
        else
        {
        frontLeft.motorTorque = zeroTorque;
        rearLeft.motorTorque = zeroTorque;
        frontRight.motorTorque = zeroTorque;
        rearRight.motorTorque = zeroTorque;
        }  
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontLeft, Fl);
        UpdateWheelPose(frontRight, Fr);
        UpdateWheelPose(rearLeft, Rl);
        UpdateWheelPose(rearRight, Rr);

    }

    private void UpdateWheelPose(WheelCollider wheelColider, Transform transform)
    {
        Vector3 position = transform.position;
        Quaternion quaternion = transform.rotation;

        wheelColider.GetWorldPose(out position, out quaternion);

        transform.position = position;
        transform.rotation = quaternion;

    }

    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPoses(); 
    }

    public void PauseGame()
    {
        clickAudio.Play();
        if(Time.timeScale == oneTimeScale)
        {
            Time.timeScale = zeroTimeScale;
            engineSound.PauseAudio();
            pausedTextMesh.SetActive(true);
        }
        else if (Time.timeScale == zeroTimeScale )
        {
            pausedTextMesh.SetActive(false);
            engineSound.PlayAudio();
            Time.timeScale = oneTimeScale;
            adss.LoadInerstitialAd();
        }
       
        

    }
    public void BackToMenu()
    {
        clickAudio.Play();
        SceneManager.LoadScene("MenuScene");
       
    }

    public void SwitchVirtualCamera()
    {
        if(cameraCount<3)
        {
        clickAudio.Play();
        cameraCount++;
        virtualCameras[cameraCount].MoveToTopOfPrioritySubqueue();  
        }
        else 
        {
         clickAudio.Play();
         cameraCount=0;
         virtualCameras[cameraCount].MoveToTopOfPrioritySubqueue();  
        }  
        
    }

    public void RestartGame()
    {
        clickAudio.Play();
         SceneManager.LoadScene("SnowScene");
         if(Time.timeScale== zeroTimeScale)
         {
            pausedTextMesh.SetActive(true);
         }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(collisionTag))
        {
            collisionAudio.Play();
        }
        if (collision.gameObject.CompareTag(manTag))
        {
            moanAudio.Play();
            collisionAudio.Play();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag(zombieTag))
        {
            zombieAudio.Play();
            collisionAudio.Play();
            Destroy(collision.gameObject);
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(coinTag))
        {
            coinAudio.Play();
            score++;
            Destroy(collider.gameObject);
            scoreTextMesh.text = "Coins : " + score.ToString();
        }
    }

}