#region Description
/* 		Kinect 2 Remote Controller
 * 		Handles messaging between the host and a Kinect 2 client
 * 		through an underlying TCP/IP socket connection
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Nov 2014
 *  @version: 1.0
 */
#endregion
#region Namespaces
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

using ProtoBuf;
using ProtoSerializer = ProtoBuf.Serializer;
#endregion
namespace Kinect2.Remote
{
	internal sealed class Kinect2Controlling : MonoBehaviour 
	{
		#region Properties
		internal int ID { get { return this.id; } }
		internal bool HasMessage { get { return this.messageQueue.Count > 0; } }
		internal Message Message
		{
			get { return this.messageQueue.Dequeue(); }
			set 
			{
				if(value.Action == MessageType.Ack)
				{
					this.timestamp = value.Elapsed.StartTime;
				}
				else
				{
					this.messageQueue.Enqueue(value); 
				}
			}
		}
		internal bool IsResponding { get; private set; }
		internal FrameSpan Elapsed { get; set; }
		#endregion
		#region Unity
		void Awake()
		{
			this.messageQueue  = new Queue<Message>();		
		}
		
		void Start () 
		{// Start receiving messages, Memorystream => gather fragmented messages
			this.buffer = new byte[BufferSize];
			this.memStream = new MemoryStream(this.buffer);
			this.netStream.BeginRead(this.buffer,0,this.buffer.Length,OnDataReceived,null);
			this.IsResponding = true;
			StartCoroutine("CheckAcknowledge");
		}
		
		void OnDestroy()
		{// Release Resources
			StopCoroutine("CheckAcknowledge");			
			this.netStream.Close();
			this.memStream.Close();
			this.client.Close();
		}
		
		void OnApplicationQuit()
		{
			if(this.client.Connected && this.IsConnected())
			{// Send Server Down Message => Clients move to reconnect state
				ProtoSerializer.SerializeWithLengthPrefix(this.netStream,new Message(
					this.id,MessageType.Bye,MessageParameter.None),LengthStyle);
			}
		}
		#endregion
		#region Helpers
		internal bool CreateWith(TcpClient client,int id)
		{// Init from server after connection
			this.id = id;
			this.client = client;
			this.client.Client.NoDelay = true;
			this.netStream = this.client.GetStream();
			return this.client.Connected && this.netStream.CanRead;
		}
		
		internal bool IsConnected()
		{
			try
			{
				return !(this.client.Client.Poll(1, SelectMode.SelectRead) && this.client.Client.Available == 0);
			} 
			catch (SocketException)
			{
				return false;
			}                
		}
		
		internal void Send(Message msg)
		{
			Console.Messaging.LogSend(msg);
			Serializer.SerializeWithLengthPrefix(this.netStream,msg,LengthStyle);			
		}
		
		private void OnDataReceived (IAsyncResult ar)
		{
			int bytes = 0;
			try
			{
				bytes = this.netStream.EndRead(ar);
				if (bytes > 0)
				{
					this.memStream.Position = 0;// rewind
					int count;
					ProtoSerializer.TryReadLengthPrefix(this.memStream, LengthStyle, out count);
					this.memStream.Position = 0;// rewind
					this.Message = ProtoSerializer.DeserializeWithLengthPrefix<Message>(this.memStream, ProtoBuf.PrefixStyle.Fixed32);			
					if (count >= (bytes - 4))	// Check for fragmented message
					{// OK => read next
						this.ar = this.netStream.BeginRead(this.buffer, 0, this.buffer.Length, OnDataReceived, this.netStream);
					}
					else
					{// Fragmented => read fragment
						this.ar = this.netStream.BeginRead(this.buffer, bytes, this.buffer.Length - bytes, OnFragmentedDataReceived, count);
					}
				}
			}
			catch (ProtoException)
			{// Fragmented message
				int length;
				this.memStream.Position = 0;// rewind
				if (ProtoSerializer.TryReadLengthPrefix(this.memStream, LengthStyle, out length)) 
				{// read fragment
					this.ar = this.netStream.BeginRead(this.buffer, bytes, this.buffer.Length - bytes, OnFragmentedDataReceived, length);
				}
			}
			catch (ObjectDisposedException)
			{// Connection somehow Closed			
				Console.Important("Object Disposed / Server Closed? , Error");		
			}    
		}
		
		private void OnFragmentedDataReceived(IAsyncResult ar)
		{
			int bytes = 0;
			try
			{
				bytes = this.netStream.EndRead(ar);
				if (bytes > 0)
				{
					var length = (int)ar.AsyncState;
					this.memStream.Position = 0;
					this.Message = Serializer.DeserializeWithLengthPrefix<Message>(this.memStream, LengthStyle);			
					this.ar = this.netStream.BeginRead(this.buffer, 0, this.buffer.Length, OnDataReceived, this.netStream);
				}
			}
			catch (ProtoException)
			{// Fragmented message
				int length;
				this.memStream.Position = 0;
				if (Serializer.TryReadLengthPrefix(this.memStream, LengthStyle, out length)) 
				{
					this.ar = this.netStream.BeginRead(this.buffer, bytes, this.buffer.Length - bytes, OnFragmentedDataReceived, length);
				}
			}
			catch (ObjectDisposedException ex)
			{// Connection somehow Closed			
				Console.Important("Object Disposed / Server Closed? , Error : " + ex.ToString());
			}    
		}
		
		private IEnumerator CheckAcknowledge()
		{
			long previous = 0;
			while(this.enabled)
			{
				yield return new WaitForSeconds(AckWaitSeconds);
				if(this.IsResponding = this.timestamp != previous)
				{
					previous = this.timestamp;
				}
			}
		}
		#endregion
		#region Fields
		private TcpClient client;
		private NetworkStream netStream;
		private MemoryStream memStream;
		private Queue<Message> messageQueue;
		private Message message;
		private IAsyncResult ar;
		private byte[] buffer;
		
		private long timestamp;
		private int id;
		
		private const int BufferSize = 1024;
		private const float AckWaitSeconds = 10.0f;// in seconds
		private const PrefixStyle  LengthStyle = PrefixStyle.Fixed32;	
		#endregion
	}
}
