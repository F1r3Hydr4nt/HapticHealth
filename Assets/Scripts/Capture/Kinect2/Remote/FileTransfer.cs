#region Description
/* 		File Transfer Base
 * 		Remote TCP/IP Socket Connection & Job Queue
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Nov 2014
 * @version: 1.0
 */
#endregion
#region Namespaces
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
#endregion
namespace Kinect2.Remote
{
	internal struct ConnectionInfo
	{
		internal ConnectionInfo(string addr,int port, int id)
		{
			this.Address = IPAddress.Parse(addr);
			this.Port = port;
			this.ID = id;
		}
		internal ConnectionInfo(IPAddress addr, int port, int id)
		{
			this.Address = addr;
			this.Port = port;
			this.ID = id;
		}
		internal readonly IPAddress Address;
		internal readonly int Port;
		internal readonly int ID;
	}
	
	internal abstract class FileTransfer
	{
		#region Constructors
		private FileTransfer()
		{
			this.Completed = true;
			this.files = new Queue<string>();
			this.buffer = new byte[BufferSize];
		}
		
		internal FileTransfer(ConnectionInfo info)
			: this()
		{
			this.info = info;
		}
		#endregion
		#region Properties
		internal int ID { get { return this.info.ID; } }
		public bool Completed { get; protected set; }
		public bool Pending { get { return this.files.Count > 0 || !this.Completed; } }
		#endregion
		#region Methods
		internal bool AddFiles(string[] filenames)
		{
			foreach (var filename in filenames)
			{
				this.files.Enqueue(filename);
			}
			this.Completed = false;
			return true;
		}
		
		internal bool AddFiles(string common, string[] suffixes)
		{
			foreach (var suffix in suffixes)
			{
				this.files.Enqueue(common + suffix);
			}
			this.Completed = false;
			return true;
		}
		
		internal abstract bool TryNext();
		internal abstract void Abort();
		#endregion
		#region Fields
		protected Queue<string> files;
		protected ConnectionInfo info;
		protected byte[] buffer;
		
		protected const int BufferSize = 1280;
		#endregion
	}
}
