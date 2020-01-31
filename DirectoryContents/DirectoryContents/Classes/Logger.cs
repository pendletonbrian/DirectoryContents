using System;
using System.Globalization;
using System.IO;

namespace DirectoryContents.Classes
{
    internal class Logger : IDisposable
    {
        #region Private Members

        /// <summary>
        /// The fully qualified path to the log file.
        /// </summary>
        private readonly string m_Filepath = string.Empty;

        private StreamWriter m_Writer = null;

        #endregion Private Members

        #region Public Properties

        /// <summary>
        /// Gets the filepath to where the log is written.
        /// </summary>
        public string Filepath
        {
            get { return m_Filepath; }

            private set { }
        }

        /// <summary>
        /// Gets/sets the format used for the timestamp.
        /// </summary>
        /// <example>
        /// The 29th of December, 2013 at 10:25 and 35 seconds in the morning
        /// would be "2013.29.12 10:25:35.715"
        /// </example>
        public string TimeFormatString { get; set; }

        #endregion Public Properties

        #region constructor

        public Logger(string fullyQualifiedPathToLogFile)
        {
            TimeFormatString = "yyyy.dd.MM HH:mm:ss.fff";

            m_Filepath = fullyQualifiedPathToLogFile;

            m_Writer = new StreamWriter(m_Filepath);

            WriteMessage("Logger constructor.", true, true);
        }

        #endregion constructor

        #region Private Methods

        /// <summary>
        /// Writes the message to the log file, prepending a timestamp and
        /// appending a new line.
        /// </summary>
        /// <param name="msg">
        /// The message to be written to the log file.
        /// </param>
        /// <param name="flush">
        /// Whether or not to flush the stream after writing the message.
        /// </param>
        /// <param name="prependTimeStamp">
        /// Whether or not to prepend a timestamp.
        /// </param>
        private void WriteMessage(string msg, bool flush, bool prependTimeStamp = true)
        {
            if (m_Writer != null)
            {
                if (string.IsNullOrWhiteSpace(msg))
                {
                    m_Writer.WriteLine();
                }
                else
                {
                    if (prependTimeStamp)
                    {
                        m_Writer.Write(DateTime.Now.ToString(TimeFormatString, CultureInfo.InvariantCulture) + " " + msg + Environment.NewLine);
                    }
                    else
                    {
                        m_Writer.Write(msg + Environment.NewLine);
                    }
                }

                if (flush)
                {
                    m_Writer.Flush();
                }
            }
        }

        #endregion Private Methods

        #region Public Methods

        public void Close()
        {
            if (m_Writer is null)
            {
                return;
            }

            m_Writer.Flush();
            m_Writer.Close();
            m_Writer.Dispose();
        }

        public void Dispose()
        {
            if (null != m_Writer)
            {
                m_Writer.Dispose();
                m_Writer = null;
            }
        }

        /// <summary>
        /// Clears all buffers for the current writer and causes any buffered
        /// data to be written to the underlying stream.
        /// </summary>
        public void Flush()
        {
            m_Writer?.Flush();
        }

        /// <summary>
        /// Writes the message to the log file, prepending a timestamp (if
        /// desired) and appending a new line. Calls flush after logging the message.
        /// </summary>
        /// <param name="msg">
        /// </param>
        /// <param name="prependTimeStamp">
        /// </param>
        public void Log(string msg, bool prependTimeStamp = true)
        {
            WriteMessage(msg, true, prependTimeStamp);
        }

        #endregion Public Methods
    }
}