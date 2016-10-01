using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3
{
    public enum BallColor
    {
        Red,
        Yellow,
        Green,
        Blue,
        Light
    }


    public enum BallState
    {
        Visible,
        Moving,
        Waiting,
        Vanishing,
        Removed
    }


    class BallElement : DrawableGameComponent
    {
        public bool IsActive = false;
        public Point GridPosition;
        public BallColor CurrentColor;
        public Vector2 Position;

        public BallState State;

        private BallState _nextState;
        private GameComponentCollection _waitForBalls;

        private Texture2D _baseTexture;
        private Texture2D _activeTexture;
        private SpriteBatch _spriteBatch;

        private Vector2 _moveDirection;

        private float _scale;
        private float _scaleVelocity;
        

        public BallElement(BallColor ballColor, Point position, Match3Game game) : base(game)
        {
            CurrentColor = ballColor;
            GridPosition = position;
        }

        public BallElement(BallElement ball) : base(ball.Game)
        {
            IsActive = ball.IsActive;
            GridPosition = ball.GridPosition;
            CurrentColor = ball.CurrentColor;

            Position = ball.Position;
            _baseTexture = ball._baseTexture;
            _activeTexture = ball._activeTexture;
            _spriteBatch = ball._spriteBatch;
        }


        public override void Initialize()
        {
            Position = new Vector2(0, 0);
            State = BallState.Visible;
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            

            base.Initialize();
        }


        protected override void LoadContent()
        {
            string baseTextureName = getBaseTextureName();
            string activeTextureName = baseTextureName + "-active";

            _baseTexture = Game.Content.Load<Texture2D>(baseTextureName);
            _activeTexture = Game.Content.Load<Texture2D>(activeTextureName);

            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {
            if (State == BallState.Removed) { return; }

            if (State == BallState.Waiting)
            {
                foreach (BallElement ball in _waitForBalls)
                {
                    if (ball.State == BallState.Moving || ball.State == BallState.Vanishing) { return; }
                }
                State = _nextState;
            }

            int endPositionX = Constants.GameFieldX + Constants.GameFieldCell * GridPosition.X;
            int endPositionY = Constants.GameFieldY + Constants.GameFieldCell * GridPosition.Y;

            if (State == BallState.Moving)
            {
                Position += getMoveDirection(endPositionX, endPositionY);

                if (Position.X >= endPositionX && Position.Y >= endPositionY)
                {
                    State = BallState.Visible;
                }
            }
            else
            {
                Position.X = endPositionX;
                Position.Y = endPositionY;

                if (State == BallState.Vanishing)
                {
                    _scale -= _scaleVelocity;

                    if (_scale <= 0)
                    {
                        State = BallState.Removed;
                    }
                }
            }

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            if (State == BallState.Waiting || State == BallState.Removed) { return; }

            if (State == BallState.Visible || State == BallState.Moving)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(IsActive ? _activeTexture : _baseTexture, Position, Color.White);
                _spriteBatch.End();
            }
            else
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(_baseTexture, Position, null, Color.White, 0f, new Vector2(0,0), _scale, SpriteEffects.None, 0f);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }


        public void Wait(GameComponentCollection forWhat, BallState nextState)
        {
            State = BallState.Waiting;
            _nextState = nextState;
            _waitForBalls = forWhat;
        }


        public void Vanish()
        {
            State = BallState.Vanishing;
            _scale = .8f;
            _scaleVelocity = .05f;
        }


        // Private methods.
        private string getBaseTextureName()
        {
            string textureName = "img/";

            switch (CurrentColor)
            {
                case BallColor.Red:
                    textureName += "red";
                    break;
                case BallColor.Yellow:
                    textureName += "yellow";
                    break;
                case BallColor.Green:
                    textureName += "green";
                    break;
                case BallColor.Blue:
                    textureName += "blue";
                    break;
                case BallColor.Light:
                    textureName += "light";
                    break;
                default:
                    return "";
            }
            return textureName + "-ball";
        }

        private Vector2 getMoveDirection(int endX, int endY)
        {
            float x = endX == Position.X ? 0 : endX > Position.X ? Constants.AnimationVelocity : 
                -Constants.AnimationVelocity; 
            float y = endY == Position.Y ? 0 : endY > Position.Y ? Constants.AnimationVelocity :
                -Constants.AnimationVelocity;
            
            return new Vector2(x, y);
        }

    }
}
