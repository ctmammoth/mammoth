using System;
namespace Mammoth.Engine.Networking
{
    public interface IDecoder
    {
        void AnalyzeObjects(string type, int id, byte[] properties);
        Mammoth.Engine.Input.InputState DecodeInputState(byte[] data);
    }
}
