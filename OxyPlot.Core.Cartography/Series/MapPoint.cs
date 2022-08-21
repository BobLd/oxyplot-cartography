using System.Diagnostics.CodeAnalysis;

namespace OxyPlot.Series
{
    /// <summary>
    /// Represents a point on point on a map, defined by its <see cref="Latitude"/>, <see cref="Longitude"/>
    /// and its optional <see cref="Altitude"/>.
    /// </summary>
    public struct MapPoint
    {
        /// <summary>
        /// Gets the latitude (Y coordinate).
        /// </summary>
        public double Latitude { get; init; }

        /// <summary>
        /// Gets the longitude (X coordinate).
        /// </summary>
        public double Longitude { get; init; }

        /// <summary>
        /// Gets the altitude (Z coordinate).
        /// </summary>
        public double? Altitude { get; init; }

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is MapPoint point)
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
            if (!Altitude.HasValue)
            {
                return $"{Latitude}, {Longitude}";
            }

            return $"{Latitude}, {Longitude}, {Altitude}";
        }

        /// <inheritdoc/>
        public static bool operator ==(MapPoint left, MapPoint right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(MapPoint left, MapPoint right)
        {
            return !(left == right);
        }
    }
}
