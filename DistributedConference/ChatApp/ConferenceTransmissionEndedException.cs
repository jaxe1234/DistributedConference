using System;
using System.Runtime.Serialization;

namespace ChatApp
{
    public class ConferenceTransmissionEndedException : Exception
    {
        public ConferenceTransmissionEndedException() : this ("Unspecified transmission end happened. Sorry we couldn't tell you more.")
        {
        }

        public ConferenceTransmissionEndedException(string reason) : base(reason)
        {
        }

        public ConferenceTransmissionEndedException(string message, Exception innerException) : base(message, innerException)
        {
        }
        
    }
}