using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    public class Crate : PhysicalObject, IEncodable, IRenderable
    {
        private Vector3 Position, dimensions; ///




        public Crate(int id, ObjectParameters parameters)
        {
            this.ID = id;
            foreach (String attribute in parameters.GetAttributes())
            {
                switch (attribute)
                {
                    case "X":
                        Position.X = (float)parameters.GetDoubleValue(attribute);
                        break;
                    case "Y":
                        Position.Y = (float)parameters.GetDoubleValue(attribute);
                        break;
                    case "Z":
                        Position.Z = (float)parameters.GetDoubleValue(attribute);
                        break;
                    case "Crate_Type":
                        Specialize(parameters.GetStringValue(attribute));
                        break;

                }
            }
        }

        private void Specialize(String attribute)
        {
            XmlHandler handler = new XmlHandler();
            handler.ChangeFile("static_objects.xml");
            handler.GetElement("VARIANT", "NAME", attribute);
            while (!handler.IsClosingTag("VARIANT"))
            {
                handler.GetNextElement();
                String name = handler.GetElementName();
                switch (name)
                {
                    case "DIMENSION":
                        HandleDimension(handler);
                        break;
                    case "MODEL":
                        HandleModel(handler);
                        break;

                }

            }

        }

        public override void InitializeDefault(int id)
        {
            ID = id;
            Position.X = 0;
            Position.Y = 0;
            Position.Z = 0;
            dimensions.X = 0;
            dimensions.Y = 0;
            dimensions.Z = 0;
        }

        public byte[] Encode()
        {
            Networking.Encoder tosend = new Networking.Encoder();

            tosend.AddElement("Position", Position);
            //tosend.AddElement("Orientation", Orientation);
            //tosend.AddElement("Velocity", Velocity);

            return tosend.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Networking.Encoder props = new Networking.Encoder(serialized);

            //Position = (Vector3)props.GetElement("Position");
            //Orientation = (Quaternion)props.GetElement("Orientation");
            //Velocity = (Vector3)props.GetElement("Velocity");
        }


        public override String getObjectType()
        {
            return "Crate";
        }

        private void HandleDimension(XmlHandler handler)
        {
            ObjectParameters parameters = handler.GetAttributes();
            foreach (String attribute in parameters.GetAttributes())
            {
                switch (attribute)
                {
                    case "WIDTH":
                        dimensions.X = (float)parameters.GetDoubleValue(attribute);
                        break;
                    case "HEIGHT":
                        dimensions.Y = (float)parameters.GetDoubleValue(attribute);
                        break;
                    case "LENGTH":
                        dimensions.X = (float)parameters.GetDoubleValue(attribute);
                        break;
                }
            }
        }

        private void HandleModel(XmlHandler handler)
        {
            String modelPath = handler.reader.ReadContentAsString();
            //TODO: Actually make this a model
        }







        #region IRenderable Members

        Vector3 IRenderable.Position
        {
            get { throw new NotImplementedException(); }
        }

        public Vector3 PositionOffset
        {
            get { throw new NotImplementedException(); }
        }

        public Quaternion Orientation
        {
            get { throw new NotImplementedException(); }
        }

        public Model Model3D
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
