using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viapix_Movement
{
    public class Viapix_PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        float speedX;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(Input.GetAxis("Horizontal") * speedX * Time.deltaTime, 0, 0);

        }
    }
}

