using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CameraMode
{
    Fixed,
    OverTheShoulder,
    FirstPerson,
}

public class CameraManager : Singleton<CameraManager> {

    private Camera fixedCamera;
    private Camera overTheShoulderCamera;
    private Player player;

    private Vector3 offsetFixedCameraFromPlayer = Vector3.zero;

    private CameraMode _currentCameraMode = CameraMode.Fixed;
    public CameraMode CurrentCameraMode 
    { 
        get { return _currentCameraMode; } 
        set
        {
            _currentCameraMode = value;
            if (value == CameraMode.Fixed)
            {
                overTheShoulderCamera.enabled = false;
                fixedCamera.enabled = true;
            }
            else if (value == CameraMode.OverTheShoulder)
            {
                fixedCamera.enabled = false;
                overTheShoulderCamera.enabled = true;
            }
            else
            {
                // TODO: first person
            }
        }
    }

    private int _currentCameraDistance = 2;
    public int CurrentCameraDistance
    { 
        get { return _currentCameraDistance; } 
        set
        {
            _currentCameraDistance = value;
            if (CurrentCameraMode == CameraMode.Fixed)
            {
                // TODO: get camera positions from SerializeField placeholders
                if (_currentCameraDistance == 1)
                {
                    offsetFixedCameraFromPlayer = new Vector3(0, 3, -7);
                    fixedCamera.transform.rotation = Quaternion.Euler(new Vector3(10, 0, 0));
                }
                else if (_currentCameraDistance == 2)
                {
                    offsetFixedCameraFromPlayer = new Vector3(0, 10, -12);
                    fixedCamera.transform.rotation = Quaternion.Euler(new Vector3(30, 0, 0));
                }
                else if (_currentCameraDistance == 3)
                {
                    offsetFixedCameraFromPlayer = new Vector3(0, 15, -10);
                    fixedCamera.transform.rotation = Quaternion.Euler(new Vector3(50, 0, 0));
                }
                else if (_currentCameraDistance == 4)
                {
                    offsetFixedCameraFromPlayer = new Vector3(0, 25, -10);
                    fixedCamera.transform.rotation = Quaternion.Euler(new Vector3(70, 0, 0));
                }
            }
            else if (CurrentCameraMode == CameraMode.OverTheShoulder)
            {
                // TODO: get camera positions from SerializeField placeholders
                if (_currentCameraDistance == 1)
                {
                    overTheShoulderCamera.transform.localPosition = new Vector3(0, 4, -4);
                    overTheShoulderCamera.transform.localRotation = Quaternion.Euler(new Vector3(10, 0, 0));
                }
                else if (_currentCameraDistance == 2)
                {
                    overTheShoulderCamera.transform.localPosition = new Vector3(0, 10, -7);
                    overTheShoulderCamera.transform.localRotation = Quaternion.Euler(new Vector3(30, 0, 0));
                }
                else if (_currentCameraDistance == 3)
                {
                    overTheShoulderCamera.transform.localPosition = new Vector3(0, 15, -5);
                    overTheShoulderCamera.transform.localRotation = Quaternion.Euler(new Vector3(50, 0, 0));
                }
                else if (_currentCameraDistance == 4)
                {
                    overTheShoulderCamera.transform.localPosition = new Vector3(0, 25, 0);
                    overTheShoulderCamera.transform.localRotation = Quaternion.Euler(new Vector3(70, 0, 0));
                }
            }
            else
            {
                // TODO: first person
            }
        }
    }



	// Use this for initialization
	void Start() 
    {
        fixedCamera = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        overTheShoulderCamera = player.GetComponentInChildren<Camera>();
        
        CurrentCameraMode = CameraMode.Fixed;
        CurrentCameraDistance = 2;
	}
	
	// Update is called once per frame
	void Update()
    {
        CheckInputForChangeCameraMode();
	}

    void LateUpdate()
    {
        PositionFixedCamera();
    }

    private void PositionFixedCamera()
    {
        fixedCamera.transform.position = player.transform.position + offsetFixedCameraFromPlayer;
    }

    private void CheckInputForChangeCameraMode()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            CurrentCameraMode = CameraMode.OverTheShoulder;
            CurrentCameraDistance = 1;
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            CurrentCameraMode = CameraMode.OverTheShoulder;
            CurrentCameraDistance = 2;
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            CurrentCameraMode = CameraMode.OverTheShoulder;
            CurrentCameraDistance = 3;
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            CurrentCameraMode = CameraMode.OverTheShoulder;
            CurrentCameraDistance = 4;
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            CurrentCameraMode = CameraMode.Fixed;
            CurrentCameraDistance = 1;
        }
        else if (Input.GetKeyDown(KeyCode.F6))
        {
            CurrentCameraMode = CameraMode.Fixed;
            CurrentCameraDistance = 2;
        }
        else if (Input.GetKeyDown(KeyCode.F7))
        {
            CurrentCameraMode = CameraMode.Fixed;
            CurrentCameraDistance = 3;
        }
        else if (Input.GetKeyDown(KeyCode.F8))
        {
            CurrentCameraMode = CameraMode.Fixed;
            CurrentCameraDistance = 4;
        }

    }


}
