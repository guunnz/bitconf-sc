using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using TMPro;
public enum CameraType
{
    None = 0,
    Normal = 1,
    Passage = 2,
    Tunnel = 3,
    Puente = 4,
    Bossfight = 5,
    Intro = 6,
    BossfightFrenada = 7,
    PostBossfightFrenada = 8,
    CamaraChase = 9,
    Death = 10,
    CameraPostFrenada = 11,
    CameraStart = 12,
}

[System.Serializable]
public class VCam
{
    public CinemachineVirtualCamera Cam;
    public CameraType CamType;
    public bool DontChangeInBossFight;
    public bool DontChangeWhileChase;
    public float delayToChange;
}

public class CameraManager : MonoBehaviour
{
    public List<VCam> Cameras;
    static public CameraManager instance;
    public CameraType currentCam = CameraType.Normal;

    private float delay = 2;
    private float delayAux;

    public TextMeshProUGUI CameraText;
    public GameObject CameraUIHelper;

    private void Awake()
    {
        instance = this;
        delayAux = delay;
    }

    private void Start()
    {
        CameraText.text = "Cam: " + currentCam.ToString();

        Invoke("ChangeStartCamera", 0.2f);
    }


    public void ChangeStartCamera()
    {
        CameraManager.instance.ChangeCamera(CameraType.Normal);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            CameraUIHelper.SetActive(!CameraUIHelper.activeSelf);
        }
        if (currentCam != CameraType.Passage)
        {
            delayAux = delay;
            return;
        }

        delayAux -= Time.deltaTime;
        if (delayAux < 0)
        {
            delayAux = delay;
            ChangeCamera(CameraType.Normal);
        }

    }

    public void ChangeCamera(CameraType camType)
    {
        VCam cam = Cameras.Single(x => x.CamType == camType);
        if (cam.DontChangeInBossFight && PlayerBossBehaviour.instance.BossfightStarted)
            return;

        if (cam.DontChangeWhileChase && PlayerCollision.instance.isInjured)
        {
            return;
        }
        StartCoroutine(IChangeCamera(camType));
    }

    public IEnumerator IChangeCamera(CameraType camType)
    {
        VCam cam = Cameras.Single(x => x.CamType == camType);

        yield return new WaitForSecondsRealtime(cam.delayToChange);
        StopNoise();
        Cameras.ForEach(x => x.Cam.enabled = false);


        cam.Cam.enabled = true;
        currentCam = camType;
        CameraText.text = "Cam: " + currentCam.ToString();
    }


    public void DoNoise(float amplitudeGain, float frequencyGain, float duration)
    {
        StartCoroutine(Noise(amplitudeGain, frequencyGain, duration));
    }

    public IEnumerator Noise(float amplitudeGain, float frequencyGain, float duration)
    {
        VCam cam = Cameras.Single(x => x.CamType == currentCam);

        CinemachineBasicMultiChannelPerlin noise = cam.Cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;
        yield return new WaitForSeconds(duration);
        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
    }

    public void StopNoise()
    {
        VCam cam = Cameras.Single(x => x.CamType == currentCam);

        CinemachineBasicMultiChannelPerlin noise = cam.Cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
    }
}
