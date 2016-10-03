using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.Entities
{
    public enum Direction
    {
        Up,
        Down,
        Right,
        Left
    }

    class Destroyer : Match3GameElement
    {
        private Direction _direction;
        private Vector2 _velocity;
        private float _rotation;

        private int _fieldWidth;
        private int _fieldHeight;

        public Destroyer(Direction direction, Vector2 position, ElementColor color, int fieldWidth, int fieldHeight, Match3Game game) : base(game)
        {
            CurrentColor = color;
            Position = position;
            State = ElementState.Moving;

            _direction = direction;
            _fieldWidth = fieldWidth;
            _fieldHeight = fieldHeight;
        }


        public override void Initialize()
        {
            Tuple<int, Vector2> rotationAndVelocity = getRotationAndVelocity();

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _velocity = rotationAndVelocity.Item2;
            _rotation = rotationAndVelocity.Item1 * MathHelper.Pi / 180;

            base.Initialize();
        }


        protected override void LoadContent()
        {
            _baseTexture = Game.Content.Load<Texture2D>(getBaseTextureName() + "-destroyer");

            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {
            if (State == ElementState.Moving)
            {
                if (isAtBorder())
                {
                    State = ElementState.Removed;
                    return;
                }
                Position += _velocity;
            }

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            if (State == ElementState.Removed) { return; }

            Vector2 origin; 

            origin.X = Constants.GameFieldCell / 2;
            origin.Y = Constants.GameFieldCell / 2;

            _spriteBatch.Begin();
            _spriteBatch.Draw(_baseTexture, Position, null, Color.White, _rotation, origin, 1.0f, SpriteEffects.None, 0f);
            _spriteBatch.End();

            base.Draw(gameTime);
        }


        private Tuple<int, Vector2> getRotationAndVelocity()
        {
            float speed = Constants.AnimationVelocity / 1.5f;

            switch (_direction)
            {
                case Direction.Up:
                    return new Tuple<int, Vector2>(180, new Vector2(0, -speed));
                case Direction.Down:
                    return new Tuple<int, Vector2>(0, new Vector2(0, speed));
                case Direction.Right:
                    return new Tuple<int, Vector2>(270, new Vector2(speed, 0));
                case Direction.Left:
                    return new Tuple<int, Vector2>(90, new Vector2(-speed, 0));
                default:
                    return new Tuple<int, Vector2>(0, new Vector2(0, 0));
            }
        }


        private bool isAtBorder()
        {
            switch (_direction)
            {
                case Direction.Up:
                    return Position.Y < Constants.GameFieldY;
                case Direction.Down:
                    return Position.Y > Constants.GameFieldY + _fieldHeight;
                case Direction.Right:
                    return Position.X > Constants.GameFieldX + _fieldWidth;
                case Direction.Left:
                    return Position.X < Constants.GameFieldX;
                default:
                    return false;
            }
        }

    }
}
