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
				lock ( messages )
				{
					return ( string[] )messages.ToArray( typeof( string ) );
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
			lock ( messages )
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
			lock ( messages )
			{
				Message -= handler;
			}
		}

		public void Add( object oMessage )
		{
			// For XML serialization
			AddMessage( ( string )oMessage );
		}

		public void AddMessage( string message )
		{
			lock ( messages )
			{
				messages.Add( message );
				if ( Message != null )
					Message( message );
			}
		}

		private ArrayList messages=new ArrayList();

		public delegate void MessageHandler( string message );
		[field:NonSerialized]
		private event MessageHandler Message;
	}
}
