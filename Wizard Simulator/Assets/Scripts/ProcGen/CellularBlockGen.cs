using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularBlockGen : MonoBehaviour {

    // MAP PREVIEW
    // Choose how the map is displayed
    public enum DrawMode { NoiseMap, Terrain };
    public DrawMode drawMode;
    // The place where we render our NoiseMap
    public Renderer textureRender;

    // TERRAIN DATA
    // The floors prefab
    public GameObject groundPrefab;
    // The space bettween the ground prefabs
    public float groundSpacer = 8.0f;
    // The smallest you empty spaces can be
    public int emptyThresholdSize = 50;
    // The smallest terrain region therre can be
    public int terrainThresholdSize = 50;
    // The size of the connecting corridors
    public int pathSize = 1;
    // How filled the map will be with walls/empty zones
    [Range(0, 100)]
    public int randomFillPercent;
    // How many times the map will be run through a smoothing algorithm
    public int numOfSmoothingPasses = 5;

    // LEVEL DATA
    // The maps width
    public int width;
    // The maps height
    public int height;
    // The size of the border around the map
    public int borderSize = 10;

    // NOISE DATA
    // The seed choosen by the user
    public string seed;
    // Whether or not to use a random seed each time instead of the seed.
    public bool useRandomSeed;

    int[,] map;

	// Use this for initialization
	void Start () {
        GenerateMap();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
            GenerateMap();
        }
	}

    void GenerateMap() {
        // Create our new noise map with desired height and width
        map = new int[width, height];
        // Randomly fills our map with integers based on my logic gates
        RandomFillMap();

        // Will smooth the map creating a cellular effect by pulling walls together and pulling empty spaces together
        for (int i = 0; i < numOfSmoothingPasses; i++) {
            SmoothMap();
        }

        ProcessMap();

        // Making a map that is exteded with a border of walls/emptiness
        int[,] extendedMap = new int[width + borderSize * 2, height + borderSize * 2];
        for (int x = 0; x < extendedMap.GetLength(0); x++) {
            for (int y = 0; y < extendedMap.GetLength(1); y++) {
                // If the x and y are withing the map we set its values to the map. Otherwise we set them to 1
                if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
                    extendedMap[x, y] = map[x - borderSize, y - borderSize];
                else
                    extendedMap[x, y] = 1;
            }
        }

        // Generate the floor or draw the noise map as a texture
        // if (drawMode == DrawMode.NoiseMap)
            // DrawTexture(TextureGenerator.TextureFromHeightMap(extendedMap));
        if (drawMode == DrawMode.Terrain)
            PlaceTerrain(extendedMap);
    }

    void ProcessMap() {
        List<List<Coord>> emptyRegions = GetRegions(1);

        // Loop through each empty region
        foreach(List<Coord> emptyRegion in emptyRegions) {
            // Check if the regions size is less than the pre-defined size
            if (emptyRegion.Count < emptyThresholdSize) {
                // Loop through each tile in the empty region
                foreach (Coord tile in emptyRegion) {
                    // Set each tile to a terrain region
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        List<List<Coord>> terrainRegions = GetRegions(0);
        List<Room> survivingRooms = new List<Room>();

        // Loop through each terrain region
        foreach (List<Coord> terrainRegion in terrainRegions) {
            // Check if the regions size is less than the pre-defined size
            if (terrainRegion.Count < terrainThresholdSize) {
                // Loop through each tile in the terrain region
                foreach (Coord tile in terrainRegion) {
                    // Set each tile to an empty region
                    map[tile.tileX, tile.tileY] = 1;
                }
            } else {
                // If the terrain is proper size we store it in our survining rooms array
                survivingRooms.Add(new Room(terrainRegion, map));
            }
        }
        // Sort the array by the room size
        survivingRooms.Sort();
        // Set the biggest room as the main room
        survivingRooms[0].isMainRoom = true;
        // Make sure that all rooms can access the main room
        survivingRooms[0].isMainRoomAccessible = true;
        // Connect all rooms
        ConnectClosestRooms(survivingRooms);
    }

    public void DrawTexture(Texture2D texture) {
        // Clearing any floor pieces left in the scene
        // FloorGeneration.ClearFloor(transform);
        // Setting the plane to active
        textureRender.gameObject.SetActive(true);
        // Apllying the texture I created
        textureRender.sharedMaterial.mainTexture = texture;
        // Scaling the map accordingly
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10.0f;
    }

    public void PlaceTerrain(int[,] map) {
        // Clearing any floor pieces left in the scene
        // FloorGeneration.ClearFloor(transform);
        // Setting the plane to in-active
        textureRender.gameObject.SetActive(false);
        // Using my floor generator to place floor pieces base on the noise map
        // FloorGeneration.GenerateFloor(map, transform, groundPrefab, groundSpacer);
    }

    void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false) {
        int bestDistance = 0;
        bool possibleConnection = false;
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();

        if (forceAccessibilityFromMainRoom) {
            foreach (Room room in allRooms) {
                if (room.isMainRoomAccessible)
                    roomListB.Add(room);
                else
                    roomListA.Add(room);
            }
        } else {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        foreach (Room roomA in roomListA) {
            if (!forceAccessibilityFromMainRoom) {
                possibleConnection = false;
                if (roomA.connectedRooms.Count > 0)
                    continue;
            }
            foreach (Room roomB in roomListB) {
                if (roomA == roomB || roomA.IsConnected(roomB))
                    continue;
                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++) {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++) {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];
                        int dstBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (dstBetweenRooms < bestDistance || !possibleConnection) {
                            bestDistance = dstBetweenRooms;
                            possibleConnection = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }
            if (possibleConnection && !forceAccessibilityFromMainRoom)
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
        }
        if (possibleConnection && forceAccessibilityFromMainRoom) {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }
        if (!forceAccessibilityFromMainRoom)
            ConnectClosestRooms(allRooms, true);
    }

    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB) {
        Room.ConnectRooms(roomA, roomB);
        Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.red, 10);

        // Get the line of tiles that connects the rooms
        List<Coord> line = GetLine(tileA, tileB);
        // Loop through each tile and draw a circle of terrain tiles
        foreach (Coord c in line) {
            DrawCircle(c, pathSize);
        }
    }

    void DrawCircle(Coord c, int r) {
        // Loop through the circles radius
        for (int x = -r; x < r; x++) {
            for (int y = -r; y < r; y++) {
                // Check if it is actually within our circle
                if (x * x + y * y <= r * r) {
                    // Find our draw x
                    int drawX = c.tileX + x;
                    // Find our draw y
                    int drawY = c.tileY + y;
                    // If the point is in the maps range then set it to a terrain piece
                    if (IsInMapRange(drawX, drawY))
                        map[drawX, drawY] = 0;
                }
            }
        }
    }

    List<Coord> GetLine(Coord from, Coord to) {
        List<Coord> line = new List<Coord>();
        bool inverted = false;
        int x = from.tileX;
        int y = from.tileY;
        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);
        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        // If the longest is greater than shortest we must switch the values and set inverted to true
        if (longest < shortest) {
            inverted = true;
            longest = shortest;
            shortest = Mathf.Abs(dx);
            step = gradientStep;
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;

        // Loop through the furthest dst whether its X or Y
        for (int i = 0; i < longest; i++) {
            // Add a new tile along every point on the line
            line.Add(new Coord(x, y));
            // Incrementing the x or the y by the XIncrement based on whether its inverted or not
            if (inverted)
                y += step;
            else
                x += step;

            // Icrement the Yincrement by the shortest dst
            gradientAccumulation += shortest;
            // Only if the line is not verticle or flat
            if (gradientAccumulation >= longest) {
                // Increment the x or the y by the YIncrement base on whether its inverted or not
                if (inverted)
                    x += gradientStep;
                else
                    y += gradientStep;
                // Subtract the longest so the gradientAccumulation is not always greater
                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    Vector3 CoordToWorldPoint(Coord tile) {
        if (drawMode == DrawMode.Terrain)
            return new Vector3((-width / 2 - 0.25f + tile.tileX) * groundSpacer, 4, (-height / 2 - 0.25f + tile.tileY) * groundSpacer);
        return new Vector3(-width / 2 + 0.5f + tile.tileX, 2, -height / 2 + 0.5f + tile.tileY);
    }

    List<List<Coord>> GetRegions(int tileType) {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] flags = new int[width, height];

        // Loop through the whole map looking for each region
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                // Checks that we havent already looked at this spot and that its the right tile type
                if (flags[x, y] == 0 && map[x, y] == tileType) {
                    List<Coord> newRegion = GetRegionTiles(x, y);

                    // Add the new region to our list of regions
                    regions.Add(newRegion);
                    // Loop through each tile in the region
                    foreach (Coord tile in newRegion) {
                        // Set the flag so we never look at it again
                        flags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    List<Coord> GetRegionTiles(int startX, int startY) {
        List<Coord> tiles = new List<Coord>();
        int[,] flags = new int[width, height];
        int tileType = map[startX, startY];
        Queue<Coord> queue = new Queue<Coord>();

        // Start the tiles queue starting with the startX and startY
        queue.Enqueue(new Coord(startX, startY));
        // Set the first flag so that we dont look at the start point anymore
        flags[startX, startY] = 1;
        while (queue.Count > 0) {
            Coord tile = queue.Dequeue();

            // Add the tile to the tiles list
            tiles.Add(tile);
            // Loop through a 1 pixel radius around our current point
            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
                    // If the point is in the map and its either equal to x or y
                    if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX)) {
                        // Making sure we havent already checked this tile and it is the right tile type
                        if (flags[x, y] == 0 && map[x, y] == tileType) {
                            // Set our flag so we dont look at it again
                            flags[x, y] = 1;
                            // Enqueue the tile
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    bool IsInMapRange(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    void RandomFillMap() {
        // If useRandomSeed is true then we generate a random seed base on the time which will always be changing
        if (useRandomSeed)
            seed = Time.time.ToString();

        // We pass the seed into our systems random number generator as a hascode
        System.Random pseudoRand = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                // Defining the edges as empty space / a wall. otherwise generates randomly a number between 1 and 0.
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    map[x, y] = 1;
                else
                    map[x, y] = (pseudoRand.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }
    }

    void SmoothMap() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                // Count surrounding walls and store it
                int neighourWallTiles = GetSurroundingWallCount(x, y);

                // if there is more than 4 walls around then it is a wall. Otherwise if it is less than 4 it is a floor.
                if (neighourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighourWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY) {
        int wallCount = 0;

        // Loop through in a square around the desinated point
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {
                // Check if that point is within the map or we will get weird numbers. Otherwise count up by one to get the right count.
                if (IsInMapRange(neighbourX, neighbourY)) {
                    // Make sure we are not counting its self in the wall count
                    if (neighbourX != gridX || neighbourY != gridY)
                        wallCount += map[neighbourX, neighbourY];
                } else {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    struct Coord {
        public int tileX;
        public int tileY;

        public Coord(int x, int y) {
            tileX = x;
            tileY = y;
        }
    }

    class Room : IComparable<Room> {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public int roomSize;
        public bool isMainRoomAccessible;
        public bool isMainRoom;

        // Basic constructor so that we can instaniate a room without the tiles or map
        public Room() {
        }

        // Assigning the tile, edgeTiles, connectedTooms and roomSize.
        public Room(List<Coord> roomTiles, int[,] map) {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();
            edgeTiles = new List<Coord>();

            // Loops through all the tiles and seeing if its a edge tile
            foreach (Coord tile in tiles) {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
                        // If there is an empty space anywhere near the tile add the tile to the edgeTiles list.
                        if ((x == tile.tileX || y == tile.tileY) && map[x, y] == 1)
                            edgeTiles.Add(tile);
                    }
                }
            }
        }

        public void SetMainRoomAccessiblity() {
            // If the isMainRoomAccessible is not true
            if (!isMainRoomAccessible) {
                // Set isMainRoomAccessible equal to true
                isMainRoomAccessible = true;
                // Loop through any connected rooms recursively until all rooms connected are set to true
                foreach(Room connectedRoom in connectedRooms) {
                    connectedRoom.SetMainRoomAccessiblity();
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB) {
            // If the either room has isMainRoomAccessible set to true the other room will also set theres to true
            if (roomA.isMainRoomAccessible)
                roomB.SetMainRoomAccessiblity();
            else if (roomB.isMainRoomAccessible)
                roomA.SetMainRoomAccessiblity();
            // Then roomA gets added to roomB's connectedRooms list and vis versa
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool IsConnected(Room otherRoom) {
            return connectedRooms.Contains(otherRoom);
        }

        // Used for the IComparable implementation
        public int CompareTo(Room otherRoom) {
            return otherRoom.roomSize.CompareTo(roomSize);
        }
    }
}
