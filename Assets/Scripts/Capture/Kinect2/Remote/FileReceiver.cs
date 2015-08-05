#region Description
/* 		Receives transmitted files from an underlying
 * 		TCP/IP socket connection
 * 		Files are queued to the receiver and received sequentially (FIFO)
 * 		Used to centrally gather files from the clients at the host
 * 		
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Nov 2014
 * @version: 1.0
 */
#endregion
#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
#endregion
namespace Kinect2.Remote
{
	internal sealed class FileReceiver : FileTransfer, IDisposable
	{
		#region Constructors
		public FileReceiver (ConnectionInfo info)
			: base(info)
		{
			this.port = info.Port + 1 + info.ID;
			this.Init();
		}
		#endregion
		#region Properties
		
		#endregion
		#region Methods
		private void Init ()
		{// init server			
			this.server = new TcpListener(this.info.Address,this.port);
			this.server.Server.NoDelay = true;
			this.server.Start();	
		}
		
		private void EndReceive ()
		{
			Console.FileTransfer.OK(this.file.Name);
			//this.writer.Flush();		
			this.writer.Close();
			if(this.client.Connected)
			{
				this.stream.Close();
				this.client.Close();
			}
			this.Completed = true;		
			Console.FileTransfer.Next(this.files.Peek());		
		}
		#endregion
		#region FileTransfer
		internal override bool TryNext ()
		{// if more files queued => receive next
			bool hasNext = false;
			if(this.files.Count > 0)
			{
				hasNext = true;
				this.Completed = false;
				this.server.BeginAcceptTcpClient(OnClientConnected,null);
			}
			else
			{
				this.Completed = true;
			}
			return hasNext;
		}
		internal override void Abort () 
		{// and delete this batch
			var dir = this.files.Any() ? Path.GetDirectoryName(this.files.Peek()) : 
				(this.file != null && this.file.CanWrite) ? Path.GetDirectoryName(this.file.Name) : string.Empty;
			this.files.Clear();
			if(this.writer != null)
			{
				//this.writer.Flush();
				this.writer.Close();
			}
			if(this.stream != null)
			{
				this.stream.Close();
			}
			if(this.client != null && this.client.Connected)
			{
				this.client.Close();
			}
			if(!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
			{
				try
				{
					Directory.Delete(dir,true);					
				}
				catch(IOException e)
				{
					Console.FileTransfer.Error(" Deleting folder <b>" + dir + "</b> Info =\n" + e.ToString());
				}
			}
			//this.server.Stop();
			this.Completed = true;
		}
		#endregion
		#region SocketCallbacks
		private void OnClientConnected (IAsyncResult ar)
		{
			try
			{
				if(this.files.Count > 0)
				{
					this.file = new FileStream(this.files.Dequeue(),FileMode.Create,FileAccess.Write,FileShare.Read);
					this.writer = new BinaryWriter(this.file);
					this.client = this.server.EndAcceptTcpClient(ar);
					if(this.Completed)
					{
						return;
					}
					this.stream = this.client.GetStream();
					this.stream.BeginRead(this.buffer,0,BufferSize,OnDataReceived,null);
				}
			}
			catch(Exception e)
			{
				Console.FileTransfer.Error(e.ToString());
			}
		}
		
		private void OnDataReceived (IAsyncResult ar)
		{
			try
			{
				var bytes = this.stream.EndRead(ar);
				if(bytes > 0)
				{
					this.writer.Write(this.buffer,0,bytes);
					this.stream.BeginRead(this.buffer,0,BufferSize,OnDataReceived,null);
				}
				else
				{
					this.EndReceive();
				}
			}
			catch(SocketException e)
			{
				Console.FileTransfer.Error(e.ToString());		
				this.EndReceive();
			}
			catch(ObjectDisposedException ex)
			{
				Console.FileTransfer.Error(ex.ToString());
				this.EndReceive();
			}
		}
		#endregion
		#region IDisposable implementation
		public void Dispose ()
		{
			this.Abort();
			this.server.Stop();
		}
		#endregion
		#region Fields
		private TcpListener server;
		private FileStream file;
		private BinaryWriter writer;
		private TcpClient client;
		private NetworkStream stream;
		
		private int port;
		#endregion
	}
}
