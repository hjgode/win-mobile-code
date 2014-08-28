//==========================================================================================
//
//		OpenNETCF.Runtime.InteropServices.FILETIME
//		Copyright (c) 2004, OpenNETCF.org
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
	/// This structure is a 64-bit value representing the number of 100-nanosecond intervals since January 1, 1601.
	/// <para><b>New in v1.1</b></para>
	/// </summary>
	public struct FILETIME
	{
		/// <summary>
		/// Specifies the low 32 bits of the FILETIME.
		/// </summary>
		public int dwLowDateTime;
		/// <summary>
		/// Specifies the high 32 bits of the FILETIME.
		/// </summary>
		public int dwHighDateTime;

		#region ToDateTime
		/// <summary>
		/// Returns the <see cref="T:System.DateTime"/> equivalent of this FILETIME value.
		/// </summary>
		/// <returns>A <see cref="T:System.DateTime"/> which equates this value.</returns>
		public DateTime ToDateTime()
		{
			return DateTime.FromFileTimeUtc(((int)dwHighDateTime) << 32 | (int)dwLowDateTime);
		}
		#endregion

	}
}
