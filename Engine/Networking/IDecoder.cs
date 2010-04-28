using System;
namespace Mammoth.Engine.Networking
{
    interface IDecoder
    {
        void AnalyzeObjects(string type, int id, byte[] properties);
    }
}
