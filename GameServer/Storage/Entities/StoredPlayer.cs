﻿namespace Storage.Players
{
    public class StoredPlayer : RedisEntity
    {
        [RedisField("uid")]
        [RedisKey("u")]
        public string UserId { get; set; }

        [RedisField("x")]
        public int X { get; set; }

        [RedisField("y")]
        public int Y { get; set; }

        [RedisField("s")]
        public int MoveSpeed { get; set; }

        [RedisField("cspr")]
        public int ChestSpriteIndex { get; set; }

        [RedisField("cspr")]
        public int LegsSpriteIndex { get; set; }

        [RedisField("cspr")]
        public int HeadSpriteIndex { get; set; }

        [RedisField("cspr")]
        public int BodySpriteIndex { get; set; }

        [RedisField("l")]
        public string Login { get; set; }

        [RedisField("p")]
        public string Password { get; set; }

        [RedisField("e")]
        public string Email { get; set; }

        [RedisField("sid")]
        public string SessionId { get; set; }
    }
}
