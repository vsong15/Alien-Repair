using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextDisplay : MonoBehaviour
{
    public FloatVariable m_TimeRemainingSO;
    public FloatVariable m_AirRemainingSO;
    public TextMeshProUGUI m_AirMeter;
    public TextMeshProUGUI m_TimeMeter;


    private void Update()
    {
        m_AirMeter.text = Mathf.RoundToInt(m_AirRemainingSO.currentValue*100).ToString() + "%";
        m_TimeMeter.text = Mathf.RoundToInt(m_TimeRemainingSO.currentValue).ToString() + "s";

    }

}
