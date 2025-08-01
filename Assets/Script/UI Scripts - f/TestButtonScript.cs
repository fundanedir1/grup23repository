using UnityEngine;
using UnityEngine.UI; 

public class TestButtonScript : MonoBehaviour
{
   
    void Start()
    {
        Debug.Log("TestButtonScript baslatildi.");
    }

    public void OnTestButtonClick()
    {
        Debug.Log("Buton tiklandi!");
    }
}