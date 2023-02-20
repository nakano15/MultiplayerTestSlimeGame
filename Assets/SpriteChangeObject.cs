using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChangeObject : MonoBehaviour
{
    [SerializeField] AppearanceUIScript appearance;

    void Interact(CharacterScript character)
    {
        appearance.gameObject.SetActive(true);
    }
}
