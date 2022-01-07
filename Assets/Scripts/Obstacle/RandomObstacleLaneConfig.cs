using System;


[Serializable]
public class RandomObstacleLaneConfig
{
    public int chancesToSpawn;
    public Config.Types.Lane[] lanes;
    public Config.Types.Obstacle.MovingObstacleSpeed[] speeds;
    public Config.Types.Obstacle.ObstacleType[] obstacles;


}
