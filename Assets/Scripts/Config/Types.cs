using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config
{
    public class Types
    {


        public enum MeshType
        {
            City,
            Shop,
            Bridge,
            Tunnel,
            Crossing,
            Station
        }


        public enum Lane //El número indica a qué porcentaje esta del centro con respecto al ancho de los carriles
        {
            Left = -100,
            Center = 0,
            Right = 100,
            BetweenLeftCenter = -50,
            BetweenRightCenter = 50
        }

        public class Segment
        {

            public enum SegmentType
            {
                Type1,
                Type2,
                Type3,
                Type4,
                Type5,
                Type6
            }

            public enum SegmentLength
            {
                Length10 = 10,
                Length20 = 20,
                Length30 = 30,
                Length50 = 50,
                Length80 = 80,
                Length85 = 85,
                Length90 = 90,
                Length95 = 95,
                Length100 = 100

            }
        }

        public class Obstacle
        {
            public enum ObstacleType
            {
                Jump,
                Slide,
                SlideOrJump,
                BlockRamp,
                Block,
                DoubleBlock,
                Wall,
                TrafficLight,
                Car,
                CarWithRamp,
                PassageLeft,
                PassageCenter,
                PassageRight,
                PassageLeftRight,
                PassageColumns,
                TrafficLightCornerGreen,
                TrafficLightCornerRed,
                InjureLow,
                PassageLeftDouble,
                PassageCenterDouble,
                PassageRightDouble,
                PassageLeftRightDouble,
                Truck,
                TruckWithRamp,
                Columns,
                FloatingCars
            }

            public enum MovingObstacleSpeed
            {
                None,
                Slow,
                Fast
            }
        }

        public class Collectible
        {

            public enum CollectibleLevel
            {
                Level1,
                Level2,
                Level3,
                Level4,
                Level5
            }

            public enum CollectibleType
            {
                BigCoin,
                BonusCoin,
                Coin,
                CoinMagnet,
                CoinMultiplier,
                Gold,
                ScoreMultiplier,
                Shield,
                Turbo
            }

            public enum CollectAnimation
            {
                PowerUp,
                Coin,
                Turbo
            }
        }

        public enum SFX
        {
            //CoinCollected,
            //BigCoinCollected,
            //GoldCollected,
            //BonusCoinCollected,
            //CoinMagnetCollected,
            //CoinMagnetBackground,
            //CoinMagnetEnded,
            //CoinMultiplierCollected,
            //CoinMultiplierBackground,
            //CoinMultiplierEnded,
            //ScoreMultiplierCollected,
            //ScoreMultiplierBackground,
            //ScoreMultiplierEnded,
            //ShieldCollected,
            //ShieldBackground,
            //ShieldEnded,
            //ShieldUsed,
            //TurboCollected,
            //TurboBackground,
            //TurboEnded,
            //PlayerDeath,
            //PlayerInjury,
            //PlayerStopInjury,
            AbrirCajaPizzaItem,//BOTONREWARD
            AgarrarItemEspecial,
            AgarrarMonedaEspecial,
            AgarrarMonedaNormal,
            AgarrarPowerUp,
            AgarrarTurbo,
            BitTip,
            BotonSeleccionar,
            BotonVolverCancelar,
            CambioDeCarril,
            ChoqueContraObstaculo,
            ChoqueMuerte,
            ColeadaSlideMoto,
            DisparoPizzaEspecial,
            DisparoPizzaNormal,
            Explosion,
            ItemSorpresaDeCajaPizza,
            LoopMotorMotito,
            LoopPowerUpIman,
            NuevoRecordPuntajeMaximo,
            PisadaTrex,
            RisaVillano,
            TrexVoice,
            Darker,
            GameplayTierra,
            RunCyber,
            Run,
            BossMusic,
            BotonVolver,
            BotonAd,
            MusicaMenu
        }
    }
}
