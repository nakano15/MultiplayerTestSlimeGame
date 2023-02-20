using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    static CameraScript Self;
    [SerializeField] CharacterScript CharacterToFollow;

    private void Awake()
    {
        Self = this;
    }

    public static void ChangeCameraTarget(CharacterScript NewCharacter)
    {
        Self.CharacterToFollow = NewCharacter;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (CharacterToFollow) transform.position = CharacterToFollow.GetSpritePosition + Vector3.back * 10;
    }
}
