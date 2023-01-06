using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    [SerializeField] float secondsBeforeEndingGame = 2f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Config.Instance.playerTag) == false) { return; }

        PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
        if (!playerMovement) { return; }

        playerMovement.SetActionsLocked(true);
        Timer.Instance.Pause();

        SaveScore();

        StartCoroutine(EndGame());
    }

    private void SaveScore()
    {
        string currentTime = Timer.Instance.GetTimeString();

        if (!PlayerPrefs.HasKey("bestTime"))
        {
            PlayerPrefs.SetString("bestTime", currentTime);
            PlayerPrefs.Save();
            return;
        }

        if (Timer.Instance.GetTimeFloat() > PlayerPrefs.GetFloat("bestTime"))
        {
            PlayerPrefs.SetString("bestTime", currentTime);
            PlayerPrefs.Save();
        }
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(secondsBeforeEndingGame);
        SceneController.Instance.HomePage();
    }
}
