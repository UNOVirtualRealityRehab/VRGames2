﻿using System;
using System.Collections;
using Firesplash.UnityAssets.SocketIO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Classes.Managers;
using System.Globalization;


namespace Network
{
    [RequireComponent(typeof(SocketIOCommunicator))]
    public class NetworkManager : MonoBehaviour
    {
        private SocketIOCommunicator _communicator;
        private SocketIOInstance _socket;
        public static NetworkManager Instance { get; private set; }
        private string clinicianId;
    
        [SerializeField] private TMP_InputField patientNameInput;
        [SerializeField] private TextMeshProUGUI patientName;
        [SerializeField] private TextMeshProUGUI clinicianName;
        [SerializeField] private GameObject setName;
        [SerializeField] private TextMeshProUGUI connText;
        [SerializeField] private GameObject keyboard;

        private void Awake()
        { 
            DontDestroyOnLoad(this);
            _communicator = GetComponent<SocketIOCommunicator>();
            _socket = _communicator.Instance;

            if (Instance != null && Instance != this)
            {
                Destroy(this);
            } else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            _socket.Connect();
            HandleServerEvents();
            StartCoroutine(PatientConnect());
            SetPatientName("Patient");
        }

        public static NetworkManager getManager() { return Instance; }

        private IEnumerator PatientConnect()
        {
            yield return new WaitUntil(() => _socket.IsConnected());
            FetchName();
        }


        private void FetchName()
        {
            if (PlayerPrefs.HasKey("PatientName"))
            {
                var name = PlayerPrefs.GetString("PatientName");
                Debug.Log("Has Name: " + name);
                _socket.Emit("unityConnect", name, true);
                patientName.SetText(name);
                SwitchToViewName();
            }
            else
            {
                SwitchToSetName();
            }
        }
        
        private void HandleServerEvents()
        {
             _socket.On("userJoined", (string payload) =>
            {
                Debug.Log("Joined Room!" + payload);
                var obj = JsonConvert.DeserializeObject<SocketClasses.JoinRoom>(payload);
                Debug.Log(obj.clinician);
                clinicianName.SetText(obj.clinician);
                connText.gameObject.SetActive(true);
            });
             _socket.On("startGame", (string payload) =>
             {
                 Debug.Log("Started Game");
                 var obj = JsonConvert.DeserializeObject<SocketClasses.StartGame>(payload);
                 Debug.Log(obj.game);
                 switch(obj.game) {
                    case "3":
                        SceneManager.LoadScene("Planes");
                        break;
                    case "2":
                        SceneManager.LoadScene("Balloons");
                        break;
                    case "1":
                        SceneManager.LoadScene("Blocks");
                        break;
                    default:
                        SceneManager.LoadScene("Initialize");
                        break;
                 }
             });

             _socket.On("pauseGame", (string payload) => {
                GameplayManager.getManager().ResumeGame();
                Debug.Log("Unpaused");             
            });

             _socket.On("resumeGame", (string payload) => {
                GameplayManager.getManager().PauseGame();
                Debug.Log("Paused");
             });

             _socket.On("updatePatientPosition", (string payload) => {
                 Debug.Log(payload);
                 var axisPosition = JsonConvert.DeserializeObject<SocketClasses.Position>(payload);
                 PlayerManager.movePlayerX(axisPosition.playerXPosition);
                 PlayerManager.movePlayerY(axisPosition.playerYPosition);
                 PlayerManager.movePlayerZ(axisPosition.playerZPosition);
             });

            _socket.On("kickPatient", (string payload) => {
                Debug.Log(payload);
                Destroy(this);
                SceneManager.LoadScene("Initialize");
            });

             _socket.On("handMirror", (string payload) => {
                Debug.Log(payload);
                switch (payload) {
                    case "LEFT":
                        CalibrationManager.Instance?.Calibrate(CalibrationManager.CalibrationType.Mirror, OVRInput.Controller.LTouch);
                        break;
                    case "RIGHT":
                        CalibrationManager.Instance?.Calibrate(CalibrationManager.CalibrationType.Mirror, OVRInput.Controller.RTouch);
                        break;
                    default:
                        CalibrationManager.Instance?.Reset();
                        break;
                }                
             });
             
             _socket.On("IKRig", (string payload) => {
                var ikRigMeasurements = JsonConvert.DeserializeObject<SocketClasses.IKRig>(payload);
                SetIKTunning(ikRigMeasurements.shoulderWidth, ikRigMeasurements.headHeight, ikRigMeasurements.armLength, ikRigMeasurements.extendedArmThreshold, ikRigMeasurements.retractedArmThreshold);
             });

             _socket.On("toggleSkeleton", (string payload) => {
                PlayerManager.ToggleIKSkeleton();
             });
             
             _socket.On("handScale", (string payload) => {
                Debug.Log(payload);
                var handScale = JsonConvert.DeserializeObject<SocketClasses.HandScaling>(payload);
                Debug.Log("Scaling: " + handScale.handToScale + "\nAmount: " + handScale.scaleAmount);
                switch (handScale.handToScale)
                {
                    case "LEFT":
                        CalibrationManager.Instance?.Calibrate(CalibrationManager.CalibrationType.Scale, OVRInput.Controller.LTouch);
                        CalibrationManager.Instance?.changeScaleAmt(handScale.scaleAmount);
                        break;
                    case "RIGHT":
                        CalibrationManager.Instance?.Calibrate(CalibrationManager.CalibrationType.Scale, OVRInput.Controller.RTouch);
                        CalibrationManager.Instance?.changeScaleAmt(handScale.scaleAmount);
                        break;
                    default:
                        CalibrationManager.Instance?.Reset();
                        break;
                }
             });
            _socket.On("changePatientName", (string payload) =>
            {
                Debug.Log("Changesd name to: " + payload);
                PlayerPrefs.SetString("PatientName", payload);
                patientName.SetText(payload);
                SwitchToViewName();
            });

            _socket.On("setClinicianID", (string payload) =>
            {
                clinicianId = payload;
            });
        }
        
        private void OnDestroy()
        {
            _socket.Close();
        }

        public void SetPatientName()
        {
            
            PlayerPrefs.SetString("PatientName", patientNameInput.text);
            FetchName();
        }
        void SetPatientName(string name)
        {
            
            PlayerPrefs.SetString("PatientName", name);
            FetchName();
        }

        public void SwitchToSetName()
        {
            Debug.Log("SetName!");
            setName.SetActive(true);
            patientName.gameObject.SetActive(false);
            keyboard.SetActive(true);
        }

        private void SwitchToViewName()
        {
            setName.SetActive(false);
            patientName.gameObject.SetActive(true);
            keyboard.SetActive(false);
        }

        private void SetIKTunning(float shoulderWidth, float headHeight, float armLength, float extendedArmThreshold, float retractedArmThreshold)
        {
            Debug.Log("Shoulder Width: " + shoulderWidth);
            Debug.Log("Head Height: " + headHeight);
            Debug.Log("Arm Length: " + armLength);
            Debug.Log("Extended Arm Threshold: " + extendedArmThreshold);
            Debug.Log("Retracted Arm Threshold: " + retractedArmThreshold);
            PlayerManager.SetIKShoulderWidth(shoulderWidth);
            PlayerManager.SetIKHeadHeight(headHeight);
            PlayerManager.SetIKArmLength(armLength);
            PlayerManager.SetExtendArmThreshold(extendedArmThreshold);
            PlayerManager.SetRetractArmThreshold(retractedArmThreshold);
        }

        public void SendPositionalData(string message)
        {
            _socket.Emit("positionalDataServer", clinicianId + ":" + message, true);
        }

        public void SendRepTrackingData(string message)
        {
            _socket.Emit("repTrackingDataServer", clinicianId + ":" + message, true);
        }
    }
}
