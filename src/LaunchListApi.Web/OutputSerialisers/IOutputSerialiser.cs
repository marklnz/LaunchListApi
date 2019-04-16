using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaunchListApi.Web.OutputSerialisers
{
    /// <summary>
    /// Interface describing the properties and methods required to be implemented by output serialiser classes.
    /// </summary>
    public interface IOutputSerialiser
    {
        /// <summary>
        /// The DTO format that the serialiser will write output to
        /// </summary>
        string MimeType { get; }

        /// <summary>
        /// When implemented in a child class, this method will accept the incoming object and serialise it to a given custom format.
        /// </summary>
        /// <param name="content">An <see cref="Object"/> that needs to be serialised.</param>
        /// <returns>An <see cref="Object"/> containing the serialised content</returns>
        /// <remarks>Each serialiser should be coded to accept a specific content type, and if the <paramref name="content"/> is not of that type, an <see cref="ArgumentException"/> should be thrown.</remarks>
        object Serialise(object content);

        /// <summary>
        /// Given a <see cref="Type"/>, indicate whether this serialiser is capable of serialising it.
        /// </summary>
        /// <param name="contentType">A <see cref="Type"/> to compare to the set of types serialisable by this class.</param>
        /// <returns>A <see cref="bool"/> value indicating whether the provided type can be serialised by this class.</returns>
        bool CanSerialise(Type contentType);
    }
}
