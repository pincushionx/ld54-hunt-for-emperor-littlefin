using Godot;
using Pincusion.LD54;
using System;
using System.Collections.Generic;

namespace Pincushion.LD54 {
	public partial class SoundManager : Node
	{
        //private Dictionary<string, AudioStreamPlayer> streams = new Dictionary<string, AudioStreamPlayer>();

        // Called when the node enters the scene tree for the first time.
        public override void _EnterTree()
        {
     		//GetNode<GameManager>("/root/GameManager").Sound = this;

			/*Godot.Collections.Array<Node> children = GetChildren();
			foreach(Node child in children)
			{
				if(child is AudioStreamPlayer)
				{
					AudioStreamPlayer stream = child as AudioStreamPlayer;

					stream.Bus = 
			

					streams.Add(child.Name, stream);

				}
			}*/
		}

		public void Play(string sound)
		{
			AudioStreamPlayer stream = GetNode<AudioStreamPlayer>(sound);
			if(!stream.Playing) {
				stream.Play();
			}
		}
	}
}