using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;

namespace PInvokeLibrary2
{
	/// <summary>
	/// Provides access to the device's registry.
	/// </summary>
	public class Registry
	{
		/// <summary>
		/// Creates a registry key.
		/// </summary>
		/// <param name="keyName">Name of the key to be created.</param>
		/// <returns>ERROR_SUCCESS if successful</returns>
		public static int CreateKey(string keyName)
		{
			UIntPtr hkey = UIntPtr.Zero;
			uint disposition = 0;

			try
			{
				return RegCreateKeyEx(HKCU, keyName, 0, null, 0,
					KeyAccess.None, IntPtr.Zero, ref hkey, ref disposition);
			}
			finally
			{
				if (UIntPtr.Zero != hkey)
				{
					RegCloseKey(hkey);
				}
			}
		}

		/// <summary>
		/// Deletes a registry key.
		/// </summary>
		/// <param name="keyName">Name of key</param>
		/// <returns>ERROR_SUCCESS if successful</returns>
		public static int DeleteKey(string keyName)
		{
			return RegDeleteKey(HKCU, keyName);
		}

		/// <summary>
		/// Create a string value in the specified registry key
		/// </summary>
		/// <param name="keyName">Name of key</param>
		/// <param name="valueName">Name of value</param>
		/// <param name="stringData">Value data</param>
		/// <returns>ERROR_SUCCESS if successful</returns>
		public static int CreateValueString(string keyName, string valueName, string stringData)
		{
			UIntPtr hkey = UIntPtr.Zero;

			try
			{
				int result = RegOpenKeyEx(HKCU, keyName, 0, KeyAccess.None, ref hkey);
				if (ERROR_SUCCESS != result)
					return result;
                    
				byte[] bytes = Encoding.Unicode.GetBytes(stringData);
                    
				return RegSetValueEx(hkey, valueName, 0, KeyType.String,
					bytes, (uint)bytes.Length);
			}
			finally
			{
				if (UIntPtr.Zero != hkey)
				{
					RegCloseKey(hkey);
				}
			}
		}

		/// <summary>
		/// Create a DWORD value in the specified registry key
		/// </summary>
		/// <param name="keyName">Name of key</param>
		/// <param name="valueName">Name of value</param>
		/// <param name="dwordData">Value data</param>
		/// <returns>ERROR_SUCCESS if successful</returns>
		public static int CreateValueDWORD(string keyName, string valueName, uint dwordData)
		{
			UIntPtr hkey = UIntPtr.Zero;

			try
			{
				int result = RegOpenKeyEx(HKCU, keyName, 0, KeyAccess.None, ref hkey);
				if (ERROR_SUCCESS != result)
					return result;

				byte[] bytes = BitConverter.GetBytes(dwordData);
				return RegSetValueEx(hkey, valueName, 0, KeyType.Dword,
					bytes, (uint)bytes.Length);
			}
			finally
			{
				if (UIntPtr.Zero != hkey)
				{
					RegCloseKey(hkey);
				}
			}
		}

        public static int GetStringValue(UIntPtr tree, string keyName, string valueName, ref string stringResult)
        {
            UIntPtr hkey = UIntPtr.Zero;

            try
            {
                int result = RegOpenKeyEx(tree, keyName, 0, KeyAccess.None, ref hkey);
                if (ERROR_SUCCESS != result)
                    return result;

                byte[] bytes = null;
                uint length = 0;
                KeyType keyType = KeyType.None;

                result = RegQueryValueEx(hkey, valueName, IntPtr.Zero, ref keyType,
                    null, ref length);

                if (ERROR_SUCCESS != result)
                    return result;

                keyType = KeyType.None;
                bytes = new byte[length];

                result = RegQueryValueEx(hkey, valueName, IntPtr.Zero, ref keyType,
                    bytes, ref length);

                if (ERROR_SUCCESS != result)
                    return result;

                stringResult = Encoding.Unicode.GetString(bytes, 0, bytes.Length-1);

                return ERROR_SUCCESS;
            }
            finally
            {
                if (UIntPtr.Zero != hkey)
                {
                    RegCloseKey(hkey);
                }
            }
        }
		/// <summary>
		/// Get the specified string value registry entry
		/// </summary>
		/// <param name="keyName">Key name</param>
		/// <param name="valueName">Value name</param>
		/// <param name="stringResult">string data</param>
		/// <returns>ERROR_SUCCESS if successful</returns>
		public static int GetStringValue(string keyName, string valueName, ref string stringResult)
		{
			UIntPtr hkey = UIntPtr.Zero;

			try
			{
				int result = RegOpenKeyEx(HKCU, keyName, 0, KeyAccess.None, ref hkey);
				if (ERROR_SUCCESS != result)
					return result;

				byte[] bytes = null;
				uint length = 0;
				KeyType keyType = KeyType.None;
                    
				result = RegQueryValueEx(hkey, valueName, IntPtr.Zero, ref keyType,
					null, ref length);

				if (ERROR_SUCCESS != result)
					return result;

				keyType = KeyType.None;
				bytes = new byte[length];

				result = RegQueryValueEx(hkey, valueName, IntPtr.Zero, ref keyType,
					bytes, ref length);

				if (ERROR_SUCCESS != result)
					return result;

				stringResult = Encoding.Unicode.GetString(bytes, 0, bytes.Length);

				return ERROR_SUCCESS;
			}
			finally
			{
				if (UIntPtr.Zero != hkey)
				{
					RegCloseKey(hkey);
				}
			}
		}

		/// <summary>
		/// Get the specified DWORD value registry entry.
		/// </summary>
		/// <param name="keyName">Key name</param>
		/// <param name="valueName">Value name</param>
		/// <param name="dwordResult">Value data</param>
		/// <returns>ERROR_SUCCESS if successful</returns>
		public static int GetDWORDValue(string keyName, string valueName, ref uint dwordResult)
		{
			UIntPtr hkey = UIntPtr.Zero;

			try
			{
				int result = RegOpenKeyEx(HKCU, keyName, 0, KeyAccess.None, ref hkey);
				if (ERROR_SUCCESS != result)
					return result;

				byte[] bytes = null;
				uint length = 0;
				KeyType keyType = KeyType.None;
                    
				result = RegQueryValueEx(hkey, valueName, IntPtr.Zero, ref keyType,
					null, ref length);

				bytes = new byte[Marshal.SizeOf(typeof(uint))];
				length = (uint)bytes.Length;
				keyType = KeyType.None;
                    
				result = RegQueryValueEx(hkey, valueName, IntPtr.Zero, ref keyType,
					bytes, ref length);

				if (ERROR_SUCCESS != result)
					return result;

				dwordResult = BitConverter.ToUInt32(bytes, 0);

				return ERROR_SUCCESS;
			}
			finally
			{
				if (UIntPtr.Zero != hkey)
				{
					RegCloseKey(hkey);
				}
			}
		}
  
		/// <summary>
		/// Delete the specified value form the registry key.
		/// </summary>
		/// <param name="keyName">Key name</param>
		/// <param name="valueName">Value name</param>
		/// <returns>ERROR_SUCCESS if successful</returns>
		public static int DeleteValue(string keyName, string valueName)
		{
			UIntPtr hkey = UIntPtr.Zero;

			try
			{
				int result = RegOpenKeyEx(HKCU, keyName, 0, KeyAccess.None, ref hkey);
				if (ERROR_SUCCESS != result)
					return result;

				return RegDeleteValue(hkey, valueName);
			}
			finally
			{
				if (UIntPtr.Zero != hkey)
				{
					RegCloseKey(hkey);
				}
			}
		}

		/// <summary>
		/// Key value types
		/// </summary>
		public enum KeyType : uint
		{
			None = 0,
			String = 1,
			Dword = 4,
		}

		/// <summary>
		/// Key access types
		/// </summary>
		public enum KeyAccess : uint
		{
			None = 0x0000,
			QueryValue = 0x0001,
			SetValue   = 0x0002,
			CreateSubKey = 0x0004,
			EnumerateSubKeys = 0x0008,
			Notify = 0x0010,
			CreateLink = 0x0020
		}

		/// <summary>
		/// HKEY_CLASSES_ROOT
		/// </summary>
		public static UIntPtr HKCR = new UIntPtr(0x80000000);
		/// <summary>
		/// HKEY_CURRENT_USER
		/// </summary>
		public static UIntPtr HKCU = new UIntPtr(0x80000001);
		/// <summary>
		/// HKEY_LOCAL_MACHINE
		/// </summary>
		public static UIntPtr HKLM = new UIntPtr(0x80000002);

		/// <summary>
		/// HKEY_USERS
		/// </summary>
		public static UIntPtr HKU = new UIntPtr(0x80000003);

		/// <summary>
		/// Successful return value from Registry API
		/// </summary>
		public const int ERROR_SUCCESS = 0;

		/// <summary>
		/// This function creates the specified key. If the key already exists in
		/// the registry, the function opens it. A remote application interface
		/// (RAPI) version of this function exists, and it is called
		/// CeRegCreateKeyEx.
		/// </summary>
		/// <param name="hkey">[in] Handle to a currently open key or one of:
		/// HKCR, HKCU, HKLM.</param>
		/// <param name="lpSubKey">[in] Pointer to a null-terminated string specifying
		/// the name of a subkey that this function opens or creates. The subkey
		/// specified must be a subkey of the key identified by the hKey parameter.
		/// This subkey must not begin with the backslash character (‘\’). This
		/// parameter cannot be NULL. In Windows CE, the maximum length of a key
		/// name is 255 characters, not including the terminating NULL character.
		/// You can also only nest 16 levels of sub-keys in Windows CE.</param>
		/// <param name="Reserved">[in] Reserved; set to 0.</param>
		/// <param name="lpClass">[in] Pointer to a null-terminated string that
		/// specifies the class (object type) of this key. This parameter is ignored
		/// if the key already exists. In Windows CE, the maximum length of a class
		/// string is 255 characters, not including the terminating NULL
		/// character.</param>
		/// <param name="dwOptions">[in] Ignored; set to 0 to ensure compatibility
		/// with future versions of Windows CE.</param>
		/// <param name="samDesired">[in] Ignored; set to 0 to ensure compatibility
		/// with future versions of Windows CE.</param>
		/// <param name="lpSecurityAttributes">[in] Set to NULL. Windows CE
		/// automatically assigns the key a default security descriptor.</param>
		/// <param name="phkResult">[out] Pointer to a variable that receives a
		/// handle to the opened or created key. When you no longer need the
		/// returned handle, call the RegCloseKey function to close it. </param>
		/// <param name="lpdwDisposition">out] Pointer to a variable that receives
		/// one of the following disposition values: REG_CREATED_NEW_KEY or
		/// REG_OPENED_EXISTING_KEY</param>
		/// <returns>ERROR_SUCCESS indicates success. A nonzero error code defined
		/// in Winerror.h indicates failure. To get a generic description of the error,
		/// call FormatMessage with the FORMAT_MESSAGE_FROM_SYSTEM flag set. The
		/// message resource is optional; therefore, if you call FormatMessage it
		/// could fail.</returns>
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern int RegCreateKeyEx
		(
			UIntPtr hkey,
			String lpSubKey,
			uint Reserved,
			StringBuilder lpClass,
			uint dwOptions,
			KeyAccess samDesired,              
			IntPtr lpSecurityAttributes,
			ref UIntPtr phkResult,
			ref uint lpdwDisposition
		);

		/// <summary>
		/// This function deletes a named subkey from the specified registry key. 
		/// A remote application interface (RAPI) version of this function exists,
		/// and it is called CeRegDeleteKey.
		/// </summary>
		/// <param name="hkey">[in] Handle to a currently open key or one of:
		/// HKCR, HKCU, HKLM.</param>
		/// <param name="subkeyName">[in] Pointer to a null-terminated string
		/// specifying the name of the key to delete. This parameter cannot
		/// be NULL.</param>
		/// <returns>ERROR_SUCCESS indicates success. A nonzero error code defined
		/// in Winerror.h indicates failure. To get a generic description of the
		/// error, call FormatMessage with the FORMAT_MESSAGE_FROM_SYSTEM flag set.
		/// The message resource is optional; therefore, if you call FormatMessage
		/// it could fail.</returns>
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern int RegDeleteKey
		(
			UIntPtr hkey,
			string subkeyName
		);

		/// <summary>
		/// This function opens the specified key.
		/// </summary>
		/// <param name="hkey">[in] Handle to a currently open key or one of:
		/// HKCR, HKCU, HKLM.</param>
		/// <param name="lpSubKey">[in] Pointer to a null-terminated string
		/// containing the name of the subkey to open. If this parameter is NULL
		/// or a pointer to an empty string, the function will open a new handle
		/// to the key identified by the hKey parameter. In this case, the function
		/// will not close the handles previously opened.</param>
		/// <param name="ulOptions">[in] Reserved; set to 0.</param>
		/// <param name="samDesired">[in] Not supported; set to 0.</param>
		/// <param name="phkResult">[out] Pointer to a variable that receives
		/// a handle to the opened key. When you no longer need the returned
		/// handle, call the RegCloseKey function to close it. </param>
		/// <returns>ERROR_SUCCESS indicates success. A nonzero error code defined
		/// in Winerror.h indicates failure. To get a generic description of the
		/// error, call FormatMessage with the FORMAT_MESSAGE_FROM_SYSTEM flag
		/// set. The message resource is optional; therefore, if you call
		/// FormatMessage it could fail.</returns>
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern int RegOpenKeyEx
		(
			UIntPtr hkey,
			String lpSubKey,
			uint ulOptions,
			KeyAccess samDesired,
			ref UIntPtr phkResult
		);

		/// <summary>
		/// This function retrieves the type and data for a specified value
		/// name associated with an open registry key.
		/// </summary>
		/// <param name="hkey">[in] Handle to a currently open key or one of:
		/// HKCR, HKCU, HKLM.</param>
		/// <param name="lpValueName">[in] Pointer to a string containing the
		/// name of the value to query. If this parameter is NULL or an empty
		/// string, the function retrieves the type and data for the key’s
		/// unnamed value. A registry key does not automatically have an unnamed
		/// or default value. Unnamed values can be of any type.</param>
		/// <param name="lpReserved">[in] Reserved; set to NULL.</param>
		/// <param name="lpType">[out] Pointer to a variable that receives the
		/// type of data associated with the specified value.</param>
		/// <param name="lpData">[out] Pointer to a buffer that receives the value’s
		/// data. This parameter can be NULL if the data is not required.</param>
		/// <param name="lpcbData">[in/out] Pointer to a variable that specifies the
		/// size, in bytes, of the buffer pointed to by the lpData parameter. When
		/// the function returns, this variable contains the size of the data copied
		/// to lpData. If the data has the REG_SZ, REG_MULTI_SZ or REG_EXPAND_SZ type,
		/// then lpcbData will also include the size of the terminating null character.
		/// The lpcbData parameter can be NULL only if lpData is NULL. 
		/// <returns>ERROR_SUCCESS indicates success. A nonzero error code defined
		/// in Winerror.h indicates failure. To get a generic description of the
		/// error, call FormatMessage with the FORMAT_MESSAGE_FROM_SYSTEM flag set.
		/// The message resource is optional; therefore, if you call FormatMessage
		/// it could fail.</returns>
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern int RegQueryValueEx
		(
			UIntPtr hkey,
			String lpValueName,
			IntPtr lpReserved,
			ref KeyType lpType,
			byte[] lpData,
			ref uint lpcbData
		);

		/// <summary>
		/// This function stores data in the value field of an open registry key.
		/// It can also set additional value and type information for the
		/// specified key.
		/// </summary>
		/// <param name="hkey">[in] Handle to a currently open key or one of:
		/// HKCR, HKCU, HKLM.</param>
		/// <param name="lpValueName">[in] Pointer to a string containing the
		/// name of the value to set. If a value with this name is not already
		/// present in the key, the function adds it to the key. If this parameter
		/// is NULL or an empty string, the function sets the type and data for the
		/// key’s unnamed value. Registry keys do not have default values, but they
		/// can have one unnamed value, which can be of any type. The maximum length
		/// of a value name is 255, not including the terminating NULL
		/// character. </param>
		/// <param name="Reserved">[in] Reserved; must be zero.</param>
		/// <param name="dwType">[in] Specifies the type of information to be stored
		/// as the value’s data.</param>
		/// <param name="lpData">[in] Pointer to a buffer containing the data to
		/// be stored with the specified value name.</param>
		/// <param name="cbData">[in] Specifies the size, in bytes, of the
		/// information pointed to by the lpData parameter. If the data is of
		/// type REG_SZ, REG_EXPAND_SZ, or REG_MULTI_SZ, cbData must include the
		/// size of the terminating null character. The maximum size of data allowed
		/// in Windows CE is 4 KB.</param>
		/// <returns>ERROR_SUCCESS indicates success. A nonzero error code defined
		/// in Winerror.h indicates failure. To get a generic description of the
		/// error, call FormatMessage with the FORMAT_MESSAGE_FROM_SYSTEM flag set.
		/// The message resource is optional; therefore, if you call FormatMessage
		/// it could fail.</returns>
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern int RegSetValueEx
		(
			UIntPtr hkey,
			String lpValueName,
			uint Reserved,
			KeyType dwType,
			byte[] lpData,
			uint cbData
		);

		/// <summary>
		/// This function removes a named value from the specified registry key.
		/// A remote application interface (RAPI) version of this function exists,
		/// and it is called CeRegDeleteValue. 
		/// </summary>
		/// <param name="hkey">[in] Handle to a currently open key or one of:
		/// HKCR, HKCU, HKLM.</param>
		/// <param name="valueName">[in] Pointer to a null-terminated string
		/// that names the value to remove. If this parameter is NULL or points
		/// to an empty string, the default value of the key is removed. A default
		/// value is create by calling RegSetValueEx with a NULL or empty string
		/// value name.</param>
		/// <returns>ERROR_SUCCESS indicates success. A nonzero error code defined
		/// in Winerror.h indicates failure. To get a generic description of the
		/// error, call FormatMessage with the FORMAT_MESSAGE_FROM_SYSTEM flag set.
		/// The message resource is optional; therefore, if you call FormatMessage
		/// it could fail.</returns>
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern int RegDeleteValue
		(
			UIntPtr hkey,
			string valueName
		);

		/// <summary>
		/// This function releases the handle of the specified key. A remote
		/// application interface (RAPI) version of this function exists, and
		/// it is called CeRegCloseKey.
		/// </summary>
		/// <param name="hkey">[in] Handle to the open key to close.</param>
		/// <returns>ERROR_SUCCESS indicates success. A nonzero error code
		/// defined in Winerror.h indicates failure. To get a generic description
		/// of the error, call FormatMessage with the FORMAT_MESSAGE_FROM_SYSTEM
		/// flag set. The message resource is optional; therefore, if you call
		/// FormatMessage it could fail.</returns>
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern int RegCloseKey
		(
			UIntPtr hkey
		);

		/// <summary>
		/// Run a test of the Registry class.
		/// </summary>
		/// <param name="showLine">Delegate called to show debug information</param>
	}
}
