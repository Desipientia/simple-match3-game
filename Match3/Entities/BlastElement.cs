using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.Entities
{
    class BlastElement : Match3GameElement
    {
        private float _scale;
        private float _scaleVelocity = 0.05f;

        public BlastElement(Vector2 position, Match3Game game) : base(game)
        {
            Position = position;
        }


        public override void Initialize()
        {
            _scale = 0.1f;
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }


        protected override void LoadContent()
        {
            _baseTexture = Game.Content.Load<Texture2D>("img/blast");

            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {
            if (_scale > 1f)
            {
                State = ElementState.Removed;
            }

            _scale += _scaleVelocity;

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            if (State == ElementState.Removed) { return; }

            Vector2 origin;

            origin.X = Constants.GameFieldCell * Constants.BlastWidth / 2;
            origin.Y = Constants.GameFieldCell * Constants.BlastHeight / 2;

            _spriteBatch.Begin();
            _spriteBatch.Draw(_baseTexture, Position, null, Color.White, 0f, origin, _scale, SpriteEffects.None, 0f);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
