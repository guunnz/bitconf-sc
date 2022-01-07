using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config
{
    public class Lane
    {

        public const float laneDistance = 3f;


        public static float GetLanePosition(Config.Types.Lane lane)
        {
            return (int)lane * laneDistance / 100;
        }

    }
}
