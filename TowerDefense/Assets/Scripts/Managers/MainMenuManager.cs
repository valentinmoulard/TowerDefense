using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_camaraReference = null;

    [SerializeField]
    private float m_cameraMoveSpeed = 5f;

    private Coroutine m_moveCameraCoroutineReference;

    private void Start()
    {
        if (m_camaraReference == null)
            Debug.LogError("The camera GO is missing in the inspector!");
    }

    public void PlayLevel(int levelIndex)
    {
        StopCoroutine(m_moveCameraCoroutineReference);
        SceneManager.LoadScene(levelIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MoveCameraTo(GameObject newLocation)
    {
        if (m_moveCameraCoroutineReference != null)
            StopCoroutine(m_moveCameraCoroutineReference);

        m_moveCameraCoroutineReference = StartCoroutine(MoveCameraCoroutine(newLocation.transform));
    }

    private IEnumerator MoveCameraCoroutine(Transform newPositionTransform)
    {
        while(Vector3.Distance(newPositionTransform.position, m_camaraReference.transform.position) > 0.1f)
        {
            m_camaraReference.transform.LerpTransform(newPositionTransform, Time.deltaTime * m_cameraMoveSpeed);
            yield return new WaitForEndOfFrame();
        }
    }

}
