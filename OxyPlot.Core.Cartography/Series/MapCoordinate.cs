using System.Diagnostics.CodeAnalysis;

namespace OxyPlot.Series
{
    /// <summary>
    /// Represents a point on a map, defined by its <see cref="Latitude"/>, <see cref="Longitude"/>
    /// and its optional <see cref="Altitude"/>.
    /// </summary>
    public struct MapCoordinate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapCoordinate"/> struct.
        /// <para>
        /// <see cref="Latitude"/> and <see cref="Longitude"/> will be set to <see cref="double.NaN"/>, <see cref="Altitude"/> to null.
        /// </para>
        /// </summary>
        public MapCoordinate()
        {
            Latitude = double.NaN;
            Longitude = double.NaN;
            Altitude = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapCoordinate"/> struct.
        /// <para>
        /// <see cref="Altitude"/> will be set to null.
        /// </para>
        /// </summary>
        /// <param name="latitude">The latitude (Y coordinate)</param>
        /// <param name="longitude">The longitude (X coordinate)</param>
        public MapCoordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapCoordinate"/> struct.
        /// </summary>
        /// <param name="latitude">The latitude (Y coordinate)</param>
        /// <param name="longitude">The longitude (X coordinate)</param>
        /// <param name="altitude">The altitude (Z coordinate)</param>
        public MapCoordinate(double latitude, double longitude, double altitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        /// <summary>
        /// Gets the latitude (Y coordinate).
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// Gets the longitude (X coordinate).
        /// </summary>
        public double Longitude { get; }

        /// <summary>
        /// Gets the altitude (Z coordinate).
        /// </summary>
        public double? Altitude { get; }

        /// <summary>
        /// Determines if the <see cref="Altitude"/> is defined.
        /// </summary>
        /// <returns><c>true</c> if this <see cref="Altitude"/> is defined; otherwise, <c>false</c>.</returns>
        public bool HasAltitude()
        {
            return Altitude.HasValue;
        }

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is MapCoordinate point)
            {
                return Latitude.Equals(point.Latitude) &&
                       Longitude.Equals(point.Longitude) &&
                       Altitude.Equals(point.Altitude);
            }

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Latitude, Longitude, Altitude);
        }

        /// <summary>
        /// <inheritdoc/>
        /// <para>
        /// Latitude, Longitude, (Altitude)
        /// </para>
        /// </summary>
        public override string ToString()
        {
            if (Altitude.HasValue)
            {
                return $"{Latitude}, {Longitude}, {Altitude}";
            }

            return $"{Latitude}, {Longitude}";
        }

        /// <inheritdoc/>
        public static bool operator ==(MapCoordinate left, MapCoordinate right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(MapCoordinate left, MapCoordinate right)
        {
            return !(left == right);
        }
    }
}
