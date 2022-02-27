using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class RotateSky : MonoBehaviour
{
    public Volume volume;
    public HDRISky sky = null;

    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<Volume>();
        VolumeProfile profile = volume.profile;
        if (!profile.TryGet<HDRISky>(out sky))
        {
            Debug.LogError("No HDRISky found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        sky.rotation.value += Time.deltaTime * 0.5f;
    }
}
