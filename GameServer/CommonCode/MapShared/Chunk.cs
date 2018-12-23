﻿using ServerCore.Game.Entities;
using System;
using System.Collections.Generic;

namespace MapHandler
{
    public class Chunk
    {
        public static int SIZE = 16;

        public int x;
        public int y;

        public MapTile[,] Tiles = new MapTile[SIZE, SIZE];

        public Dictionary<EntityType, List<Entity>> EntitiesInChunk = new Dictionary<EntityType, List<Entity>>();

        public Chunk()
        {
            EntitiesInChunk.Add(EntityType.PLAYER, new List<Entity>());
            EntitiesInChunk.Add(EntityType.MONSTER, new List<Entity>());
        }
    }
}

