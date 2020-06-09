using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using TMPro;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth, dungeonLength;
    public int roomWidthMin, roomLengthMin;
    public int maxIterations;
    public int corridorWidth;
    public List<Material> material;
    public List<Material> fogMaterial;
    [Range(0.0f,0.3f)]
    public float roomBottomCornerModifier;
    [Range(0.7f,1.0f)]
    public float roomTopCornerModifier;
    [Range(0, 3)]
    public int roomOffset,positionOffsetFromMiddle;
    public List<GameObject>  wallVertical, wallHorizontal;
    public List<GameObject> ceilingLight;
    public int lightWallIntermission;
    List<Vector3Int> possibleDoorVerticalPosition;
    List<Vector3Int> possibleDoorHorizontalPosition;
    List<Vector3Int> possibleWallVerticalPosition;
    List<Vector3Int> possibleWallHorizontalPosition;
    //start and end prefabs
    public GameObject startPoint;
    public List<GameObject> teleportZones;
    //player and enemies
    public List<GameObject> enemyList;
    public GameObject player;
    private GameObject spawnedPlayer;
    //items and pickups
    public List<GameObject> pickUpList;
    public List<GameObject> bossPickup;
    //rooms
    private int maxPickUpRooms;
    //cam maps
    public GameObject menuMapCamera;
    //levels and levelmodifiers
    private int tempDungeonWidth, tempDungeonLength, tempRoomWidthMin, tempRoomLengthMin,tempMaxIteration;
    private bool isShop, isBoss = false;
    public int currentDungeonLevel = 0;
    private int maxEnemiesPerRoom = 3;
    public List<GameObject> corridorObjectList;
    //menu
    private bool isMenuOpen;
    private GameObject inGameMenu;
    private GameObject minimap;
    private GameObject canvasUI;
    //player ui
    //fade screen ui
    public GameObject crossFade;
    public float transitionTime;
    //--health ui
    private GameObject canvas;
    private GameObject playerHealthBar;
    private Slider slider;
    private TextMeshProUGUI instructionText;
    private Text APUpText,HPUpText;
    //--currencyUI
    private TextMeshProUGUI currencyText;
    //counters
    private int enemyCounter = 0;
    //player stats
    //--health and currency
    private int currency = 0;
    public GameObject currencyModel;
    private float baseHealth = 50;
    public float health;
    public Image healthavatar;
    public List<Sprite> playerAvatars;
    //--damagemodifier
    public float playerAttackUp = 0;
    public bool hasDash = false;
    public float dashDistance = 35f;
    //--GameoverUI
    public GameObject gameOverCanvas;
    public GameObject displayLevelReached;
    //effects
    public List<GameObject> effects;
    //collection lists
    public List<GameObject> weaponList;
    public List<ParticleSystem> emitterList;
    public List<GameObject> bossList;
    //temp vars
    private GameObject bossTeleport;
    public GameObject parentFogObject;
    private bool isBossRoom =false;
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(Transition(0));
        //get ui elements
        canvasUI = GameObject.FindGameObjectWithTag("MainUI");
        inGameMenu = canvasUI.transform.Find("InGameMenu").gameObject;
        minimap = canvasUI.transform.Find("borderminimap").GetChild(0).GetChild(0).gameObject;
        //--player ui
        playerHealthBar = canvasUI.transform.Find("PlayerHealthBar").gameObject;
        slider = playerHealthBar.GetComponentInChildren<Slider>();
        instructionText = canvasUI.transform.Find("InstructionText").GetComponent<TextMeshProUGUI>();
        currencyText = canvasUI.transform.Find("CurrencyIcon/AmountText").GetComponent<TextMeshProUGUI>();
        APUpText = canvasUI.transform.Find("InGameMenu/APUpText").GetComponent<Text>();
        HPUpText = canvasUI.transform.Find("InGameMenu/HPUpText").GetComponent<Text>();
        //set ui, health player on start
        health = baseHealth;
        slider.value = CalculateHealth(health,baseHealth);
        ShowInstructionText(null);
        CalculateCurrency(currency);
        //parent fog 
        parentFogObject = GameObject.FindGameObjectWithTag("ParentFog");
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = CalculateHealth(health, baseHealth);
        if (health <= 0 )
        {
            PlayerDeath();
        }
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
            StartCoroutine(Transition(0));
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(Transition(1));
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            CalculateCurrency(1);
        }
        if (health < 0 )
        {
            OnDeath(spawnedPlayer);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            instructionText.text = "";
        }
        changeAvatar();
    }

    public void CreateDungeon(int typeLevel)
    {
        Debug.Log(currentDungeonLevel);
        //checks if every 5th lvl is boss level
        if (Convert.ToDouble((currentDungeonLevel) % 5) == 0 && typeLevel != 1)
        {
            if (!isBoss)
            {
                typeLevel = 2;
            }
            else if (isBoss)
            {
                isBoss = false;
            }
        }
        //check if dungeonlevel,shop or boss -- 0 = normal / 1 = shop / 2 = boss
        if (typeLevel == 0)
        {
            //increment dungeonlevel
            currentDungeonLevel++;
            
            //set amount of pickuprooms at start
            maxPickUpRooms = 1 + (currentDungeonLevel / 5);
        }
        else if (typeLevel == 1)
        {
            //save last level parameters
            SaveOrLoadLastLevelParameters(0);
            //sets up shop level
            isShop = true;
            dungeonWidth = 15;
            dungeonLength = 15;
            roomLengthMin = 15;
            roomWidthMin = 15;
        }
        else if (typeLevel == 2)
        {
            //save last level parameters
            SaveOrLoadLastLevelParameters(0);
            //set up boss room
            isBoss = true;
            dungeonWidth = 40;
            dungeonLength = 40;
            roomLengthMin =40;
            roomWidthMin = 40;
            maxIterations = 0;
        }
        DestroyAllChildren();
        if (dungeonWidth > 100 && dungeonLength > 100)
        {
            dungeonWidth = 85;
            dungeonLength = 85;
        }
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
            //check if level is not a shop level
            if (typeLevel != 1)
            {
                if (i == 0)
                {
                    if (typeLevel != 2)
                    {
                        //startroom
                        CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, "Start");
                    }
                    else
                    {
                        //bossroom
                        CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, "Boss");
                    }
                }
                else if (i == generator.roomCount.Count - 1)
                {
                    if (typeLevel != 2)
                    {
                        //endroom
                        CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, "End");
                    }
                    else if (typeLevel == 2)
                    {
                        //bossroom
                        CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, "Boss");
                    }
                }
                else if (i > 0 && i < generator.roomCount.Count - 1)
                {
                    //all rooms between start and end
                    //checks if pickup room has to be created
                    if (maxPickUpRooms > 0)
                    {
                        CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, "PickUp");
                        maxPickUpRooms--;
                    }
                    else
                    {
                        CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, "Room");
                    }
                }
                else
                {
                    CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, "Empty");
                }
            }
            //create shop level
            else if (typeLevel == 1)
            {                
                CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, "Shop");
            }
        }
        CreateWalls(wallParent);
        CreateHitCollForMouseRay();
        CreateMinimap();
        
        //increase dungeon size per level
        DungeonModifier();
        //reload last normal level parameters
        if (typeLevel != 0)
        {
            SaveOrLoadLastLevelParameters(1);
            isShop = false;
        }        
    }
    private void SaveOrLoadLastLevelParameters(int type)
    {
        //0 = save , 1 == load
        if (type == 0)
        {
            tempDungeonLength = dungeonLength;
            tempDungeonWidth = dungeonWidth;
            tempRoomLengthMin = roomLengthMin;
            tempRoomWidthMin = roomWidthMin;
            tempMaxIteration = maxIterations;
        }
        else if (type == 1)
        {
            dungeonWidth = tempDungeonWidth;
            dungeonLength = tempDungeonLength;
            roomLengthMin = tempRoomLengthMin;
            roomWidthMin = tempRoomWidthMin;
            maxIterations = tempMaxIteration;
        }
    }
    private void DungeonModifier()
    {
        //increases dungeon size with set length and width
        dungeonWidth += roomWidthMin;
        dungeonLength += roomLengthMin;
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
    private void CreateFogArea(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        //GameObject fog = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //fog.transform.position = new Vector3(dungeonWidth, transform.position.y + 4f, dungeonLength);
        //fog.transform.localScale = new Vector3(dungeonWidth / 2, transform.localScale.y - 0.5f, dungeonLength / 2);
        //fog.GetComponent<Renderer>().material = fogMaterial[0];
        //fog.tag = "Fog";
        //fog.layer = 10;

        //Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 3f, bottomLeftCorner.y);
        //Vector3 bottomRightV = new Vector3(topRightCorner.x, 3f, bottomLeftCorner.y);
        //Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 3f, topRightCorner.y);
        //Vector3 topRightV = new Vector3(topRightCorner.x, 3f, topRightCorner.y);

        //Vector3[] vertices = new Vector3[]
        //{
        //    topLeftV,
        //    topRightV,
        //    bottomLeftV,
        //    bottomRightV
        //};

        //Vector2[] uvs = new Vector2[vertices.Length];
        //for (int i = 0; i < uvs.Length; i++)
        //{
        //    uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        //}
        //int[] triangles = new int[]
        //{
        //    0,
        //    1,
        //    2,
        //    2,
        //    1,
        //    3
        //};
        //Mesh mesh = new Mesh();
        //mesh.vertices = vertices;
        //mesh.uv = uvs;
        //mesh.triangles = triangles;

        //GameObject fog = new GameObject("Fog", typeof(MeshFilter), typeof(MeshRenderer));

        //fog.transform.position = Vector3.zero;
        //fog.transform.localScale = Vector3.one;

        //fog.layer = 10;
        //fog.tag = "Fog";

        //fog.GetComponent<MeshFilter>().mesh = mesh;
        //fog.GetComponent<Renderer>().material = fogMaterial[0];

        //fog.gameObject.AddComponent<BoxCollider>();
        //fog.transform.parent = parentFogObject.transform;
    }
    private void CreateWalls(GameObject wallParent)
    {
        foreach (var wallPosition in possibleWallHorizontalPosition)
        {
                CreateWall(wallParent, wallPosition, wallHorizontal[0]);
        }
        foreach (var wallPosition in possibleWallVerticalPosition)
        {
                CreateWall(wallParent, wallPosition, wallVertical[0]);
        }
    }
    
    private void CreateWall(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);    
    }
    private void CreateMesh(Vector2 bottomLeftCorner,Vector2 topRightCorner,string point)
    {
        //creates fog area above ground mesh
        //CreateFogArea(bottomLeftCorner,topRightCorner);

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
        dungeonFloor.GetComponent<Renderer>().material = material[0];
        //recalc normals ---> lighting probs fixed
        Mesh msh = dungeonFloor.GetComponent<MeshFilter>().mesh;
        msh.RecalculateNormals();
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
        float x = topRightCorner.x - bottomLeftCorner.x;
        float z = topRightCorner.y - bottomLeftCorner.y;
        Vector3 middle = new Vector3((topRightCorner.x + bottomLeftCorner.x)/2, 0, (topRightCorner.y + bottomLeftCorner.y)/2);
        switch (point)
        {
            case "Start":
                //player and start obj
                Instantiate(startPoint, middle, Quaternion.identity, parent.transform);
                spawnedPlayer = Instantiate(player, middle + new Vector3(0, player.gameObject.transform.localScale.y / 2, 0), Quaternion.identity, parent.transform);
                break;
            case "End":
                //to the next level obj
                Instantiate(teleportZones[0], middle + new Vector3(0, 0, 0), Quaternion.identity, parent.transform);
                break;
            case "Room":
                //set points in rooms for spawns and extra
                //spawns random enemies
                RandomSpawner(bottomLeftCorner, topRightCorner, "enemies", parent);
                break;
            case "PickUp":
                if (Random.Range(0,20) == 0)
                {
                    //set pickup on room
                    var inst = Instantiate(pickUpList[Random.Range(0, pickUpList.Count)], middle, Quaternion.identity, parent.transform);
                    inst.GetComponent<PickUp>().currencyValue = 0;
                }
                else
                {
                    //spawns random enemies
                    RandomSpawner(bottomLeftCorner, topRightCorner, "enemies", parent);
                }
                break;
            case "Shop":
                //3 pickups per shop level + player and teleport to next level zone
                Instantiate(teleportZones[0], middle + new Vector3(0, 0, -positionOffsetFromMiddle), Quaternion.identity, parent.transform);
                middle += new Vector3(-positionOffsetFromMiddle, 0, positionOffsetFromMiddle);
                Instantiate(player, middle + new Vector3(-positionOffsetFromMiddle, player.gameObject.transform.localScale.y / 2, -positionOffsetFromMiddle), Quaternion.identity, parent.transform);
                for (int i = 0; i < 3; i++)
                {
                    Instantiate(pickUpList[Random.Range(0, pickUpList.Count)], middle, Quaternion.identity, parent.transform);
                    middle += new Vector3(positionOffsetFromMiddle, 0, 0);
                }
                break;
            case "Boss":
                //boss spawn
                Instantiate(bossList[0], new Vector3(middle.x, enemyList[0].gameObject.transform.localScale.y /2, middle.z + positionOffsetFromMiddle * 3), Quaternion.identity, parent.transform);
                //player spawn
                spawnedPlayer = Instantiate(player, new Vector3(middle.x, player.gameObject.transform.localScale.y / 2, middle.z - positionOffsetFromMiddle * 3), Quaternion.identity, parent.transform);
                //instantiate teleporter and turns this inactive
                bossTeleport =  Instantiate(teleportZones[1], new Vector3(middle.x, 0, middle.z), Quaternion.identity, gameObject.transform);
                bossTeleport.SetActive(false);
                break;
            case "Empty":
                if (Random.Range(0, 3) == 0)
                {
                    //when random is 0 spawn door in corridor  
                    //0 = wooden door
                    int corrObject = Random.Range(0, corridorObjectList.Count);
                    if (z > x)
                    {
                        //vertical
                        Instantiate(corridorObjectList[corrObject], middle + new Vector3(0, 0, -1.5f), Quaternion.Euler(0, -90, 0), parent.transform);
                        Instantiate(corridorObjectList[corrObject], middle + new Vector3(0, 0, 1.5f), Quaternion.Euler(0, 90, 0), parent.transform);
                    }
                    else if (x > z)
                    {
                        //horizontal
                        Instantiate(corridorObjectList[corrObject], middle + new Vector3((-1.5f), 0, 0), Quaternion.identity, parent.transform);
                        Instantiate(corridorObjectList[corrObject], middle + new Vector3((1.5f), 0, 0), Quaternion.Euler(0, 180, 0), parent.transform);
                    }
                }
                break;                
            default:
                break;
        }
        if (point != "Empty")
        {
            //instantiates light in every room not corridors
            Instantiate(ceilingLight[0], middle + new Vector3(0, 2.62f, 0), Quaternion.identity, parent.transform);
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
        HPUpText.text = health.ToString() + " / " + baseHealth.ToString();
        APUpText.text = playerAttackUp.ToString();
        if (isMenuOpen)
        {
            menuMapCamera.gameObject.SetActive(true);
            minimap.SetActive(false);
            inGameMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else if (!isMenuOpen)
        {
            menuMapCamera.gameObject.SetActive(false);
            minimap.SetActive(true);
            inGameMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }
    private void RandomSpawner(Vector2 bottomLeft,Vector2 topRight,string objToSpawn, GameObject par)
    {
        //offset to make spawns more to middle of room
        float offset = 1;
        int rand = Random.Range(0, enemyList.Count);
        switch (objToSpawn)
        {
            case "enemies":
                //randomize spawn for enemies in room
                Instantiate(enemyList[rand], new Vector3(Random.Range(bottomLeft.x + offset, topRight.x - offset), enemyList[0].gameObject.transform.localScale.y / 2, Random.Range(bottomLeft.y + offset, topRight.y - offset)), Quaternion.identity, par.transform);
                for (int i = 0; i < maxEnemiesPerRoom; i++)
                {
                    switch (Random.Range(0,5))//20% chance for extra enemy /room
                    {
                        case 0:
                            Instantiate(enemyList[0], new Vector3(Random.Range(bottomLeft.x + offset, topRight.x - offset), enemyList[0].gameObject.transform.localScale.y / 2, Random.Range(bottomLeft.y + offset, topRight.y - offset)), Quaternion.identity,par.transform);
                            enemyCounter++;
                            break;
                        default:
                            break;
                    }                    
                }
                break;
            case "breakables":
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
        switch (obj.tag)
        {
            case "Enemy":
                if (obj.name.Substring(0,4) == "Boss")
                {
                    //teleporter to next room active
                    bossTeleport.SetActive(true);
                    Instantiate(effects[0], obj.transform.position, Quaternion.identity);
                    //0 is boss dash pickup
                    Instantiate(bossPickup[0], obj.transform.position, Quaternion.identity, gameObject.transform);
                }
                else
                {
                    //spawns coin
                    Instantiate(currencyModel, obj.transform.position, Quaternion.Euler(90, 0, 0), gameObject.transform);
                    Instantiate(effects[0], obj.transform.position, Quaternion.identity);
                }
                break;
            case "Breakable":
                Instantiate(effects[1], obj.transform.position, Quaternion.identity,gameObject.transform);
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
            case "Breakable": obj.GetComponent<Breakable>().health -= (weapon.baseDamage + damageModifiers);
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
                case "PickUp":
                    if (obj.GetComponent<PickUp>().currencyValue == 0)
                    {
                        instructionText.text = "Press 'SPACE' to take upgrade";
                    }
                    else
                    {
                        instructionText.text = "Press 'SPACE' to Buy";
                    }                    
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
    public void ApplyPickUp(GameObject pickUp,int currencyV, int pickUpV)
    {
        if (currency - currencyV >=0)
        {
            switch (pickUp.name.Substring(0, 4))
            {
                case "APUp":
                    playerAttackUp += pickUpV;
                    break;
                case "HPUp":
                    baseHealth += pickUpV;
                    break;
                case "Heal":
                    if (health + pickUpV > baseHealth)
                    {
                        health = baseHealth;
                    }
                    else
                    {
                        health += pickUpV;
                    }
                    break;
                case "Dash":
                   hasDash = true;
                    break;
                default:
                    break;
            }
            CalculateCurrency(-currencyV);
            Destroy(pickUp);
        }
        else
        {
            instructionText.text = "You need more currency to buy this!";
        }
    }
    public static GameObject FindParentWithTag(GameObject childObject, string tag)
    {
        Transform t = childObject.transform;
        while (t.parent != null)
        {
            if (t.parent.tag == tag)
            {
                return t.parent.gameObject;
            }
            t = t.parent;
        }
        return null; // Could not find a parent with given tag.
    }
    public IEnumerator Transition(int type)
    {
        if (currentDungeonLevel > 1)
        {
            if ((currentDungeonLevel)  % 5 == 0 && !isBossRoom)
            {
                crossFade.GetComponentInChildren<TextMeshProUGUI>().text = "BOSS";
                isBossRoom = true;
            }
            else if (type == 1)
            {
                crossFade.GetComponentInChildren<TextMeshProUGUI>().text = "SHOP";
            }
            else
            {
                crossFade.GetComponentInChildren<TextMeshProUGUI>().text = "LEVEL " + currentDungeonLevel.ToString();
                isBossRoom = false;
            }
            crossFade.GetComponent<Animator>().SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            CreateDungeon(type);
            crossFade.GetComponent<Animator>().SetTrigger("End");
        }
        else if( currentDungeonLevel <= 1)
        {
            crossFade.GetComponentInChildren<TextMeshProUGUI>().text = "LEVEL " + currentDungeonLevel.ToString();
            CreateDungeon(type);
        }
    }
    private void changeAvatar()
    {
        float currhealth = health / baseHealth;

        if (currhealth >= 0.75)
        {
            healthavatar.sprite = playerAvatars[0];
        }
        else if (currhealth < 0.75 && currhealth > 0.25)
        {
            healthavatar.sprite = playerAvatars[1];
        }
        else if (currhealth <= 0.25)
        {
            healthavatar.sprite = playerAvatars[2];
        }
    }
    private void PlayerDeath()
    {
        canvasUI.SetActive(false);
        gameOverCanvas.SetActive(true);
        if (isBossRoom)
        {
            displayLevelReached.GetComponent<TextMeshProUGUI>().text = "You died on Boss level";
        }
        else
        {
            displayLevelReached.GetComponent<TextMeshProUGUI>().text = "You died on level " + (currentDungeonLevel-1).ToString();
        }
       
    }
}
