using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3
{
    public enum BallColor
    {
        red,
        yellow,
        green,
        blue,
        light
    }


    class BallElement : DrawableGameComponent
    {
        public bool IsActive = false;
        public Point GridPosition;
        public BallColor CurrentColor;

        private Vector2 _position;
        private Texture2D _baseTexture;
        private Texture2D _activeTexture;
        private SpriteBatch _spriteBatch;

        private bool _isMoving = false;
        private Vector2 _moveDirection;

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

            _position = ball._position;
            _baseTexture = ball._baseTexture;
            _activeTexture = ball._activeTexture;
            _spriteBatch = ball._spriteBatch;
        }


        public override void Initialize()
        {
            _position = new Vector2(0, 0);
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
            int endPositionX = Constants.GameFieldX + Constants.GameFieldCell * GridPosition.X;
            int endPositionY = Constants.GameFieldY + Constants.GameFieldCell * GridPosition.Y;

            if (_isMoving)
            {
                _position += getMoveDirection(endPositionX, endPositionY);

                if (_position.X >= endPositionX && _position.Y >= endPositionY)
                {
                    _isMoving = false;
                }
            }
            else
            {
                _position.X = endPositionX;
                _position.Y = endPositionY;
            }

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(IsActive ? _activeTexture : _baseTexture, _position, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }


        public void Move()
        {
            _isMoving = true; 
        }


        // Private methods.
        private string getBaseTextureName()
        {
            string textureName = "img/";

            switch (CurrentColor)
            {
                case BallColor.red:
                    textureName += "red";
                    break;
                case BallColor.yellow:
                    textureName += "yellow";
                    break;
                case BallColor.green:
                    textureName += "green";
                    break;
                case BallColor.blue:
                    textureName += "blue";
                    break;
                case BallColor.light:
                    textureName += "light";
                    break;
                default:
                    return "";
            }
            return textureName + "-ball";
        }

        private Vector2 getMoveDirection(int endX, int endY)
        {
            float x = endX == _position.X ? 0 : endX > _position.X ? Constants.AnimationVelocity : 
                -Constants.AnimationVelocity; 
            float y = endY == _position.Y ? 0 : endY > _position.Y ? Constants.AnimationVelocity :
                -Constants.AnimationVelocity;
            
            return new Vector2(x, y);
        }

    }
}
