using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.Entities
{
    class BombElement : BallElement
    {
        public BombElement(ElementColor color, Point position, Match3Game game) : base(color, position, game) { }

        public BombElement(BallElement ball) : base(ball) { }


        protected override void LoadContent()
        {
            string baseTextureName = getBaseTextureName() + "-bomb";
            string activeTextureName = baseTextureName + "-active";

            _baseTexture = Game.Content.Load<Texture2D>(baseTextureName);
            _activeTexture = Game.Content.Load<Texture2D>(activeTextureName);
        }
    }
}
