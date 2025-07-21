using Viapix_PlayerParams;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Viapix_UI
{
    public class Viapix_UI_PlayerHP : MonoBehaviour
    {
        Viapix_PlayerHP playerHP;
        TextMeshProUGUI textMeshProUGUI;

        // Start is called before the first frame update
        void Start()
        {
            playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<Viapix_PlayerHP>();
            textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            textMeshProUGUI.text = "Player's HP: " + playerHP.playerHP;
        }
    }
}

