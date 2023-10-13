namespace ConsoleApp1.Appointment
{
    [Serializable]
    public class AppointmentAlreadyHappenedException : Exception
    {
        public AppointmentAlreadyHappenedException() : base("Can not modify an appointment that already happened.") { }

        protected AppointmentAlreadyHappenedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
