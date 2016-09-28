using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.Entities
{
    class BombElement : BallElement
    {
        public BombElement(BallColor color, Vector2 position, Match3Game game) : base(color, position, game) { }
    }
}
