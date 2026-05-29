using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIWave : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI wavetext;

    private void OnEnable()
    {
        wavetext.text = $"Wave {(EnemyManager.Instance.CurrentWaveIndex + 1)}";
        StartCoroutine(WaitForDisable(2));
    }

    IEnumerator WaitForDisable(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }
}
