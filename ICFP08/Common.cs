using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ICFP08
{
    public class InitializationMessage
    {
        public InitializationMessage(float dx, float dy, int time_limit, float min_sensor, float max_sensor, float max_speed, float max_turn, float max_hard_turn)
        {
            this.size = new SizeF(dx, dy);
            this.time_limit = time_limit;
            this.min_sensor = min_sensor;
            this.max_sensor = max_sensor;
            this.max_speed = max_speed;
            this.max_turn = max_turn;
            this.max_hard_turn = max_hard_turn;
        }

        public readonly SizeF size;
        public readonly int time_limit;
        public readonly float min_sensor;
        public readonly float max_sensor;
        public readonly float max_speed;
        public readonly float max_turn;
        public readonly float max_hard_turn;
    }

    public enum ObstacleType
    {
        Boulder,
        Crater,
        Home
    }

    public enum MoveType
    {
        Accelerate,
        Roll,
        Brake
    }

    public enum TurnType
    {
        HardLeft,
        Left,
        Straight,
        Right,
        HardRight
    }

    public class ObstacleMessage
    {
        public ObstacleMessage(ObstacleType type, float xpos, float ypos, float radius)
        {
            this.type = type;
            this.position = new Vector2d(xpos, ypos);
            this.radius = radius;
        }

        public readonly ObstacleType type;
        public readonly Vector2d position;
        public readonly float radius;
    }

    public class MartianMessage
    {
        public MartianMessage(float xpos, float ypos, float direction, float speed)
        {
            this.position = new Vector2d(xpos, ypos);
            this.direction = direction;
            this.speed = speed;
        }

        public readonly Vector2d position;
        public readonly float direction;
        public readonly float speed;
    }

    public class TelemetryMessage
    {
        public TelemetryMessage(int time_stamp, MoveType move_state, TurnType turn_state, 
            float xpos, float ypos, float direction, float speed,
            ObstacleMessage[] obstacles, MartianMessage[] martians)
        {
            this.time_stamp = time_stamp;
            this.move_state = move_state;
            this.turn_state = turn_state;
            this.position = new Vector2d(xpos, ypos);
            this.direction = direction;
            this.speed = speed;
            this.obstacles = obstacles;
            this.martians = martians;
        }

        public readonly int time_stamp;
        public readonly MoveType move_state;
        public readonly TurnType turn_state;
        public readonly Vector2d position;
        public readonly float direction;
        public readonly float speed;
        public readonly ObstacleMessage[] obstacles;
        public readonly MartianMessage[] martians;
    }
}
