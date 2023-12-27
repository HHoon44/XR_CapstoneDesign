using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoomEffect : MonoBehaviour
{
    public ParticleSystem boomEffect0;
    public ParticleSystem boomEffect1;
    public ParticleSystem boomEffect2;

    private Vector3 tempPosition0;
    private Vector3 tempPosition1;
    private Vector3 tempPosition2;

    public float testDuration;

    public void Initialize(int boomCol)
    {
        boomEffect0.transform.localPosition = new Vector3(boomCol, 0, 0);
        boomEffect1.transform.localPosition = new Vector3(boomCol, -7f, 0);
        boomEffect2.transform.localPosition = new Vector3(boomCol, -7f, 0);

        tempPosition0 = new Vector3(boomCol, -7.8f, 0);
        tempPosition1 = new Vector3(-3, -7f, 0);
        tempPosition2 = new Vector3(3, -7f, 0);
    }
    
    private void Update()
    {
        if (boomEffect0.gameObject.activeSelf)
        {
            boomEffect0.transform.localPosition = Vector3.Lerp(boomEffect0.transform.localPosition, tempPosition0, testDuration);
        }

        if (boomEffect1.gameObject.activeSelf)
        {
            boomEffect1.transform.localPosition = Vector3.Lerp(boomEffect1.transform.localPosition, tempPosition1, testDuration);
        }


        if (boomEffect2.gameObject.activeSelf)
        {
            boomEffect2.transform.localPosition = Vector3.Lerp(boomEffect2.transform.localPosition, tempPosition2, testDuration);
        }

        if (boomEffect0.transform.localPosition.y <= -7.6f)
        {
            boomEffect0.gameObject.SetActive(false);
        }

        if (boomEffect1.transform.localPosition.x <= -2.9f)
        {
            boomEffect1.gameObject.SetActive(false);
        }

        if (boomEffect2.transform.localPosition.x >= 2.9f)
        {
            boomEffect2.gameObject.SetActive(false);
        }

    }
}