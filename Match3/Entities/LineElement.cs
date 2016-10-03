using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3
{
    class LineElement : BallElement
    {
        private BonusType _lineType;
        //private 

        public LineElement(BonusType type, BallColor color, Point position, Match3Game game) : base(color, position, game)
        {
            _lineType = type;
        }

        public LineElement(BallElement ball) : base(ball)
        {
            _lineType = ball.whatBonusNext;
        }


        public override void Initialize()
        {

            base.Initialize();
        }


        protected override void LoadContent()
        {
            string baseTextureName = getBaseTextureName() + "-line";
            string activeTextureName = baseTextureName + "-active";

            _baseTexture = Game.Content.Load<Texture2D>(baseTextureName);
            _activeTexture = Game.Content.Load<Texture2D>(activeTextureName);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

    }
}
