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

        StartCoroutine(EndGame());
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(secondsBeforeEndingGame);
        SceneController.Instance.HomePage();
    }
}
