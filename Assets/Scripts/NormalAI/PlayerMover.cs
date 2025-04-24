using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public static PlayerMover Instance;
    public static bool IsFinished => Instance.m_IsFinished;

    public float MoveInterval = 0.35f;
    private bool m_IsFinished = true;

    private void Awake()
    {
        Instance = this;
    }

    public static void SetRoute(List<Vector2Int> path)
    {
        Instance.StartCoroutine(MovePlayer(path));
    }

    private static IEnumerator MovePlayer(List<Vector2Int> path)
    {
        Instance.m_IsFinished = false;

        int index = 0;
        PlayerController controller = PlayerController.Instance;

        while(index < path.Count)
        {
            if (!controller.TryMove(path[index]))
            {
                Debug.LogWarning($"Can't move to {path[index]}");
                break;
            }

            yield return new WaitForSeconds(Instance.MoveInterval);

            index++;
        }

        Instance.m_IsFinished = true;
    }
}
