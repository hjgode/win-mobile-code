//==========================================================================================
//
//		OpenNETCF.Runtime.InteropServices.CriticalHandle
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
	/// Represents a wrapper class for handle resources.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public abstract class CriticalHandle : IDisposable
	{
		private bool closed;
		private bool invalid;

		/// <summary>
		/// Initializes a new instance of the CriticalHandle class with the specified invalid handle value.
		/// </summary>
		/// <param name="invalidHandleValue">The value of an invalid handle (usually 0 or -1).</param>
		protected CriticalHandle(IntPtr invalidHandleValue)
		{
			this.invalid = false;
			handle = invalidHandleValue;
			this.closed = false;
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
			this.invalid = true;
			GC.SuppressFinalize(this);
		}

		#region IDisposable Members

		protected void Dispose(bool disposing)
		{
			if(!IsClosed && !IsInvalid)
			{
				ReleaseHandle();
			}
			GC.SuppressFinalize(this);
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
		~CriticalHandle()
		{
			Dispose(false);
		}
		#endregion
	}
}
