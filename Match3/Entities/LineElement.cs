using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.Entities
{
    class LineElement : BallElement
    {
        private BonusType LineType;
        private Vector2 _origin;

        public LineElement(BonusType type, ElementColor color, Point position, Match3Game game) : base(color, position, game)
        {
            LineType = type;
        }

        public LineElement(LineElement ball) : base(ball)
        {
            LineType = ball.LineType;
        }

        public LineElement(BonusType type, BallElement ball) : base(ball)
        {
            LineType = type;
        }


        public override void Initialize()
        {
            _origin = new Vector2(0, 0);

            base.Initialize();
        }


        protected override void LoadContent()
        {
            string baseTextureName = getBaseTextureName() + "-line";
            string activeTextureName = baseTextureName + "-active";

            _baseTexture = Game.Content.Load<Texture2D>(baseTextureName);
            _activeTexture = Game.Content.Load<Texture2D>(activeTextureName);
        }


        protected override void drawNormal()
        {
            float rotation = LineType == BonusType.HorisontalLine ? 0 : (90 * MathHelper.Pi / 180);

            _origin.X = Constants.GameFieldCell / 2;
            _origin.Y = Constants.GameFieldCell / 2;

            Vector2 shiftedPosition = new Vector2(Position.X + _origin.X, Position.Y + _origin.Y);

            _spriteBatch.Begin();
            _spriteBatch.Draw(IsActive ? _activeTexture : _baseTexture, shiftedPosition, null, Color.White, rotation, _origin, 1.0f, SpriteEffects.None, 0f);
            _spriteBatch.End();
        }


        public Tuple<Direction, Direction> GetDestroyersDirection()
        {
            switch (LineType)
            {
                case BonusType.VerticalLine:
                    return new Tuple<Direction, Direction>(Direction.Up, Direction.Down);
                case BonusType.HorisontalLine:
                    return new Tuple<Direction, Direction>(Direction.Right, Direction.Left);
                default:
                    return null;
            }
        }

    }
}
