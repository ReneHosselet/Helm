using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth, dungeonLength;
    public int roomWidthMin, roomLengthMin;
    public int maxIterations;
    public int corridorWidth;
    public Material material;
    [Range(0.0f,0.3f)]
    public float roomBottomCornerModifier;
    [Range(0.7f,1.0f)]
    public float roomTopCornerModifier;
    [Range(0, 2)]
    public int roomOffset;
    public GameObject wallVertical, wallHorizontal;
    List<Vector3Int> possibleDoorVerticalPosition;
    List<Vector3Int> possibleDoorHorizontalPosition;
    List<Vector3Int> possibleWallVerticalPosition;
    List<Vector3Int> possibleWallHorizontalPosition;
    //start and end prefabs
    public GameObject startPoint, endPoint;
    //player and enemies
    public GameObject player,Enemy1;
    private GameObject spawnedPlayer;
    //cam maps
    public GameObject menuMapCamera;
    //levels and modifiers
    public int currentDungeonLevel = 0;
    private int maxEnemiesPerRoom = 3;
    //menu
    private bool isMenuOpen;
    private GameObject inGameMenu;
    private GameObject minimap;
    private GameObject canvasUI;
    //player ui
    //--health ui
    private GameObject canvas;
    private GameObject playerHealthBar;
    private Slider slider;
    private Text instructionText;
    //--currencyUI
    private Text currencyText;
    //counters
    private int enemyCounter = 0;
    //player currency and health
    private int currency = 0;
    public GameObject currencyModel;
    private float baseHealth = 50;
    public float health;
    //effects
    public GameObject BloodEffect;

    
    // Start is called before the first frame update
    void Start()
    {
        CreateDungeon();
        //get ui elements
        canvasUI = GameObject.FindGameObjectWithTag("MainUI");
        inGameMenu = canvasUI.transform.Find("InGameMenu").gameObject;
        minimap = canvasUI.transform.Find("Minimap").gameObject;
        //--player ui
        playerHealthBar = canvasUI.transform.Find("PlayerHealthBar").gameObject;
        slider = playerHealthBar.GetComponentInChildren<Slider>();
        instructionText = canvasUI.transform.Find("InstructionText").GetComponent<Text>();
        currencyText = canvasUI.transform.Find("CurrencyIcon/AmountText").GetComponent<Text>();
        //set ui, health player on start
        health = baseHealth;
        slider.value = CalculateHealth(health,baseHealth);
        ShowInstructionText(null);
        CalculateCurrency(currency);
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = CalculateHealth(health, baseHealth);
        //enlarge map + show stats
        if (Input.GetKeyDown(KeyCode.Tab) && !isMenuOpen)
        {
            isMenuOpen = true;
            StatsAndMap();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            isMenuOpen = false;
            StatsAndMap();
        }
        //debug test button
        if (Input.GetKeyDown(KeyCode.N))
        {
            CreateDungeon();
        }
        if (health < 0 )
        {
            OnDeath(spawnedPlayer);
        }
    }

    public void CreateDungeon()
    {
        currentDungeonLevel++;
        DestroyAllChildren();
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
        var listOfRooms = generator.CalculateDungeon(maxIterations,roomWidthMin,roomLengthMin,roomBottomCornerModifier,roomTopCornerModifier,roomOffset, corridorWidth);

        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;
        possibleDoorVerticalPosition = new List<Vector3Int>();
         possibleDoorHorizontalPosition = new List<Vector3Int>();
         possibleWallVerticalPosition = new List<Vector3Int>();
         possibleWallHorizontalPosition = new List<Vector3Int>();
        for (int i = 0; i < listOfRooms.Count; i++)
        {
            if (i == 0)
            {
                //startroom
                CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, "Start");
            }
            else if (i == generator.roomCount.Count-1)
            {
                //endroom
                CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, "End");
            }
            else if (i > 0 && i < generator.roomCount.Count - 1)
            {
                //all rooms between start and end
                CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, "Room");
            }
            else
            {
                CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, "Empty");
            }
                    
        }
        CreateWalls(wallParent);
        CreateHitCollForMouseRay();
        CreateMinimap();
        //increase dungeon size per level
        DungeonModifier();
    }
    private void DungeonModifier()
    {
        //increases dungeon size with set lenght and width
        dungeonWidth = dungeonWidth + roomWidthMin;
        dungeonLength = dungeonLength + roomLengthMin;
        Debug.Log(currentDungeonLevel);
    }
    private void CreateMinimap()
    {
        menuMapCamera.transform.position = new Vector3(dungeonWidth / 2, menuMapCamera.transform.position.y, dungeonLength / 2);
    }
    private void CreateHitCollForMouseRay()
    {
        //create plane for mouseraycasts
        GameObject hitColl = GameObject.CreatePrimitive(PrimitiveType.Plane);
        hitColl.transform.position = new Vector3(dungeonWidth,transform.position.y,dungeonLength);
        hitColl.transform.localScale = new Vector3(dungeonWidth/2, transform.localScale.y-0.5f,dungeonLength/2);
        hitColl.GetComponent<MeshRenderer>().enabled = false;
        hitColl.transform.parent = transform;
    }
    private void CreateWalls(GameObject wallParent)
    {
        foreach (var wallPosition in possibleWallHorizontalPosition)
        {
            CreateWall(wallParent,wallPosition,wallHorizontal);
        }
        foreach (var wallPosition in possibleWallVerticalPosition)
        {
            CreateWall(wallParent, wallPosition, wallVertical);
        }
    }
    
    private void CreateWall(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        Instantiate(wallPrefab,wallPosition,Quaternion.identity,wallParent.transform);
    }

    private void CreateMesh(Vector2 bottomLeftCorner,Vector2 topRightCorner,string point)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[]
        {
            topLeftV,
            topRightV,
            bottomLeftV,
            bottomRightV
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        int[] triangles = new int[]
        {
            0,
            1,
            2,
            2,
            1,
            3
        };
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject dungeonFloor = new GameObject("Mesh"+bottomLeftCorner,typeof(MeshFilter),typeof(MeshRenderer));

        //create start and endpoint
        setRoomPoints(bottomLeftCorner, topRightCorner, point, dungeonFloor);

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;
        dungeonFloor.gameObject.AddComponent<BoxCollider>();
        dungeonFloor.gameObject.layer = 8;
        dungeonFloor.transform.parent = transform;

        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }
        for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }
        for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
        for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
    }
    private void setRoomPoints(Vector2 bottomLeftCorner, Vector2 topRightCorner, string point, GameObject parent)
    {
        //0 = startroom
        //1 = endroom

        //mid point of room
        Vector3 middle = new Vector3((topRightCorner.x + bottomLeftCorner.x)/2, 0, (topRightCorner.y + bottomLeftCorner.y)/2);
        switch (point)
        {
            case "Start":
                //player and start obj
                Instantiate(startPoint, middle, Quaternion.identity, parent.transform);
                spawnedPlayer = Instantiate(player, middle+new Vector3(0,player.gameObject.transform.localScale.y/2,0), Quaternion.identity, parent.transform);
                break;
            case "End":
                //to the next level obj
                Instantiate(endPoint, middle, Quaternion.identity, parent.transform);
                break;
            case "Room":
                //set points in rooms for spawns and extra
                //spawns random enemies
                RandomSpawner(bottomLeftCorner, topRightCorner, "enemies",parent);
                break;
            default:
                break;
        }
    }

    private void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }
    private void DestroyAllChildren()
    {
        while (transform.childCount != 0)
        {
            foreach (Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }
    private void StatsAndMap()
    {
        if (isMenuOpen)
        {
            minimap.SetActive(false);
            menuMapCamera.gameObject.SetActive(true);
            inGameMenu.SetActive(true);
        }
        else if (!isMenuOpen)
        {
            minimap.SetActive(true);
            menuMapCamera.gameObject.SetActive(false);
            inGameMenu.SetActive(false);
        }
    }
    private void RandomSpawner(Vector2 bottomLeft,Vector2 topRight,string objToSpawn, GameObject par)
    {
        //offset to make spawns more to middle of room
        float offset = 1;
        switch (objToSpawn)
        {
            case "enemies":
                //randomize spawn for enemies in room
                Instantiate(Enemy1, new Vector3(Random.Range(bottomLeft.x + offset, topRight.x - offset), Enemy1.gameObject.transform.localScale.y / 2, Random.Range(bottomLeft.y + offset, topRight.y - offset)), Quaternion.identity, par.transform);
                for (int i = 0; i < maxEnemiesPerRoom; i++)
                {
                    switch (Random.Range(0,5))//20% chance for extra enemy /room
                    {
                        case 0:
                            Instantiate(Enemy1, new Vector3(Random.Range(bottomLeft.x + offset, topRight.x - offset), Enemy1.gameObject.transform.localScale.y / 2, Random.Range(bottomLeft.y + offset, topRight.y - offset)), Quaternion.identity,par.transform);
                            enemyCounter++;
                            break;
                        default:
                            break;
                    }
                    
                }
                break;
            default:
                break;
        }
    }
    public float CalculateHealth(float h,float mH)
    {
        return h / mH;
    }
    public void OnDeath(GameObject obj)
    {
        Destroy(obj.gameObject);
        Instantiate(BloodEffect,obj.transform.position,Quaternion.identity);
        switch (obj.tag)
        {
            case "Enemy":
                //spawns coin
                Instantiate(currencyModel, obj.transform.position, Quaternion.Euler(90,0,0),gameObject.transform);
                break;
            case "Player":
                break;
            default:
                break;
        }
    }
    public void CalculateDamage(GameObject obj,WeaponScript weapon, float damageModifiers)
    {
        //base damage + other modifiers
        switch (obj.tag)
        {
            case "Enemy":
                obj.GetComponent<Enemy>().health -= (weapon.baseDamage + damageModifiers);
                break;
            case "Player":
                this.health -= (weapon.baseDamage + damageModifiers);
                break;
            default:
                break;
        }
    }
    public void ShowInstructionText(GameObject obj)
    {
        if (obj == null)
        {
            instructionText.text = "";
        }
        else
        {
            switch (obj.tag)
            {
                case "EndPoint":
                    instructionText.text = "Press 'SPACE' to continue";
                    break;
                default:
                    break;
            }
        }        
    }
    public void CalculateCurrency(int amount)
    {
        currency += amount;
        currencyText.text = currency.ToString();
    }
}
