using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCaseModelAnim : MonoBehaviour
{
    public Vector3 SwingRange;
    public Vector3 RotateEulerSpeed;
    public float SwingSpeed;

    Vector3 IniPos;
    Vector3 IniEuler;
    void Start()
    {
        IniPos = transform.position;
        IniEuler = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = IniPos + SwingRange * Mathf.Sin( SwingSpeed * Time.time);
        transform.eulerAngles = IniEuler + RotateEulerSpeed * Time.time;
    }
}
