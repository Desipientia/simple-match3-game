using System.Diagnostics;
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
        Normal,
        Moving,
        Waiting,
        Vanishing,
        Removed
    }


    public enum BonusType
    {
        None,
        VerticalLine,
        HorisontalLine,
        Bomb,
    }


    class BallElement : DrawableGameComponent
    {
        public bool IsActive = false;
        public bool IsNew = false;

        public Point GridPosition;
        public BallColor CurrentColor;
        public Vector2 Position;

        public BallState State;
        public BonusType whatBonusNext = BonusType.None;

        protected BallState _nextState;
        protected GameComponentCollection _waitForBalls;

        protected Texture2D _baseTexture;
        protected Texture2D _activeTexture;
        protected SpriteBatch _spriteBatch;

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
            IsNew = true;

            _baseTexture = ball._baseTexture;
            _activeTexture = ball._activeTexture;
            _spriteBatch = ball._spriteBatch;
        }


        public override void Initialize()
        {
            Position = new Vector2(0, 0);
            State = BallState.Normal;
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            

            base.Initialize();
        }


        protected override void LoadContent()
        {
            string baseTextureName = getBaseTextureName() + "-ball";
            string activeTextureName = baseTextureName + "-active";

            _baseTexture = Game.Content.Load<Texture2D>(baseTextureName);
            _activeTexture = Game.Content.Load<Texture2D>(activeTextureName);

            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {
            IsNew = false;

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
                    State = BallState.Normal;
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
            switch (State)
            {
                case BallState.Waiting:
                case BallState.Removed:
                    return;
                case BallState.Normal:
                case BallState.Moving:
                    drawNormal();
                    break;
                case BallState.Vanishing:
                    drawVanishing();
                    break;
                default:
                    break;
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


        protected string getBaseTextureName()
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
            return textureName;
        }

        
        protected void drawNormal()
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(IsActive ? _activeTexture : _baseTexture, Position, Color.White);
            _spriteBatch.End();
        }


        protected void drawVanishing()
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_baseTexture, Position, null, Color.White, 0f, new Vector2(0, 0), _scale, SpriteEffects.None, 0f);
            _spriteBatch.End();
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
