using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterScript : NetworkBehaviour
{
    static readonly Vector3[] DirectionOrientations = new Vector3[]
    {
        new Vector3(0, 1),
        new Vector3(1, 1),
        new Vector3(1, 0),
        new Vector3(1, -1),
        new Vector3(0, -1),
        new Vector3(-1, -1),
        new Vector3(-1, 0),
        new Vector3(-1, 1)
    };

    [SerializeField] protected Vector2Int MapPosition = new Vector2Int(0, 0);
    [SerializeField] protected Vector2 MoveDirection = new Vector2(0, 0);
    [SerializeField] protected Transform SpritesContainer;
    [SerializeField] protected SpriteRenderer Renderer;
    [SerializeField] protected float Speed = 5;
    protected bool FacingLeft = false;
    protected float AttackTime = 0, MaxAttackTime = 1.5f;
    [SerializeField] public Directions Direction;
    [SerializeField] AudioClip AttackSound;
    [SerializeField] AudioClip HitSound;

    public Vector3 GetSpritePosition { get { return SpritesContainer.position; } }

    protected void Initialize()
    {
    }

    public void UpdateMovement()
    {
        bool Moved = MoveDirection.x != 0 || MoveDirection.y != 0;
        if (MoveDirection.x > 0)
        {
            MoveDirection.x = Mathf.Max(0, MoveDirection.x - Speed * Time.deltaTime);
        }
        else if (MoveDirection.x < 0)
        {
            MoveDirection.x = Mathf.Min(0, MoveDirection.x + Speed * Time.deltaTime);
        }
        if (MoveDirection.y > 0)
        {
            MoveDirection.y = Mathf.Max(0, MoveDirection.y - Speed * Time.deltaTime);
        }
        else if (MoveDirection.y < 0)
        {
            MoveDirection.y = Mathf.Min(0, MoveDirection.y + Speed * Time.deltaTime);
        }
        if (Moved) UpdatePosition();
    }

    public void UpdatePosition()
    {
        transform.position = new Vector3(MapPosition.x * 0.4f + 0.2f, MapPosition.y * 0.4f + 0.2f);
        SpritesContainer.localPosition = new Vector3(MoveDirection.x * transform.localScale.x, MoveDirection.y);
    }

    public Vector2 GetDirectionVector()
    {
        return DirectionOrientations[(int)Direction];
    }

    public bool MoveCharacter(int X, int Y)
    {
        if (MoveDirection.x != 0 || MoveDirection.y != 0 || AttackTime > 0)
            return false;
        if (Y > 0)
        {
            if (X < 0)
            {
                Direction = Directions.UpLeft;
            }
            else if (X > 0)
            {
                Direction = Directions.UpRight;
            }
            else
            {
                Direction = Directions.Up;
            }
        }
        else if (Y < 0)
        {
            if (X < 0)
            {
                Direction = Directions.DownLeft;
            }
            else if (X > 0)
            {
                Direction = Directions.DownRight;
            }
            else
            {
                Direction = Directions.Down;
            }
        }
        else
        {
            if (X < 0)
            {
                Direction = Directions.Left;
            }
            else if (X > 0)
            {
                Direction = Directions.Right;
            }
        }
        if (X < 0) FacingLeft = true;
        else if (X > 0) FacingLeft = false;
        if (IsOwner) SyncDirection();
        if (X != 0 && Y != 0)
        {
            bool HCollision = CheckForSolidTile(X, 0), VCollision = CheckForSolidTile(0, Y);
            if (HCollision && VCollision)
            {
                return false;
            }
            if (HCollision) X = 0;
            if (VCollision) Y = 0;
        }
        if (CheckForSolidTile(X, Y))
        {
            return false;
        }
        MapPosition.x += X;
        MoveDirection.x -= X * 0.4f;
        MapPosition.y += Y;
        MoveDirection.y -= Y * 0.4f;
        if (IsOwner) SyncPosition();
        return true;
    }

    private void UpdateDirection(int X, int Y)
    {
        if (Y > 0)
        {
            if (X < 0)
            {
                Direction = Directions.UpLeft;
            }
            else if (X > 0)
            {
                Direction = Directions.UpRight;
            }
            else
            {
                Direction = Directions.Up;
            }
        }
        else if (Y < 0)
        {
            if (X < 0)
            {
                Direction = Directions.DownLeft;
            }
            else if (X > 0)
            {
                Direction = Directions.DownRight;
            }
            else
            {
                Direction = Directions.Down;
            }
        }
        else
        {
            if (X < 0)
            {
                Direction = Directions.Left;
            }
            else if (X > 0)
            {
                Direction = Directions.Right;
            }
        }
    }

    public void Interact()
    {
        if (MoveDirection.x != 0 || MoveDirection.y != 0) return;
        Vector2Int Direction = Vector2Int.RoundToInt(GetDirectionVector());
        Collider2D c = GetColliderInDirection(MapPosition.x + Direction.x, MapPosition.y + Direction.y);
        if (c != null && c.GetComponent<CharacterScript>() == null)
        {
            c.SendMessage("Interact", this);
            return;
        }
        Attack();
    }

    private void Attack()
    {
        if (AttackTime > 0) return;
        AttackTime = MaxAttackTime;
        AudioSource.PlayClipAtPoint(AttackSound, transform.position, 0.8f);
        if (IsOwner) SyncAttack();
    }

    public void UpdateAttack()
    {
        float LastAttackTime = AttackTime;
        AttackTime -= Time.deltaTime;
        float HalfAttackTime = MaxAttackTime * 0.5f;
        if (AttackTime < HalfAttackTime && LastAttackTime >= HalfAttackTime)
        {

        }
    }

    private void SyncPosition()
    {
        if (IsServer)
            SyncNetworkPositionClientRpc(MapPosition, MoveDirection);
        else
            SyncNetworkPositionServerRpc(MapPosition, MoveDirection);
    }

    private void SyncDirection()
    {
        if(IsServer)
            SyncDirectionClientRpc(Direction, FacingLeft);
        else
            SyncDirectionServerRpc(Direction, FacingLeft);
    }

    private void SyncAttack()
    {
        if (IsServer)
            SyncAttackClientRpc();
        else
            SyncAttackServerRpc();
    }

    [ServerRpc]
    private void SyncNetworkPositionServerRpc(Vector2Int NewPosition, Vector2 NewMovement)
    {
        SyncNetworkPositionClientRpc(NewPosition, NewMovement);
    }

    [ClientRpc]
    private void SyncNetworkPositionClientRpc(Vector2Int NewPosition, Vector2 NewMovement)
    {
        MapPosition = NewPosition;
        MoveDirection = NewMovement;
        UpdateDirection((int)-MoveDirection.x, (int)-MoveDirection.y);
        UpdatePosition();
    }

    [ServerRpc]
    private void SyncDirectionServerRpc(Directions NewDirection, bool NewFacingLeft)
    {
        SyncDirectionClientRpc(NewDirection, NewFacingLeft);
    }

    [ClientRpc]
    private void SyncDirectionClientRpc(Directions NewDirection, bool NewFacingLeft)
    {
        Direction = NewDirection;
        FacingLeft = NewFacingLeft;
        transform.localScale = new Vector3(FacingLeft ? -1 : 1, 1, 1);
    }

    [ServerRpc]
    public void SyncAttackServerRpc()
    {
        SyncAttackClientRpc();
    }

    [ClientRpc]
    public void SyncAttackClientRpc()
    {
        Attack();
    }

    public bool CheckForSolidTile(int X, int Y)
    {
        return GetColliderInDirection(MapPosition.x + X, MapPosition.y + Y);
    }

    public Collider2D GetColliderInDirection(int X, int Y)
    {
        return Physics2D.OverlapBox(new Vector3(X * 0.4f + 0.2f, Y * 0.4f + 0.2f), Vector2.one * 0.1f, 0);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            PlayerScript.PlayerCharacter = (this as PlayerScript);
            CameraScript.ChangeCameraTarget(this);
            SyncPosition();
            SyncDirection();
            if (this is PlayerScript) (this as PlayerScript).SyncAppearance();
        }
    }

    public void ChangeColor(Color NewColor)
    {
        Renderer.color = NewColor;
    }

    public enum Directions : byte
    {
        Up = 0,
        UpRight = 1,
        Right = 2,
        DownRight = 3,
        Down = 4,
        DownLeft = 5,
        Left = 6,
        UpLeft = 7
    }
}
