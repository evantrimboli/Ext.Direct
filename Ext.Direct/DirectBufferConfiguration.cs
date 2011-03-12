using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ext.Direct
{
    /// <summary>
    /// This class is used to configure the buffering on the provider.
    /// </summary>
    public class DirectBufferConfiguration
    {

        /// <summary>
        /// Creates an instance of the object.
        /// </summary>
        /// <param name="buffer">Whether or not to buffer the provider.</param>
        /// <remarks>Created as a seperate class since Ext supports either an integer or a true/false value.</remarks>
        public DirectBufferConfiguration(bool buffer) : base()
        {
            this.BoolBuffer = buffer;
        }

        /// <summary>
        /// Creates an instance of the object.
        /// </summary>
        /// <param name="buffer">The amount of time to wait to buffer (in milliseconds).</param>
        public DirectBufferConfiguration(int buffer)
            : base()
        {
            this.IntBuffer = buffer;
        }

        /// <summary>
        /// Write the buffer value to the configuration.
        /// </summary>
        /// <param name="jw">The JsonTextWriter</param>
        internal void WriteBuffer(JsonTextWriter jw)
        {
            if (this.IntBuffer.HasValue)
            {
                Utility.WriteProperty<int>(jw, "enableBuffer", this.IntBuffer.Value);
            }
            else
            {
                Utility.WriteProperty<bool>(jw, "enableBuffer", this.BoolBuffer);
            }
        }

        /// <summary>
        /// The value for a boolean buffer
        /// </summary>
        internal bool BoolBuffer
        {
            get;
            private set;
        }

        /// <summary>
        /// The value for an integer buffer
        /// </summary>
        internal int? IntBuffer
        {
            get;
            private set;
        }

    }
}
