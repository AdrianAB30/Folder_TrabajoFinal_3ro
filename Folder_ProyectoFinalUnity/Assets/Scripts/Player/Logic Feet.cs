using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicFeet : MonoBehaviour
{
    [SerializeField] private PlayerController logicPlayer;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Ground")) 
        {
            logicPlayer.canJump = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground")) 
        {
            logicPlayer.canJump = false;
        }
    }
}
