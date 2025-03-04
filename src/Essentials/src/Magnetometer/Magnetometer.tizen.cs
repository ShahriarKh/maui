using Tizen.Sensor;
using TizenMagnetometer = Tizen.Sensor.Magnetometer;

namespace Microsoft.Maui.Essentials.Implementations
{
	public partial class MagnetometerImplementation : IMagnetometer
	{
		static TizenMagnetometer DefaultSensor =>
			(TizenMagnetometer)Platform.GetDefaultSensor(SensorType.Magnetometer);

		bool PlatformIsSupported =>
			TizenMagnetometer.IsSupported;

		TizenMagnetometer sensor;

		void PlatformStart(SensorSpeed sensorSpeed)
		{
			sensor = DefaultSensor;
			sensor.Interval = sensorSpeed.ToPlatform();
			sensor.DataUpdated += DataUpdated;
			sensor.Start();
		}

		void PlatformStop()
		{
			sensor.DataUpdated -= DataUpdated;
			sensor.Stop();
			sensor = null;
		}

		void DataUpdated(object sender, MagnetometerDataUpdatedEventArgs e)
		{
			RaiseReadingChanged(new MagnetometerData(e.X, e.Y, e.Z));
		}
	}
}
