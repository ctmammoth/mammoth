using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Mammoth.Engine
{
    public class Crate : PhysicalObject, IEncodable, IRenderable
    {
        private int width, height, length, x, y, z;

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));
            r.DrawRenderable(this);
        }


        public Game Game
        {
            get;
            protected set;
        }

        public Crate(int id, ObjectParameters parameters, Game game)
            this.ID = id;
            this.Game = game;
        public Crate(int id, Game game)
        {
            // this.Game = game;
            InitializeDefault(id);
        }

                            dimensions.Z = (float) parameters.GetDoubleValue(attribute);
                String modelPath = handler.reader.ReadElementContentAsString();
                Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));
                this.Model3D = r.LoadModel(modelPath);
        {
        }

        public override string getObjectType()
        {
            throw new NotImplementedException();
        }
                get;
                set;
    }

            public override void collideWith(PhysicalObject obj)
            {
                throw new NotImplementedException();
            }
}
