using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerScript : CharacterScript
{
    public static PlayerScript PlayerCharacter;

    [SerializeField] Sprite[] Faces;
    [SerializeField] Sprite[] Bodies;
    [SerializeField] SpriteRenderer FaceRenderer;
    byte FaceID;
    byte BodyID;
    public HDColor ColorSet = new HDColor();
    [SerializeField] AnimationType AnimType = AnimationType.NotInitialized;
    [SerializeField] float AnimationTime = 0, AnimationMaxTime = 1;
    [SerializeField] AudioClip[] HopSounds;

    public byte GetFaceID { get { return FaceID; } }
    public byte GetBodyID { get { return BodyID; } }

    public void ChangeFace(byte ID)
    {
        FaceID = (byte)Mathf.Clamp(ID, 0, Faces.Length - 1);
        FaceRenderer.sprite = Faces[ID];
    }

    public void ChangeBody(byte ID)
    {
        BodyID = (byte)Mathf.Clamp(ID, 0, Bodies.Length - 1);
        Renderer.sprite = Bodies[ID];
    }

    public int GetBodyTypeCount { get { return Bodies.Length; } }
    public int GetFaceTypeCount { get { return Bodies.Length; } }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        UpdatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        if (IsOwner) UpdateControls();
        UpdateAttack();
        UpdateAnimation();
    }

    public void UpdateControls()
    {
        int Horizontal = (int)(Input.GetAxis("Horizontal"));
        int Vertical = (int)(Input.GetAxis("Vertical"));
        if (Input.GetButtonDown("Fire1"))
            Interact();
        if (Horizontal != 0 || Vertical != 0)
        {
            if (MoveCharacter(Horizontal, Vertical))
                AudioSource.PlayClipAtPoint(HopSounds[Random.Range(0, HopSounds.Length)], transform.position, 0.8f);
            transform.localScale = new Vector3(FacingLeft ? -1 : 1, 1, 1);
        }
    }

    private void UpdateAnimation()
    {
        AnimationType NewAnimation = AnimationType.Stopped;
        if (AttackTime > 0)
            NewAnimation = AnimationType.Attacking;
        else if (MoveDirection.x != 0 || MoveDirection.y != 0)
            NewAnimation = AnimationType.Moving;
        if (NewAnimation != AnimType)
        {
            AnimationTime = 0;
            switch(NewAnimation)
            {
                case AnimationType.Stopped:
                    AnimationMaxTime = 0.6f;
                    break;
                case AnimationType.Moving:
                    AnimationMaxTime = 0.05f;
                    break;
            }
            AnimType = NewAnimation;
        }
        switch (AnimType)
        {
            case AnimationType.Stopped:
                {
                    float Value = Mathf.Sin((AnimationTime / AnimationMaxTime) * (360 * Mathf.Deg2Rad));
                    Vector3 NewScale = new Vector3(0.02f, 0.03f) * Value;
                    SpritesContainer.localScale = Vector3.one + NewScale;
                    SpritesContainer.transform.localPosition = Vector3.zero;
                }
                break;
            case AnimationType.Moving:
                {
                    float Value = (MoveDirection.x != 0 ? Mathf.Abs(MoveDirection.x) : Mathf.Abs(MoveDirection.y)) / 0.4f; 
                    Value = Mathf.Sin(Value * (180 * Mathf.Deg2Rad));
                    Vector3 NewScale = new Vector3(-0.3f, 0.4f) * Value;
                    SpritesContainer.localScale = Vector3.one + NewScale;
                    SpritesContainer.transform.localPosition = SpritesContainer.transform.localPosition + Vector3.up * 0.2f * Value;
                }
                break;
            case AnimationType.Attacking:
                {
                    float Percentage = 1f - AttackTime / MaxAttackTime;
                    const float Full360 = 360 * Mathf.Deg2Rad;
                    float Sin = Mathf.Sin(Percentage * Full360), Cos = Mathf.Cos(Percentage * Full360);
                    Vector3 MovementDirection = GetDirectionVector() * 0.2f;
                    MovementDirection.x *= transform.localScale.x;
                    MovementDirection = MovementDirection + new Vector3(Cos * -MovementDirection.x, Sin * MovementDirection.y);
                    MovementDirection.y += (0.3f) * Mathf.Abs(Sin);
                    SpritesContainer.transform.localPosition = MovementDirection;
                    Vector3 NewScale = new Vector3(-0.3f, 0.4f) * Mathf.Abs(Sin);
                    SpritesContainer.localScale = Vector3.one + NewScale;
                }
                break;
        }
        AnimationTime += Time.deltaTime;
        if (AnimationTime >= AnimationMaxTime)
            AnimationTime -= AnimationMaxTime;
    }

    public void SyncAppearance()
    {
        if (IsServer)
            SyncAppearanceChangeClientRpc(BodyID, FaceID, Renderer.color);
        else
            SyncAppearanceChangeServerRpc(BodyID, FaceID, Renderer.color);
    }

    [ServerRpc]
    public void SyncAppearanceChangeServerRpc(byte Body, byte Face, Color BodyColor)
    {
        SyncAppearanceChangeClientRpc(Body, Face, BodyColor);
    }

    [ClientRpc]
    public void SyncAppearanceChangeClientRpc(byte Body, byte Face, Color BodyColor)
    {
        BodyID = Body;
        FaceID = Face;
        Renderer.sprite = Bodies[Body];
        FaceRenderer.sprite = Faces[Face];
        Renderer.color = BodyColor;
    }

    public enum AnimationType : byte
    {
        NotInitialized = 255,
        Stopped = 0,
        Moving = 1,
        Attacking = 2
    }
}
