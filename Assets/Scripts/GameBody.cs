using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBody : MonoBehaviour
{
    public GameBody Opponent;
    public List<GameBody> Attackers = new List<GameBody>();
    public CharacterData CharacterData;
    [SerializeField]
    protected Transform HealthBar;
    private float CurrentHealth;
    public float AttackTimer;
    public float FindNewOpponentTimer = 0.2f;
    protected float FindNewOpponentRate = 0.2f;
    public bool IsAttacking;
    public GroundNode MyGroundNode;
    public Player MyPlayer;
    protected System.Action<uint> ScoreUpdateAction;
    protected System.Action CharacterDestroyedAction;
    private bool IsInitialized;
    public List<MapGraph.Direction> directions = new List<MapGraph.Direction>();
    public bool IsMoving = false;
    protected float ReactionSpeed = 3;
    [SerializeField]
    public Queue<IAction> Actions = new Queue<IAction>();
    public Rigidbody2D MyRigidBody;

    private void Start()
    {
        MyRigidBody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(CharacterData characterData, Player myPlayer, System.Action<uint> scoreUpdate = null, System.Action onCharacterDestroyed = null)
    {
        Opponent = null;
        CharacterData = characterData;
        CurrentHealth = characterData.HitPoints;
        ScoreUpdateAction = scoreUpdate;
        CharacterDestroyedAction = onCharacterDestroyed;
        MyPlayer = myPlayer;

        Color color = characterData.Color;

        GetComponent<SpriteRenderer>().material.color = color;
        HealthBar.GetComponent<SpriteRenderer>().material.color = MyPlayer.GetMyTeam() == 1 ? Color.blue : Color.red;
        SetHealth(CurrentHealth);
       
        IsInitialized = true;
    }

    public void SetGroundNode(GroundNode groundNode)
    {
        if(MyGroundNode)
        {
            MyGroundNode.LeaveNode(this);
        }
        MyGroundNode = groundNode;
    }

    public GroundNode GetGroundNode()
    {
        return MyGroundNode;
    }

    public void SetHealth(float newHealth)
    {
        CurrentHealth = newHealth;
        Vector2 healthScale = HealthBar.localScale;
        healthScale.x = 0.05f * (CurrentHealth / CharacterData.HitPoints);
        HealthBar.localScale = healthScale;
        if (CurrentHealth <= 0)
        {
            if (Opponent)
            {
                Opponent.Attackers.Remove(this);
            }

            if (MyGroundNode)
            {
                MyGroundNode.LeaveNode(this);
            }

            gameObject.SetActive(false);
            enabled = false;
            if(ScoreUpdateAction != null)
            {
                ScoreUpdateAction(10);
            }

            if (CharacterDestroyedAction != null)
            {
                CharacterDestroyedAction();
            }
        }
    }

    public float GetHealth()
    {
        return CurrentHealth;
    }

    public Player GetMyPlayer()
    {
        return MyPlayer;
    }

    (GameBody body, uint radius) GetNearestBuilding(Player player)
    {
        if (player.ActiveBuildings.Count == 0) { return (null, 0); }
        uint myPositionIndex = MyGroundNode.GetNodeIndex();
        if(player.ActiveBuildings.Count == 1)
        {
            return (player.ActiveBuildings[0], (uint)System.Math.Ceiling(MyPlayer.GetMapGraph().GridDiagonalDistance(myPositionIndex, player.ActiveBuildings[0].MyGroundNode.GetNodeIndex())));
        }
        
        Dictionary<uint, double> distances = new Dictionary<uint, double>();
        double distanceToBuilding1;
        double distanceToBuilding2;
        for (int i = 0; i < player.ActiveBuildings.Count; i++)
        {
            for (int j = i + 1; j < player.ActiveBuildings.Count; j++)
            {
                uint ithBuildingIndex = player.ActiveBuildings[i].MyGroundNode.GetNodeIndex();
                uint jthBuildingIndex = player.ActiveBuildings[j].MyGroundNode.GetNodeIndex();

                if (distances.ContainsKey(ithBuildingIndex)) {
                    distanceToBuilding1 = distances[ithBuildingIndex];
                }
                else
                {
                    distanceToBuilding1 = MyPlayer.GetMapGraph().GridDiagonalDistance(myPositionIndex, ithBuildingIndex);
                    distances[ithBuildingIndex] = distanceToBuilding1;
                }

                if (distances.ContainsKey(jthBuildingIndex))
                {
                    distanceToBuilding2 = distances[jthBuildingIndex];
                }
                else
                {
                    distanceToBuilding2 = MyPlayer.GetMapGraph().GridDiagonalDistance(myPositionIndex, jthBuildingIndex);
                    distances[jthBuildingIndex] = distanceToBuilding2;
                }

                if(distanceToBuilding1 > distanceToBuilding2)
                {
                    GameBody temp = player.ActiveBuildings[i];
                    player.ActiveBuildings[i] = player.ActiveBuildings[j];
                    player.ActiveBuildings[j] = temp;
                }
            }
        }
        return (player.ActiveBuildings[0], (uint)System.Math.Ceiling(distances[player.ActiveBuildings[0].MyGroundNode.GetNodeIndex()]));
    }

    void ChargeAgainstEnemy()
    {
        Vector3 direction = Opponent.transform.position - transform.position;
        float Rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (CharacterData.CanRotate && MyRigidBody.rotation != Rotation)
        {
            IAction rotateAction = new ActionRotate(MyRigidBody, Rotation, CharacterData.Speed * 3);
            Actions.Enqueue(rotateAction);
        }

        float dist = Vector2.Distance(this.transform.position, Opponent.transform.position);

        if (CharacterData.CanMove)
        {
            IAction moveAction = new ActionMove(MyRigidBody, CharacterData.Range, CharacterData.Speed, Opponent.transform.position);
            Actions.Enqueue(moveAction);
        }

        IAction attackAction = new ActionAttack(Opponent,transform.position, CharacterData.Range, CharacterData.Damage, CharacterData.HitRate);
        Actions.Enqueue(attackAction);
    }

    void Update()
    {
        if (!IsInitialized) return;

        if (!Opponent && MyGroundNode)
        {
            IsMoving = false;
            (GameBody body, uint radius) nearestBuilding = GetNearestBuilding(MyPlayer.MyOpponentPlayer);
            if(nearestBuilding.body != null)
            {
                uint minSearchRadius = (uint)Mathf.Min(nearestBuilding.radius, CharacterData.Radius);
                GroundNode opponentGroundNode = MyPlayer.GetMapGraph().BreadthFirstSearch(MyGroundNode, null, MyPlayer.GetMyTeam(), minSearchRadius);
                if (opponentGroundNode)
                {
                    Opponent = opponentGroundNode.GetOpponent(MyPlayer.GetMyTeam());
                    //var path = MyPlayer.GetMapGraph().AstarPath(MyGroundNode, opponentGroundNode);
                    ChargeAgainstEnemy();
                }

                if (Opponent == null)
                {
                    Opponent = nearestBuilding.body;
                    ChargeAgainstEnemy();
                }
            } else
            {
                Debug.LogError("Nearest building cant be null");
            }
        }


        if (Opponent && Opponent.GetHealth() <= 0)
        {
            Opponent.Attackers.Remove(this);
            Opponent = null;
            IsAttacking = false;
            Actions.Clear();
        }

        if (Opponent && !IsAttacking)
        {
            FindNewOpponentTimer += Time.deltaTime;
            if (FindNewOpponentTimer >= FindNewOpponentRate)
            {
                Opponent.Attackers.Remove(this);
                Opponent = null;
                FindNewOpponentTimer = 0;
                Actions.Clear();
            }
        }

        if (!Opponent) return;

        if(!Opponent.Attackers.Contains(this))
        {
            Opponent.Attackers.Add(this);
        }
    }

    private void FixedUpdate()
    {
        if (!IsInitialized) return;

        if(Actions.Count != 0)
        {
            IAction action = Actions.Peek();
            if(action.IsActive())
            {
               action.Update();
            }
            else
            {
                Actions.Dequeue();
            }
        }
    }
}
