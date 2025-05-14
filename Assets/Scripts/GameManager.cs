using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static bool isDevMode = false;
    public void ToMenu()
    {


        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
