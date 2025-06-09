using CodeMonkey.Utils;
using UnityEngine;

public class WelcomeUI : MonoBehaviour
{
    [SerializeField] private Button_UI playBtn; 
    
    private void OnEnable()
    {
        playBtn.ClickFunc = () =>
        {
            Destroy(gameObject);
        };
    }
}
