using System;
using System.Collections;
using System.Xml.Serialization;

namespace NProf.Glue.Profiler.Project
{
	/// <summary>
	/// Summary description for RunMessageCollection.
	/// </summary>
	[Serializable]
	public class RunMessageCollection : IEnumerable
	{
		/// <summary>
		/// Create a new run message collection.
		/// </summary>
		public RunMessageCollection()
		{
			_alMessages = new ArrayList();
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator</returns>
		public IEnumerator GetEnumerator()
		{
			// Enumerate the current state of the messages
			return AllMessages.GetEnumerator();
		}

		/// <summary>
		/// Returns all of the messages within the collection.
		/// </summary>
		public string[] AllMessages
		{
			get
			{
				lock ( _alMessages )
				{
					return ( string[] )_alMessages.ToArray( typeof( string ) );
				}
			}
		}

		/// <summary>
		/// Start listening to run messages, returning the current list of messages.
		/// </summary>
		/// <param name="handler">The <see cref="MessageHandler" /> called when a message is added</param>
		/// <returns>The current list of messages</returns>
		public string[] StartListening( MessageHandler handler )
		{
			lock ( _alMessages )
			{
				Message += handler;
				return AllMessages;
			}
		}

		/// <summary>
		/// Stops listening for messages.
		/// </summary>
		/// <param name="handler">The <see cref="MessageHandler" /> added earlier</param>
		public void StopListening( MessageHandler handler )
		{
			lock ( _alMessages )
			{
				Message -= handler;
			}
		}

		public void Add( object oMessage )
		{
			// For XML serialization
			AddMessage( ( string )oMessage );
		}

		public void AddMessage( string strMessage )
		{
			lock ( _alMessages )
			{
				_alMessages.Add( strMessage );
				if ( Message != null )
					Message( strMessage );
			}
		}

		private ArrayList _alMessages;

		public delegate void MessageHandler( string strMessage );
		[field:NonSerialized]
		private event MessageHandler Message;
	}
}
