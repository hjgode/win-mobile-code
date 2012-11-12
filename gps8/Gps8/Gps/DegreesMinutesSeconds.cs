#region Using directives

using System;

#endregion

namespace Microsoft.WindowsMobile.Samples.Location
{
    /// <summary>
    /// class that represents a gps coordinate in degrees, minutes, and seconds.  
    /// </summary>
    public class DegreesMinutesSeconds
    {
        int degrees;
        /// <summary>
        /// The degrees unit of the coordinate
        /// </summary>
        public int Degrees
        {
            get { return degrees; }
        }

        int minutes;
        /// <summary>
        /// The minutes unit of the coordinate
        /// </summary>
        public int Minutes
        {
            get { return minutes; }
        }

        double seconds;
        /// <summary>
        /// The seconds unit of the coordinate
        /// </summary>
        public double Seconds
        {
            get { return seconds; }
        }

        /// <summary>
        /// Constructs a new instance of DegreesMinutesSeconds converting 
        /// from decimal degrees
        /// </summary>
        /// <param name="decimalDegrees">Initial value as decimal degrees</param>
        public DegreesMinutesSeconds(double decimalDegrees)
        {
            degrees = (int)decimalDegrees;
            
            double doubleMinutes = (Math.Abs(decimalDegrees) - Math.Abs((double)degrees)) * 60.0;

            minutes = (int)doubleMinutes;
            seconds = (doubleMinutes - (double)minutes) * 60.0;
        }

        /// <summary>
        /// Constructs a new instance of DegreesMinutesSeconds
        /// </summary>
        /// <param name="degrees">Degrees unit of the coordinate</param>
        /// <param name="minutes">Minutes unit of the coordinate</param>
        /// <param name="seconds">Seconds unit of the coordinate</param>
        public DegreesMinutesSeconds(int degrees, int minutes, double seconds)
        {
            this.degrees = degrees;
            this.minutes = minutes;
            this.seconds = seconds;
        }

        /// <summary>
        /// Converts the decimal, minutes, seconds coordinate to 
        /// decimal degrees
        /// </summary>
        /// <returns></returns>
        public double ToDecimalDegrees()
        {   // see http://peterfoot.net/PermaLink.aspx?guid=1d7952f3-3741-4233-bae7-b36a282db99b
            if (degrees == 0)
            {
                return 0.0;
            }

            int absDegrees = Math.Abs(degrees);

            double val = (double)absDegrees + ((double)minutes / 60.0) + ((double)seconds / 3600.0);

            return val * (absDegrees / degrees);

            //int absDegrees = Math.Abs(degrees);

            //double val = (double)absDegrees + ((double)minutes / 60.0) + ((double)seconds / 3600.0);
            //if ( degrees != 0) //avoid DivideByZero
            //    return val * (absDegrees / degrees);
            //else
            //    return 0;
        }

        /// <summary>
        /// Converts the instance to a string in format: D M' S"
        /// </summary>
        /// <returns>string representation of degrees, minutes, seconds</returns>
        public override string ToString()
        {
            return degrees + "d " + minutes + "' " + seconds + "\"";
        }
    }
}
