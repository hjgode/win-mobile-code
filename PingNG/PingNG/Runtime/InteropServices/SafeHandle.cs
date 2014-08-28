//==========================================================================================
//
//		OpenNETCF.Runtime.InteropServices.SafeHandle
//		Copyright (c) 2005, OpenNETCF.org
//
//		This library is free software; you can redistribute it and/or modify it under 
//		the terms of the OpenNETCF.org Shared Source License.
//
//		This library is distributed in the hope that it will be useful, but 
//		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
//		for more details.
//
//		You should have received a copy of the OpenNETCF.org Shared Source License 
//		along with this library; if not, email licensing@opennetcf.org to request a copy.
//
//		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
//		email licensing@opennetcf.org.
//
//		For general enquiries, email enquiries@opennetcf.org or visit our website at:
//		http://www.opennetcf.org
//
//==========================================================================================
using System;

namespace OpenNETCF.Runtime.InteropServices
{
	/// <summary>
	/// Represents a wrapper class for operating system handles.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public abstract class SafeHandle : IDisposable
	{
		private bool closed;
		private bool dirty;
		private bool ownsHandle;
		private IntPtr invalid;

		/// <summary>
		/// Initializes a new instance of the SafeHandle class with the specified invalid handle value.
		/// </summary>
		/// <param name="invalidHandleValue">The value of an invalid handle (usually 0 or -1).</param>
		/// <param name="ownsHandle">true to reliably let SafeHandle release the handle during the finalization phase; otherwise, false (not recommended).</param>
		protected SafeHandle(IntPtr invalidHandleValue, bool ownsHandle)
		{
			this.invalid = invalidHandleValue;
			handle = invalidHandleValue;
			this.closed = false;
			this.ownsHandle = ownsHandle;
			if(!ownsHandle)
			{
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Specifies the handle to be wrapped.
		/// </summary>
		protected IntPtr handle;

		/// <summary>
		/// Gets a value indicating whether the handle is closed.
		/// </summary>
		/// <value>true if the handle is closed; otherwise, false.</value>
		public bool IsClosed
		{
			get
			{
				return closed;
			}
		}

		/// <summary>
		/// When overridden in a derived class, gets a value indicating whether the handle value is invalid.
		/// </summary>
		/// <value>true if the handle is valid; otherwise, false.</value>
		public abstract bool IsInvalid
		{
			get;
		}

		/// <summary>
		/// Marks the handle for releasing and freeing resources.
		/// </summary>
		public void Close()
		{
			Dispose();
		}

		/// <summary>
		/// Returns the value of the <see cref="handle"/> field.
		/// </summary>
		/// <returns></returns>
		public IntPtr DangerousGetHandle()
		{
			return handle;
		}

		/// <summary>
		/// When overridden in a derived class, executes the code required to free the handle.
		/// </summary>
		/// <returns></returns>
		protected abstract bool ReleaseHandle();

		/// <summary>
		/// Sets the handle to the specified pre-existing handle.
		/// </summary>
		/// <param name="handle"></param>
		protected void SetHandle(IntPtr handle)
		{
			this.handle = handle;
		}

		/// <summary>
		/// Marks a handle as invalid.
		/// </summary>
		public void SetHandleAsInvalid()
		{
			dirty = true;
			GC.SuppressFinalize(this);
		}

		#region IDisposable Members

		protected void Dispose(bool disposing)
		{
			if(ownsHandle && !IsClosed && !dirty && !IsInvalid)
			{
				ReleaseHandle();
			}

			if(disposing)
			{
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Marks the handle for releasing and freeing resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			
		}

		/// <summary>
		/// Frees all resources associated with the handle.
		/// </summary>
		~SafeHandle()
		{
			Dispose(false);
		}
		#endregion
	}
}
