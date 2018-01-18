using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloorGeneration {

    private static int width;
    private static int height;
    private static int[,] map;
    private static List<Room> survivingRooms = new List<Room>();
    private static List<Room> corridors = new List<Room>();
    private static bool isTiles;

    public static int GetSurvivingRoomCount() {
        return survivingRooms.Count;
    }

    public static int GetCorridorCount() {
        return corridors.Count;
    }

    public static List<Coord> GetSurvivingRoomEdges(int index) {
        return survivingRooms[index].edgeTiles;
    }

    public static void GenerateFloor(int[,] inMap, Transform parent, TerrainData terrainData) {
        isTiles = true;
        width = TerrainGenerator.GetWidth();
        height = TerrainGenerator.GetHeight();
        map = SmoothMap(inMap, terrainData);
        ProcessMap(terrainData);
        BuildFloorPieces(parent, terrainData);
    }

    public static Texture2D GenerateFloorTexture(int[,] inMap, TerrainData terrainData) {
        isTiles = false;
        width = TerrainGenerator.GetWidth();
        height = TerrainGenerator.GetHeight();
        map = SmoothMap(inMap, terrainData);
        ProcessMap(terrainData);
        return TextureGenerator.TextureFromHeightMap(new HeightMap(TerrainGenerator.ConvertMapToFloat(map), 0, 1));
    }

    private static void BuildFloorPieces(Transform parent, TerrainData terrainData) {
        Vector3 rot = new Vector3(270.0f, 0.0f, 0.0f);
        Vector3 groundSpacer = new Vector3(8.0f, 1.0f, 8.0f);

        // If I want to do it this way, I need to add all the corridors to seprate rooms and add all the rooms to an array called corridors
        foreach (Room room in survivingRooms) {
            foreach (Coord tile in room.tiles) {
                if (room.edgeTiles.Contains(tile)) {
                    // Print the edge tile piece that I want unless the piece is a cooridor
                    // Different scale just to be able to visualize the edges
                    ObjectSpawner.QueueObjectSpawn(new ObjectSpawnerData(terrainData.groundPrefab, new Vector3(tile.tileX, 0.0f, tile.tileY), new Vector3(2.0f, 2.0f, 1.0f), rot, groundSpacer, parent));
                } else {
                    // Print the regular floor piece
                    ObjectSpawner.QueueObjectSpawn(new ObjectSpawnerData(terrainData.groundPrefab, new Vector3(tile.tileX, 0.0f, tile.tileY), new Vector3(4.0f, 4.0f, 1.0f), rot, groundSpacer, parent));
                }
            }
        }
        // Then loop through all cooridors printing them
        foreach (Room room in corridors) {
            foreach (Coord tile in room.tiles) {
                if (room.edgeTiles.Contains(tile)) {
                    // Print the edge tile pieces for cooridors
                    // Different scale just to be able to visualize the edges
                    ObjectSpawner.QueueObjectSpawn(new ObjectSpawnerData(terrainData.groundPrefab, new Vector3(tile.tileX, 0.0f, tile.tileY), new Vector3(2.0f, 2.0f, 1.0f), rot, groundSpacer, parent));
                } else {
                    // Print the cooridor floor pieces
                    ObjectSpawner.QueueObjectSpawn(new ObjectSpawnerData(terrainData.groundPrefab, new Vector3(tile.tileX, 0.0f, tile.tileY), new Vector3(4.0f, 4.0f, 1.0f), rot, groundSpacer, parent));
                }
            }
        }
    }

    private static void ProcessMap(TerrainData terrainData) {
        List<List<Coord>> emptyRegions = GetRegions(1);
        List<List<Coord>> terrainRegions = GetRegions(0);

        // Loop through each empty region
        foreach (List<Coord> emptyRegion in emptyRegions) {
            // Check if the regions size is less than the pre-defined size
            if (emptyRegion.Count < terrainData.emptyThresholdSize) {
                // Loop through each tile in the empty region
                foreach (Coord tile in emptyRegion) {
                    // Set each tile to a terrain region
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        // If there are rooms stored we are going to wipe them and restart
        if (survivingRooms.Count > 0)
            survivingRooms.Clear();
        // Loop through each terrain region
        foreach (List<Coord> terrainRegion in terrainRegions) {
            // Check if the regions size is less than the pre-defined size
            if (terrainRegion.Count < terrainData.terrainThresholdSize) {
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

        // If there are cooridors store we are going to wipe them to restart
        if (corridors.Count > 0)
            corridors.Clear();
        if (terrainData.connectionType != TerrainData.ConnectionType.None) {
            if (terrainData.connectionType == TerrainData.ConnectionType.All) {
                // Sort the array by the room size
                survivingRooms.Sort();
                // Set the biggest room as the main room
                survivingRooms[0].isMainRoom = true;
                // Make sure that all rooms can access the main room
                survivingRooms[0].isMainRoomAccessible = true;
            }
            // Connect all rooms
            ConnectClosestRooms(survivingRooms, terrainData);
        }
    }

    private static int[,] SmoothMap(int[,] map, TerrainData terrainData) {
        for (int i = 0; i < terrainData.numOfSmoothingPasses; i++) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    // Count surrounding walls and store it
                    int neighourWallTiles = GetSurroundingWallCount(x, y, map);

                    // if there is more than 4 walls around then it is a wall. Otherwise if it is less than 4 it is a floor.
                    if (neighourWallTiles > 4)
                        map[x, y] = 1;
                    else if (neighourWallTiles < 4)
                        map[x, y] = 0;
                }
            }
        }

        return map;
    }

    private static int GetSurroundingWallCount(int gridX, int gridY, int[,] map) {
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

    private static List<List<Coord>> GetRegions(int tileType) {
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

    private static List<Coord> GetRegionTiles(int startX, int startY) {
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
    
    private static void ConnectClosestRooms(List<Room> allRooms, TerrainData terrainData, bool forceAccessibilityFromMainRoom = false) {
        int bestDst = 0;
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
                    roomListA.Add(room);
                else
                    roomListB.Add(room);
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
                        int dstBtwnRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (dstBtwnRooms < bestDst || !possibleConnection) {
                            bestDst = dstBtwnRooms;
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
                CreatePassage(terrainData, bestRoomA, bestRoomB, bestTileA, bestTileB);
        }
        if (possibleConnection && forceAccessibilityFromMainRoom) {
            CreatePassage(terrainData, bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, terrainData, true);
        }
        if (!forceAccessibilityFromMainRoom)
            ConnectClosestRooms(allRooms, terrainData, true);
    }

    private static void CreatePassage(TerrainData terrainData, Room roomA, Room roomB, Coord tileA, Coord tileB) {
        // Store all the new tiles created for the passageway
        List<Coord> passageTiles = new List<Coord>();
        // Get the line of tiles that connects the rooms
        List<Coord> line = GetLine(tileA, tileB);

        Room.ConnectRooms(roomA, roomB);
        Debug.DrawLine(TerrainGenerator.CoordToWorldPoint(tileA.tileX, tileA.tileY, terrainData.groundSpacer, isTiles), TerrainGenerator.CoordToWorldPoint(tileB.tileX, tileB.tileY, terrainData.groundSpacer, isTiles), Color.red, 5);
        // Loop through each tile and draw a circle of terrain tiles
        foreach (Coord c in line) {
            List<Coord> newTiles = DrawCircle(c, terrainData.pathSize);

            // Loop through all tiles
            foreach (Coord tile in newTiles) {
                // Make sure it hasnt already been added to the list
                if (!passageTiles.Contains(tile)) {
                    passageTiles.Add(tile);
                    // If our tile is an edge tile of a room remove it from the list
                    if (roomA.edgeTiles.Contains(tile))
                        roomA.edgeTiles.Remove(tile);
                    if (roomA.tiles.Contains(tile))
                        roomA.tiles.Remove(tile);
                    if (roomB.edgeTiles.Contains(tile))
                        roomB.edgeTiles.Remove(tile);
                    if (roomB.tiles.Contains(tile))
                        roomB.tiles.Remove(tile);
                }
            }
        }
        corridors.Add(new Room(passageTiles, map));
    }

    private static List<Coord> DrawCircle(Coord c, int r) {
        List<Coord> addedTiles = new List<Coord>();

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
                    if (IsInMapRange(drawX, drawY)) {
                        map[drawX, drawY] = 0;
                        addedTiles.Add(new Coord(drawX, drawY));
                    }
                }
            }
        }

        return addedTiles;
    }

    private static List<Coord> GetLine(Coord from, Coord to) {
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

    private static bool IsInMapRange(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    class Room : IComparable<Room> {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public int roomSize;
        public bool isMainRoomAccessible;
        public bool isMainRoom;

        public Room() {
        }

        public Room(List<Coord> roomTiles, int[,] map) {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();
            edgeTiles = new List<Coord>();

            // Loops through all the tiles and seeing if its a edge tile
            foreach (Coord tile in tiles) {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
                        // If there is an empty space anywhere near and the tile is not its self the tile add the tile to the edgeTiles list.
                        if ((x == tile.tileX || y == tile.tileY) && map[x, y] == 1)
                            edgeTiles.Add(tile);
                    }
                }
            }
        }

        public void SetMainRoomAccessibility() {
            // If the isMainRoomAccessible is not true
            if (!isMainRoomAccessible) {
                isMainRoomAccessible = true;
                // Loop through any connected rooms recursively until all rooms connected are set to true
                foreach(Room connectedRoom in connectedRooms) {
                    connectedRoom.SetMainRoomAccessibility();
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB) {
            // If the either room has isMainRoomAccessible set to true the other room will also set theres to true
            if (roomA.isMainRoomAccessible)
                roomB.SetMainRoomAccessibility();
            else if (roomB.isMainRoomAccessible)
                roomB.SetMainRoomAccessibility();
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

    // Need a way to store my floor pieces better incase I want different types of terrain (Use my coords??? return a bunch of coords with types and instaniate after)
    // Also need a reference for the terrain so I can spawn objects above them
}