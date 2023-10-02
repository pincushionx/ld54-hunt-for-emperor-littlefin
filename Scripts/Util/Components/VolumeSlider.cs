using Godot;
using System;

namespace Pincushion.Util.Components
{
	public partial class VolumeSlider : HSlider
	{
		[Export] public string BusName = null;
		private int _busIndex;

		public override void _Ready()
		{
			_busIndex = AudioServer.GetBusIndex(BusName);
			ValueChanged += OnValueChanged;

			Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_busIndex));
		}

        private void OnValueChanged(double value)
        {
            AudioServer.SetBusVolumeDb(_busIndex, (float) Mathf.LinearToDb(value));
        }
	}
}