using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config
{
    public class AnimationTriggers
    {
        public const string BossJumpRight = "JumpRight";
        public const string BossJumpLeft = "JumpLeft";
        public const string PowerupPickup = "PowerupPickup";

        public const string BossStop ="Stop";

        public const string BossChasingLose = "Lose";

        public class PauseMenu
        {
            public const string Show = "Show";
        }

        public class GameMenu
        {
            public const string DiamondCollect = "Collect";
        }

        public class Player
        {
            public const string Jump = "Jump";
            public const string Sliding = "Sliding";
            public const string Dead = "Dead";
            public const string StartRunning = "StartRunning";
            public const string Grounded = "Grounded";
            public const string Injured = "Injured";
            public const string GirarIzquierda = "GirarIzq";
            public const string GirarDerecha = "GirarDer";

            public const string PowerUpPickup = "PowerupPickup";
            public const string Iman = "Iman";
            public const string Turbo = "Turbo";

            public const string OnJump = "OnJump";

            public const string Landed = "Landed";
        }

        public class Collectible
        {
            public const string Collected = "Collected";
        }
    }
}

