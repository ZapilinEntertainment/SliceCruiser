using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IslandConstructor
{
    private enum CellType :byte { Water, Island, Empty}

    public static Transform ConstructIsland(Vector3 position)
    {
        //input
        const int MAP_RESOLUTION = 16;
        float entrancePositionKey = Random.value, harbourPositionKey1 = Random.value, harbourPositionKey2 = Random.value;
        //
        (int, int) entrancePosition = DefineEdgePosition(entrancePositionKey, MAP_RESOLUTION),
            harbourPosition = DefineHarbourPosition(harbourPositionKey1, harbourPositionKey2, MAP_RESOLUTION);



        var g = new GameObject("island");
        var map = new CellType[MAP_RESOLUTION, MAP_RESOLUTION];
        return g.transform;
    }

    public static Transform ConstructIsland2(Vector3 position)
    {
        //input
        const int MAP_RESOLUTION = 16;
        float entrancePositionKey = Random.value, harbourPositionKey1 = Random.value, harbourPositionKey2 = Random.value;
        //
        (int, int) entrancePosition = DefineEdgePosition(entrancePositionKey, MAP_RESOLUTION), 
            harbourPosition = DefineHarbourPosition(harbourPositionKey1, harbourPositionKey2, MAP_RESOLUTION);
        
        

        var g = new GameObject("island");
        var map = new CellType[MAP_RESOLUTION, MAP_RESOLUTION];
        return g.transform;
    }

    private static (int, int) DefineEdgePosition(float key, int mapResolution)
    {
        int positionIndex = (int)(key * (4 * (mapResolution - 1)));
        if (positionIndex < 2 * mapResolution)
        { // верхняя или нижняя строчка
            if (positionIndex < mapResolution) return (positionIndex, 0);
            else return (positionIndex - mapResolution, mapResolution - 1);
        }
        else
        {
            //крайние столбцы
            positionIndex -= 2 * mapResolution;
            if (positionIndex + 2 < mapResolution) return (0, positionIndex + 1);
            else return (mapResolution - 1, positionIndex + 1);
        }
    }
    private static (int, int) DefineHarbourPosition(float key1,float key2, int mapResolution)
    {
        var v = Quaternion.AngleAxis(key2 * Random.value, Vector3.up) * (Vector3.forward * (key1 * key1));
        v *= (mapResolution - 2);
        return ((int)v.x + 1, (int)v.z + 1);
    }
}
