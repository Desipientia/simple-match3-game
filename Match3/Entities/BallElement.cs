using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.Entities
{
    public enum BonusType
    {
        None,
        VerticalLine,
        HorisontalLine,
        Bomb,
    }


    class BallElement : Match3GameElement
    {
        public bool IsActive = false;
        public bool IsNew = false;

        public Point GridPosition;
        
        public BonusType whatBonusNext = BonusType.None;

        protected ElementState _nextState;
        protected GameComponentCollection _waitForBalls;
        
        protected Texture2D _activeTexture;

        private float _scale;
        private float _scaleVelocity;
        

        public BallElement(ElementColor ballColor, Point position, Match3Game game) : base(game)
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
            State = ElementState.Normal;
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

            if (State == ElementState.Removed) { return; }

            if (State == ElementState.Waiting)
            {
                foreach (BallElement ball in _waitForBalls)
                {
                    if (ball.State == ElementState.Moving || ball.State == ElementState.Vanishing) { return; }
                }
                State = _nextState;
            }

            int endPositionX = Constants.GameFieldX + Constants.GameFieldCell * GridPosition.X;
            int endPositionY = Constants.GameFieldY + Constants.GameFieldCell * GridPosition.Y;

            if (State == ElementState.Moving)
            {
                Position += getMoveDirection(endPositionX, endPositionY);

                if (Position.X >= endPositionX && Position.Y >= endPositionY)
                {
                    State = ElementState.Normal;
                }
            }
            else
            {
                Position.X = endPositionX;
                Position.Y = endPositionY;

                if (State == ElementState.Vanishing)
                {
                    _scale -= _scaleVelocity;

                    if (_scale <= 0)
                    {
                        State = ElementState.Removed;
                    }
                }
            }

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            switch (State)
            {
                case ElementState.Waiting:
                case ElementState.Removed:
                    return;
                case ElementState.Normal:
                case ElementState.Moving:
                    drawNormal();
                    break;
                case ElementState.Vanishing:
                    drawVanishing();
                    break;
                default:
                    break;
            }

            base.Draw(gameTime);
        }


        public void Wait(GameComponentCollection forWhat, ElementState nextState)
        {
            State = ElementState.Waiting;
            _nextState = nextState;
            _waitForBalls = forWhat;
        }


        public void Vanish()
        {
            State = ElementState.Vanishing;
            _scale = .8f;
            _scaleVelocity = .05f;
        }

        
        protected virtual void drawNormal()
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(IsActive ? _activeTexture : _baseTexture, Position, Color.White);
            _spriteBatch.End();
        }


        protected virtual void drawVanishing()
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
