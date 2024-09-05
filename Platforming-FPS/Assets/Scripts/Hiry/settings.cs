using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settings : MonoBehaviour
{
    [SerializeField] cameraController cameraController_Damon;
    [SerializeField] playerController playerController_Damon;

    [SerializeField] Slider sensSlider;
    [SerializeField] Toggle sprintToggle;
    [SerializeField] Toggle invertYToggle;
    [SerializeField] Slider fovSlider;



    // Start is called before the first frame update
    void Start()
    {

        cameraController_Damon.SetInvertY(false);
        playerController_Damon.SetSprintToggle(false);
        invertYToggle.isOn = false;
        sprintToggle.isOn = false;

        sensSlider.value = cameraController_Damon.sens;
        sensSlider.onValueChanged.AddListener(cameraController_Damon.setSens);

        invertYToggle.isOn = cameraController_Damon.invertY;
        invertYToggle.onValueChanged.AddListener(cameraController_Damon.SetInvertY);

        sprintToggle.isOn = playerController_Damon.sprintToggle;
        sprintToggle.onValueChanged.AddListener(playerController_Damon.SetSprintToggle);

        fovSlider.minValue = cameraController_Damon.minFOV;
        fovSlider.maxValue = cameraController_Damon.maxFOV;
        fovSlider.value = cameraController_Damon.playerCamera.fieldOfView;
        fovSlider.onValueChanged.AddListener(cameraController_Damon.SetFOV);



    }

    
   
}
