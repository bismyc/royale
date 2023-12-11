using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject CharactersParent;
    [SerializeField]
    private CharacterData[] TotalCharactersInDeck;
    private List<CharacterData> ActiveCharactersInDeck = new List<CharacterData>();
    public List<GameBody> ActiveCharactersInBattle = new List<GameBody>();
    public List<GameBody> CharacterPool = new List<GameBody>();

    [SerializeField]
    private CharacterData QueenTowerData;
    [SerializeField]
    private CharacterData PrinceTowerData;

    [SerializeField]
    private uint CurrentElixer;
    private float ElixerTimer;

    private CharacterData nextCharacterToSpawn;
    public uint Score;
    [SerializeField]
    private uint MyTeam;

    public Player MyOpponentPlayer;

    [SerializeField]
    private GameData GameData;

    public List<uint> SpawnableNodeIndices;

    private bool Initialized;
    private IMapGraph MapGraph;
    private (GroundNode start, GroundNode end) OwnTerritory;
    private uint TotalNodes;
    public List<GameBody> ActiveBuildings;


    public void Initialize(uint team, List<uint> spawnableNodeIndices, IMapGraph mapGraph, (GroundNode start, GroundNode end) ownTerritory, uint totalNodes)
    {
        CurrentElixer = GameData.ElixerLimit - 3;
        ElixerTimer = GameData.ElixerRate;
        MyTeam = team;
        MapGraph = mapGraph;
        SpawnableNodeIndices = spawnableNodeIndices;
        OwnTerritory = ownTerritory;
        TotalNodes = totalNodes;

        CharacterData towerData;

        string playerObjectName = MyTeam + "_" + "Player";
        GameBody[] towers = GameObject.Find(playerObjectName).GetComponentsInChildren<GameBody>();
        for (int i = 0; i < towers.Length; i++)
        {
            towerData = towers[i].CharacterData;
            InitializeTower(towerData, towers[i]);
            ActiveBuildings.Add(towers[i]);
        }

        Dictionary<int, uint> activeCharactersInDeck = new Dictionary <int, uint>();

        for (int i = 0; i < TotalCharactersInDeck.Length; i++)
        {
            int randomIndex = Random.Range(0, TotalCharactersInDeck.Length - 1);
            while (activeCharactersInDeck.ContainsKey(randomIndex))
            {
                randomIndex = Random.Range(0, TotalCharactersInDeck.Length - 1);
            }
            ActiveCharactersInDeck.Add(TotalCharactersInDeck[randomIndex]);
        }

        Initialized = true;

    }

    public void SetOpponentPlayer(Player opponentPlayer)
    {
        MyOpponentPlayer = opponentPlayer;
    }

    public uint GetMyTeam()
    {
        return MyTeam;
    }

    public IMapGraph GetMapGraph()
    {
        return MapGraph;
    }

    void Update()
    {
        if (!Initialized) return;

        ElixerTimer += Time.deltaTime;
        if (CurrentElixer < GameData.ElixerLimit && ElixerTimer >= GameData.ElixerRate)
        {
            ElixerTimer = 0;
            CurrentElixer += 1;
        }

        if(CurrentElixer > 0 && !nextCharacterToSpawn)
        {
            nextCharacterToSpawn = GetNextCharacterData();
        }

        if(nextCharacterToSpawn && nextCharacterToSpawn.Cost <= CurrentElixer)
        {
            SpawnCharacter(nextCharacterToSpawn);
            nextCharacterToSpawn = null;
        }

    }

    //public CharacterData[] GetActiveCharactersInDeck()
    //{
    //    return ActiveCharactersInDeck;
    //}

    CharacterData GetNextCharacterData()
    {
        int randomActiveCharacterIndex = UnityEngine.Random.Range(0, (int)GameData.MaximumCharactersInDeck - 1);
        CharacterData nextCharacterData = ActiveCharactersInDeck[randomActiveCharacterIndex];
        ActiveCharactersInDeck.Remove(nextCharacterData);
        return nextCharacterData;
    }

    void SpawnCharacter(CharacterData characterData)
    {
        float yOffset = MyTeam == 1 ? -5 : 5;
        Quaternion rotation = MyTeam == 1 ? Quaternion.Euler(0, 0, 90) : Quaternion.Euler(0, 0, 270);
        int randomSpawnableIndex = (int)Random.Range(0, SpawnableNodeIndices.Count - 1);
        bool[] visited = new bool[TotalNodes];
        GroundNode chosenGroundNode = MapGraph.DepthFirstSearchPartGraph(OwnTerritory.start, OwnTerritory.end, SpawnableNodeIndices[randomSpawnableIndex], visited);

        GameBody character = null;
        if(CharacterPool.Count > 0)
        {
            character = CharacterPool[CharacterPool.Count - 1];
            CharacterPool.RemoveAt(CharacterPool.Count - 1);
            character.CharacterData = characterData;
            character.gameObject.SetActive(true);
            character.enabled = true;
        } else
        {
            character = GameObject.Instantiate(characterData.Prefab).GetComponent<GameBody>();
        }

        character.transform.SetParent(CharactersParent.transform);
        character.transform.position = chosenGroundNode.transform.position;
        character.transform.rotation = rotation;
        character.name = MyTeam + "_" + characterData.name;

        void onCharacterDestroyed()
        {
            CharacterPool.Add(character);
        }

        character.Initialize(characterData, this, null, onCharacterDestroyed);

        ActiveCharactersInDeck.Add(characterData);
        if(characterData.CanMove)
        {
            ActiveCharactersInBattle.Add(character);
        }
        else
        {
            ActiveBuildings.Add(character);
        }
        CurrentElixer -= characterData.Cost;
    }

    void InitializeTower(CharacterData characterData, GameBody tower)
    {
        void scoreUpdate(uint extraScore)
        {
            Score += extraScore;
        }
        tower.Initialize(characterData, this, scoreUpdate, null);
    }
}