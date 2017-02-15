/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 14/02/2017
 * Time: 19:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Google.Protobuf;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Platform.Requests;
using POGOProtos.Networking.Platform.Responses;
using POGOProtos.Networking.Requests;
namespace PokemonGo.RocketAPI.Rpc
{
    public class GetStoreMessage : IMessage
    {
        public Google.Protobuf.Reflection.MessageDescriptor Descriptor{ get; }
            
        public void MergeFrom(CodedInputStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0) {
                switch (tag) {
                    default:
                        input.SkipLastField();
                        break;
                }
            }
        }
        

        public void WriteTo(CodedOutputStream output)
        {
        }

        public int CalculateSize()
        {
            return 0;
        }
    }
}


