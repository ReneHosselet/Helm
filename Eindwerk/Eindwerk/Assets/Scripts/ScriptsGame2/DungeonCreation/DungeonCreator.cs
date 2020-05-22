using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    //levels and modifiers
    private int currentDungeonLevel = 0;

    // Start is called before the first frame update
    void Start()
    {
        CreateDungeon();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            else
            {
                CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, "Empty");
            }
                    
        }
        CreateWalls(wallParent);
        CreateHitCollForMouseRay();

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
        createStartAndEndPoint(bottomLeftCorner, topRightCorner, point, dungeonFloor);

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;
        dungeonFloor.gameObject.AddComponent<BoxCollider>();
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
    private void createStartAndEndPoint(Vector2 bottomLeftCorner, Vector2 topRightCorner, string point, GameObject parent)
    {
        //0 = startroom
        //1 = endroom

        //mid point of room
        Vector3 middle = new Vector3((topRightCorner.x + bottomLeftCorner.x)/2, 0, (topRightCorner.y + bottomLeftCorner.y)/2);
        switch (point)
        {
            case "Start":
                Instantiate(startPoint, middle, Quaternion.identity, parent.transform);
                Instantiate(player, middle+new Vector3(0,player.gameObject.transform.localScale.y/2,0), Quaternion.identity, GameObject.Find("DungeonCreator").transform);
                //enemy instantiate for test purpose
                Instantiate(Enemy1, middle + new Vector3(1, Enemy1.gameObject.transform.localScale.y / 2, 1), Quaternion.identity, GameObject.Find("DungeonCreator").transform);
                break;
            case "End":
                Instantiate(endPoint, middle, Quaternion.identity, parent.transform);
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
}
