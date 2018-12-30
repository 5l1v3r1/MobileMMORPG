﻿using Common.Entity;
using MapHandler;
using UnityEngine;

namespace Assets.Code.Game
{
    public class PlayerWrapper : LivingEntity
    {
        public int Speed;
        public string SessionId;

        public GameObject PlayerObject;
        public MovingEntityBehaviour Movement;

        public Entity Target;
        public long NextAttackAt;
    }
}
