using System;
using System.Collections.Generic;
using System.Text;

namespace ICFP08
{
    public class MobileObject
    {
        Vector2d position;
        float direction;
        float speed;
        float radius;
    }

    public class Rover : MobileObject
    {
    }

    public class Martian : MobileObject
    {
    }

    class WorldState
    {
        private StaticObjects m_objects;

    }
}
