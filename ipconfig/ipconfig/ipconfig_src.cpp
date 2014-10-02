//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
//
// Use of this source code is subject to the terms of the Microsoft end-user
// license agreement (EULA) under which you licensed this SOFTWARE PRODUCT.
// If you did not accept the terms of the EULA, you are not authorized to use
// this source code. For a copy of the EULA, please see the LICENSE.RTF on your
// install media.
//
//====================================================================
//           
//                            ipconfig
//
//               Displays the configuration Information
//
//
//====================================================================
#include "stdafx.h"

#include "coutClass.h"

#include "ipconfig_src.h"

#include<stdio.h>
#include<stdlib.h>
#include <tchar.h>
#include <windows.h>
#include <winsock2.h>
#pragma comment (lib, "ws2.lib")
#include <ws2tcpip.h>
#include <iptypes.h>
#include "iphlpapi.h"
#pragma comment (lib, "iphlpapi.lib")
#include <stdarg.h>
#include <memory.h>
#include "ntddip6.h"

	COutput cout;

	/*
// Function Declarations
void  usage();
int GetConfigData();
void ReleaseAddress(DWORD);
void RenewAddress(DWORD);
void FlushDNSCache(void);

void setOutputWindow(HWND hWndEdit);
*/

// Global Variables
//int Verbose=FALSE;
BOOL bflagall=FALSE;    // This is TRUE if the /all option is specified at the command prompt  

// month[] and dayofweek[] are arrays meant to convert the month and 
//dayofweek indices to month and day names respectively
TCHAR *month[]={NULL, _T("January"),_T("February"),_T("March"),_T("April"),_T("May"),_T("June"),_T("July"),_T("August"),_T("September"),_T("October"),_T("November"),_T("December")};
TCHAR *dayofweek[]={_T("Sunday"),_T("Monday"),_T("Tuesday"),_T("Wednesday"),_T("Thursday"),_T("Friday"),_T("Saturday")};


typedef int (__cdecl *PFN_tprintf)(const TCHAR*, ...);
PFN_tprintf  v_pfn_tprintf;
HMODULE v_hCoreDLL;
BOOL v_fDebugOut=FALSE;


//
//  Currently defined privately only.
//

#define	IF_TYPE_OTHER	  	1
#define	IF_TYPE_ETHERNET	6
#define	IF_TYPE_TOKENRING	9
#define	IF_TYPE_FDDI		15
#define	IF_TYPE_PPP		23
#define	IF_TYPE_LOOPBACK	24
#define	IF_TYPE_SLIP		28

void setOutputWindow(HWND hwndEdit){
	cout.SetOutputWindow(hwndEdit);
}

//==============================================================================

// Name        : OutputMessage
// Description : Outputs to the console or the debug output port
// Parameters  : Variable Number
// Returns     : integer 

//================================================================================

int OutputMessage (TCHAR *pFormat, ...) {
   va_list ArgList;
   TCHAR Buffer[256];
   int      RetVal;

   va_start (ArgList, pFormat);
   RetVal = wvsprintf (Buffer, pFormat, ArgList);
   if (!v_fDebugOut) {
      if (v_pfn_tprintf == NULL) {
         // Since not all configs contain the wprintf function we'll
         // try to find it.  If it's not there we'll default to using
         // OutputDebugString.
         v_hCoreDLL = LoadLibrary(TEXT("coredll.dll"));
         if (v_hCoreDLL) {
#ifdef UNICODE
            v_pfn_tprintf = (PFN_tprintf)GetProcAddress(v_hCoreDLL, TEXT("wprintf"));
#else 
            v_pfn_tprintf = (PFN_tprintf)GetProcAddress(v_hCoreDLL, TEXT("printf"));
#endif
         }

      }
      if (v_pfn_tprintf != NULL) {
         (v_pfn_tprintf) (TEXT("%s"), Buffer);
      }
      else {
         // Couldn't find the entry point, revert to OutputDebugString()
         v_fDebugOut = TRUE;
      }
   }
   if (v_fDebugOut) {
      OutputDebugString(Buffer);
   }
   
   cout.print(L"%s", Buffer);
   cout.refresh();

   return RetVal; 
}



//==============================================================
//
//Name        : TimeToFileTime 
//Description : Converts time in the structure time_t to FILETIME format 
// Parameters : time_t  t
//              LPFILETIME pft
//Returns     : void
//
//==========================================================


void TimeToFileTime( time_t t, LPFILETIME pft ) {
   unsigned __int64 ui64 = (unsigned __int64)t * 10000000 + 116444736000000000;
   *pft = *(FILETIME *)&ui64;
   return;
}



//===============================================================================

//      Name        : DisplayErrorMessage
//      Description : It displays the error message resulting from an unsuccessful attempt to use an IPhelper API
//      Parameters  :
//                    int errorcode  The return value of that API
//
//      Returns     : void
//
//===================================================================================
void DisplayErrorMessage(int errorcode) {

   LPVOID lpMsgBuf = NULL;

   FormatMessage(
                FORMAT_MESSAGE_ALLOCATE_BUFFER |
                FORMAT_MESSAGE_FROM_SYSTEM | 
                FORMAT_MESSAGE_IGNORE_INSERTS,
                NULL,
                errorcode,   //GetLastError(),
                MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
                (LPTSTR) &lpMsgBuf,
                0,
                NULL 
                );

   if (lpMsgBuf) {
      OutputMessage(TEXT("%ls"),lpMsgBuf);
      LocalFree( lpMsgBuf );
   }
   return; 
}

// ===================================================================================

// Name        : IsAutoConfigEnabled
// Description : Determines whether Autoconfig is enabled for a particular adapter
//               Since this information is not returned by the 2 main APIs used to
//               get the other information,namely, GetAdaptersInfo and GetNetworkParams,
//               this function calls another API, GetPerAdapterInfo
// Parameters :  ULONG index   The index of the Adapter that has to be tested for Autoconfig Enabled
// Returns:  BOOL 
//           True : AutoConfig is enbled
//           False: AutoConfig is NOT enbled

//======================================================================================    
BOOL IsAutoConfigEnabled( ULONG Index)

{
   PIP_PER_ADAPTER_INFO ptr=NULL;
   ULONG size=0;
   DWORD result;
   BOOL ReturnValue;


   result=GetPerAdapterInfo( Index,ptr, &size);
   if (result==ERROR_BUFFER_OVERFLOW) {
      if (!(ptr=(PIP_PER_ADAPTER_INFO)malloc(size))) {
         OutputMessage(TEXT("Insufficient Memory\r\n"));
         exit(1);
      }
      result=GetPerAdapterInfo(Index,ptr,&size);
      if (result!=ERROR_SUCCESS) {
         DisplayErrorMessage(result);
         exit(1);
      }
   }
   else if (result!=ERROR_SUCCESS) {
      DisplayErrorMessage(result);
      exit(1);
   }

   ReturnValue = (ptr && ptr->AutoconfigEnabled ) ? TRUE : FALSE;
   if (ptr)
      free(ptr);
   return(ReturnValue);   

}

//=============================================================================================
//
// Name           :  _tmain
// Decription     :  Entry Point for the program
// Parameters     :
//                   argc : The number of commandline arguments
//                   argv : Tha array containing the commandline arguments( as Unicode strings)
// Retuns         :  int

//============================================================================================= 

int _tmain1(int argc, TCHAR **argv ) {


   DWORD Address =0; 
   BOOL NextParamIsAddress=FALSE, Error=FALSE, Renew=FALSE, Release=FALSE, FlushDNS=FALSE;

   for (int i=1;i<argc;i++) {

      if ((argv[i][0]==_T('-')) || (argv[i][0] ==_T('/'))) {
         if (NextParamIsAddress) {
            Error=TRUE;
         }
         switch (toupper(argv[i][1])) {
            case _T('D') :
                if (_tcsicmp(&(argv[i][1]),_T("D"))!=0)
                  Error=TRUE;
               else
                  v_fDebugOut=TRUE;
               break;

            case _T('?') :
               usage();
               exit(1);
               break; 

            case _T('A'):
               if (_tcsicmp(&(argv[i][1]), _T("ALL"))==0)
                  bflagall =TRUE;
               break;

            case _T('R'):
               if ( _tcsicmp(&(argv[i][1]),_T("RENEW"))==0) {
                  Renew=TRUE;
                  NextParamIsAddress=TRUE;
               }
               else if (_tcsicmp(&(argv[i][1]),_T("RELEASE"))==0) {
                  Release =TRUE;
                  NextParamIsAddress=TRUE; 
               }
               break;

            case _T('F'):
               if ( _tcsicmp(&(argv[i][1]),_T("FLUSHDNS"))==0) {
                  FlushDNS = TRUE;
               }
               break;

            default:
               Error=TRUE;
         }
      }
      else if (NextParamIsAddress) {
         if (!(Address=_ttoi(argv[i]))) {
            Error=TRUE;
         }
         else
            break;  
      }
      else {
         Error=TRUE; 
      }

   }//End of for
   if ((Renew && Release) || (Error==TRUE)) {
      OutputMessage(TEXT("Incorrect Parameters\r\n"));
      exit(1);
   }

   WSADATA WsaData;
   if (WSAStartup(MAKEWORD(1,1), &WsaData) != 0)
   {
      OutputMessage(TEXT("WSAStartup failed (error %ld)\r\n"), GetLastError());
      return 0;
   }

   if (Release)
      ReleaseAddress(Address);
   else if (Renew)
      RenewAddress(Address);
   else if (FlushDNS) {
      FlushDNSCache();
   } else {
      GetConfigData();
   }

   WSACleanup();

   return(0);
}




// ==========================================================================
// Name        :  usage
// Description :  display usage info and quit
// Parameters  :  None
// Returns     :  Nothing

//==========================================================================

void  usage() {
   OutputMessage(TEXT("usage : ipconfig [/? | /all | /d |/renew [adapter index] | /release [adapter index] ] /flushdns\r\n")
                 TEXT("\r\n")
                 TEXT("    /d    Display redirected to the Debug Output Port \r\n"));

   OutputMessage( TEXT("    /?    Display this help message. \r\n")
                  TEXT("    /all  Display full configuration information.\r\n"));
   OutputMessage(TEXT("    /release   Release the IP address for the specified adapter \r\n"));
   OutputMessage(TEXT("    /renew     Renew the IP address for the specified adapter \r\n") 
                 TEXT("    /flushdns  Clear the name resolution client cache \r\n")
                 TEXT("\r\n")
                 TEXT(" The default is to display only the IP address, subnet mask and default gateway ")
                 TEXT(" for each adapter bound to TCP/IP. \r\n"));

   return;
}      

LPTSTR
FormatNICAddrString(
    BYTE * Address,
    DWORD  AddressLength
    )
{
    LPTSTR pszRet;
    LPTSTR pszTmp;
    BOOL   bFirstEntry = TRUE;

    pszTmp = pszRet = (LPTSTR)LocalAlloc(LPTR, AddressLength * 3 * sizeof(TCHAR) + sizeof(TCHAR));
    if (NULL == pszRet) {
        return NULL;
    }

    for (unsigned int uindex = 0; uindex < AddressLength; uindex++) {
        pszTmp += wsprintf(pszTmp, bFirstEntry ? TEXT("%02x"):TEXT(" %02x"), Address[uindex]);
        bFirstEntry=FALSE;
    }

    return pszRet;
}


///////////////////////////////////////////////////////////////////////////////
//	DisplayV6Address()
//
//	Routine Description:
//
//		Display the IPv6 address.
//      
//	Arguments:
//		
//		pAddr   :: points to the SOCKADDR_IN6 structure.
//      pcTitle :: If non null print this before the IPv6 address.
//
//	Return Value:
//
//		None.
//

void
DisplayV6Address(PSOCKADDR_IN6 pAddr, PTCHAR ptcTitle, PTCHAR ptcAddressType)
{  
    #define MAX_BUFFER_LEN      128

    TCHAR   ptcBuffer[MAX_BUFFER_LEN];
    DWORD   dwBufferLen;
    
    dwBufferLen = MAX_BUFFER_LEN;

    if (WSAAddressToString(
            (struct sockaddr *) pAddr,
            sizeof(SOCKADDR_IN6),
            NULL,
            ptcBuffer,
            &dwBufferLen))
    {
        wcscpy(ptcBuffer, TEXT("<invalid>"));
    }
    else
    {
        if (ptcAddressType)
        {
            wcscat(ptcBuffer, TEXT(" "));
            wcscat(ptcBuffer, ptcAddressType);
        }
    }

    if (ptcTitle)
    {
        OutputMessage(TEXT("\t %s %s\r\n"),
            ptcTitle,
            ptcBuffer);
    }
    else
    {       
        OutputMessage(TEXT("\t                       %s\r\n"),
            ptcBuffer);
    }
        


}   //  DisplayV6Address()



///////////////////////////////////////////////////////////////////////////////
//	PrintIPv6Addresses()
//
//	Routine Description:
//
//		Print the V6 addresses correcponds to the given IP_ADAPTER_INFO.
//      
//	Arguments:
//		
//		pAdapterInfo        ::  [optional] should be from GetAdapterInfo()
//      pAdapterAddresses   ::  should be from GetAdapterAddresses().
//      PrintType           ::  The V6 address type to be printed.
//      bNoLabel            ::  Don't print label.
//
//	Return Value:
//
//		None.
//

typedef enum 
{
    PRINT_UNICAST_ADDRESS = 0,
    PRINT_ANYCAST_ADDRESS,
    PRINT_MULTICAST_ADDRESS,
    PRINT_DNSSERVER_ADDRESS,
    PRINT_ROUTER_ADDRESS
    
} PRINT_TYPE;

void
PrintIpv6Addresses(
    PIP_ADAPTER_INFO      pAdapterInfo,  
    PIP_ADAPTER_ADDRESSES pAdapterAddresses,
    PRINT_TYPE            PrintType,
    BOOL                  bNoLabel)
{
    PIP_ADAPTER_ADDRESSES           pAA;
    PIP_ADAPTER_UNICAST_ADDRESS     pUnicastAddr;
    PIP_ADAPTER_DNS_SERVER_ADDRESS  pDnsServerAddr;
    BOOL                            bFirstAddress;

    //
    //  Find matching entry in IPv6 interface list.
    //

    if (pAdapterInfo)
    {
        for (pAA = pAdapterAddresses ; pAA ; pAA = pAA->Next)
        {
            if (pAA->IfIndex == pAdapterInfo->Index)
                break;
        }
    }
    else
        pAA = pAdapterAddresses;


    if (pAA == NULL || pAA->Ipv6IfIndex == 0)
        return;

    //
    //  Print the Unicast Addresses..
    //

    bNoLabel ? bFirstAddress = FALSE: bFirstAddress = TRUE;

    if (!(pAA->OperStatus & IfOperStatusUp))
    {
        //
        //  Iterface is most likely in media status disconnected.
        //  IPv6 stack does not remove the addresses to keep idle connections
        //  alive.
        //  We'll skip this i/f.
        //

        return;
    }

    switch(PrintType)
    {
        ///////////////////////////////////////////////////////////////////////
        
        case PRINT_UNICAST_ADDRESS:            
            for (pUnicastAddr = pAA->FirstUnicastAddress; 
                 pUnicastAddr ; 
                 pUnicastAddr = pUnicastAddr->Next) 
            {
                if (pUnicastAddr->Address.lpSockaddr->sa_family == AF_INET6)
                {        
                    //
                    //  Following ipv6.exe, wellknown is not printed.
                    //
                    
                    DisplayV6Address(
                        (PSOCKADDR_IN6)pUnicastAddr->Address.lpSockaddr, 
                        bFirstAddress ? L"IP Address ........ :" : NULL,                        
                        (pUnicastAddr->PrefixOrigin == IpPrefixOriginManual &&
                         pUnicastAddr->SuffixOrigin == IpSuffixOriginManual) ?
                            TEXT("[Manual]"):                
                        (pUnicastAddr->PrefixOrigin == IpPrefixOriginRouterAdvertisement &&
                         pUnicastAddr->SuffixOrigin == IpSuffixOriginLinkLayerAddress) ?
                            TEXT("[Public]"):                
                        (pUnicastAddr->PrefixOrigin == IpPrefixOriginRouterAdvertisement &&
                         pUnicastAddr->SuffixOrigin == IpSuffixOriginRandom) ?
                            TEXT("[Temporary]"):             
                        (pUnicastAddr->PrefixOrigin == IpPrefixOriginDhcp &&
                         pUnicastAddr->SuffixOrigin == IpSuffixOriginDhcp) ?
                            TEXT("[DHCP]"): 
                            NULL);

                    bFirstAddress = FALSE;
                }
            }
            break;

        ///////////////////////////////////////////////////////////////////////            
            
        case PRINT_DNSSERVER_ADDRESS:
            for (pDnsServerAddr = pAA->FirstDnsServerAddress;
                 pDnsServerAddr ; 
                 pDnsServerAddr = pDnsServerAddr->Next) 
            {
                if (pDnsServerAddr->Address.lpSockaddr->sa_family == AF_INET6)
                {        
                    DisplayV6Address(
                        (PSOCKADDR_IN6)pDnsServerAddr->Address.lpSockaddr, 
                        bFirstAddress ? L"DNS Servers ......  :" : NULL,
                        NULL);

                    bFirstAddress = FALSE;
                }
            }
            
            break;

        ///////////////////////////////////////////////////////////////////////

        case PRINT_ROUTER_ADDRESS:
        {
            #define IP6_DEVICE_NAME			TEXT("IP60:")
            
            HANDLE                  hIPv6;
            IPV6_QUERY_ROUTE_TABLE  QueryRouteTable, NextQueryRouteTable;
            IPV6_INFO_ROUTE_TABLE   RTE;
            ULONG                   ulBytesReturned;
            SOCKADDR_IN6            Addr;
                

            hIPv6 = CreateFileW(
                        IP6_DEVICE_NAME,
                        GENERIC_WRITE,                      // access mode
                        FILE_SHARE_READ | FILE_SHARE_WRITE,
                        NULL,                               // security attributes
                        OPEN_EXISTING,
                        0,                                  // flags & attributes
                        NULL);                              // template file

            NextQueryRouteTable.Neighbor.IF.Index = 0;

            for (;;) 
            {
                QueryRouteTable = NextQueryRouteTable;

                if (!DeviceIoControl(
                        hIPv6, 
                        IOCTL_IPV6_QUERY_ROUTE_TABLE,
                        &QueryRouteTable, 
                        sizeof(QueryRouteTable),
                        &RTE, 
                        sizeof(RTE), 
                        &ulBytesReturned,
                        NULL)) 
                {
                    
                }

                NextQueryRouteTable = RTE.Next;

#if 0                
                OutputMessage(TEXT("Prefix Length = [%d] --- Index = [%d]\r\n"),
                    RTE.This.PrefixLength,
                    RTE.This.Neighbor.IF.Index);
#endif
                
                if (QueryRouteTable.Neighbor.IF.Index != 0) 
                {
                    RTE.This = QueryRouteTable;

                    if ((RTE.This.PrefixLength == 0) &&
                       (pAA->Ipv6IfIndex == RTE.This.Neighbor.IF.Index))  
                    {                    
                        //
                        //  We have valid router, display it.
                        //

                        Addr.sin6_family   = AF_INET6;
                        Addr.sin6_addr     = RTE.This.Neighbor.Address;
                        Addr.sin6_port     = 0;

                        if (IN6_IS_ADDR_LINKLOCAL(&(Addr.sin6_addr)))
                        {
                            Addr.sin6_scope_id = pAA->ZoneIndices[ScopeLevelLink];
                        }
                        else                             
                        if (IN6_IS_ADDR_SITELOCAL(&(Addr.sin6_addr))) 
                        {
                            Addr.sin6_scope_id = pAA->ZoneIndices[ScopeLevelSite];
                        }
                        else
                        {
                            Addr.sin6_scope_id = 0;
                        }

                        DisplayV6Address(
                            (PSOCKADDR_IN6)&Addr,
                            bFirstAddress ? L"Default Gateway ... :" : NULL,
                            NULL);

                        bFirstAddress = FALSE;
                        
                    }
                    
                }

                if (NextQueryRouteTable.Neighbor.IF.Index == 0)
                    break;
                    
            }

            CloseHandle(hIPv6);
            break;
        }
            
        ///////////////////////////////////////////////////////////////////////
        
        case PRINT_ANYCAST_ADDRESS:
        case PRINT_MULTICAST_ADDRESS:
        default:
            pUnicastAddr = NULL;
            break;
    }       
    
}   //  PrintIpv6Addresses()



///////////////////////////////////////////////////////////////////////////////
//	DisplayV6OnlyAddresses()
//
//	Routine Description:
//
//		Run through the IP_ADAPTER_ADDRESSES list and print out V6 only
//      adapters.
//      
//	Arguments:
//		
//      pAdapterAddresses   ::  should be from GetAdapterAddresses().
//
//	Return Value:
//
//		None.
//

void
DisplayV6OnlyAddresses(PIP_ADAPTER_ADDRESSES pAdapterAddresses)
{
    PIP_ADAPTER_ADDRESSES   pAA;

    for (pAA = pAdapterAddresses ; pAA ; pAA = pAA->Next)
    {
        if ((pAA->IfIndex == 0)                        &&  
            (pAA->Ipv6IfIndex != 0)                    &&         
            (pAA->IfType != IF_TYPE_SOFTWARE_LOOPBACK) &&
            (pAA->OperStatus & IfOperStatusUp))
        {
            OutputMessage(TEXT("Tunnel adapter [%s]: \r\n"),
                pAA->Description);

            OutputMessage(TEXT("\t Interface Number .. : %d\r\n"), pAA->Ipv6IfIndex);

            PrintIpv6Addresses(NULL, pAA, PRINT_UNICAST_ADDRESS, FALSE);
            PrintIpv6Addresses(NULL, pAA, PRINT_ROUTER_ADDRESS,  FALSE);            
            OutputMessage(TEXT("\r\n"));            
        }
    }
}   //  DisplayV6OnlyAddresses()



// ================================================================================
//  Name         :  GetConfigData
//  Description  :  Gets the ipconfig information by calling 2 APIS, GetAdaptersInfo and getNetworkParams
//  Parameters   :   None
//  Returns      : integer 

//==================================================================================
int GetConfigData()
{

   //  -----------------------GetAdaptersInfo-----------------------------------------
   PIP_ADAPTER_INFO pAdapterInfo = NULL;
   ULONG ulSizeAdapterInfo = 0;
   DWORD dwReturnvalueGetAdapterInfo;
   PIP_ADAPTER_INFO pOriginalPtr;   
   LPTSTR pszNICAddr;

   ULONG                    ulBufferLength, ulFlags=0;
   DWORD                    dwError;
   PIP_ADAPTER_ADDRESSES    pAdapterAddresses;
   BOOL                     bFirstAddress;
   
   OutputMessage(TEXT("Windows IP configuration \r\n\r\n"));


   //
   //   Get V6 address list.
   //
   
   ulFlags = GAA_FLAG_SKIP_ANYCAST | GAA_FLAG_SKIP_MULTICAST;

   GetAdaptersAddresses(AF_UNSPEC, ulFlags, NULL, NULL, &ulBufferLength);

//HGO   OutputMessage(TEXT("GetAdaptersAddresses ulBufferLength: %d\r\n"), ulBufferLength);
   
   pAdapterAddresses = (PIP_ADAPTER_ADDRESSES)malloc(ulBufferLength);
   
   if (NULL == pAdapterAddresses)
   {
       OutputMessage(TEXT("Insufficient Memory\n"));
       return(1);
   }


   dwError = GetAdaptersAddresses(AF_UNSPEC, ulFlags, NULL, pAdapterAddresses, &ulBufferLength);
   
   if (dwError != NO_ERROR)
   {
       DisplayErrorMessage(dwError);
       return(1);
   }

   dwReturnvalueGetAdapterInfo = GetAdaptersInfo( pAdapterInfo, &ulSizeAdapterInfo );
   if ( dwReturnvalueGetAdapterInfo == ERROR_BUFFER_OVERFLOW) {
      if (!(pAdapterInfo = (PIP_ADAPTER_INFO)malloc(ulSizeAdapterInfo))) {
         OutputMessage(TEXT("Insufficient Memory\n"));
         return(1);
      }

      dwReturnvalueGetAdapterInfo = GetAdaptersInfo( pAdapterInfo, &ulSizeAdapterInfo);
      if (dwReturnvalueGetAdapterInfo != ERROR_SUCCESS) {
         DisplayErrorMessage(dwReturnvalueGetAdapterInfo);
         return(1);
      }
   }
   else if (dwReturnvalueGetAdapterInfo == ERROR_NO_DATA)
   {
       OutputMessage(TEXT("No IPv4 adapter found.\r\n\r\n"));
       return(1);
   }
   else if (dwReturnvalueGetAdapterInfo != ERROR_SUCCESS) {
      DisplayErrorMessage(dwReturnvalueGetAdapterInfo);
      return(1);
   }

   pOriginalPtr = pAdapterInfo;

   if(ulSizeAdapterInfo == 0)
      pAdapterInfo = NULL;

   if (pAdapterInfo == NULL)
      OutputMessage(TEXT("No Interfaces Present.\r\n"));

   while (pAdapterInfo != NULL) {

   OutputMessage(TEXT("AdapterInfo->Index : %d\r\n"), pAdapterInfo->Index);
   OutputMessage(TEXT("AdapterInfo->Type : %d\r\n"), pAdapterInfo->Type);
   
     switch (pAdapterInfo->Type)
     {
        case IF_TYPE_ETHERNET:
            if (bflagall)
                OutputMessage(TEXT("Ethernet adapter Local Area Connection: \r\n"));                
            else
                OutputMessage(TEXT("Ethernet adapter [%hs]: \r\n"),
                    pAdapterInfo->AdapterName);                
            break;

        case IF_TYPE_PPP:
            OutputMessage(TEXT("PPP Adapter [%hs]:\r\n"), pAdapterInfo->AdapterName);
            break;

        case IF_TYPE_OTHER:  
        case IF_TYPE_TOKENRING:
        case IF_TYPE_FDDI:
        case IF_TYPE_LOOPBACK:
        case IF_TYPE_SLIP:
            OutputMessage(TEXT("IP connection:\r\n\r\n"));
            break;
     }

      

      PIP_ADDR_STRING pAddressList = &(pAdapterInfo->IpAddressList);

      do {
         OutputMessage(TEXT("\t IP Address ........ : %hs\r\n"),pAddressList->IpAddress.String);
         OutputMessage(TEXT("\t Subnet Mask ....... : %hs\r\n"), pAddressList->IpMask.String);
         pAddressList = pAddressList->Next;
      } while (pAddressList != NULL);


      PrintIpv6Addresses(pAdapterInfo, pAdapterAddresses, PRINT_UNICAST_ADDRESS, FALSE);

      PIP_ADDR_STRING pGatewayList = &(pAdapterInfo->GatewayList);

      if (pGatewayList->IpAddress.String[0] != 0)
      {
         do {
            OutputMessage(TEXT("\t Default Gateway ... : %hs\r\n"), pGatewayList->IpAddress.String);
            pGatewayList = pGatewayList->Next;
         } while (pGatewayList != NULL);
      }

      PrintIpv6Addresses(pAdapterInfo, pAdapterAddresses, PRINT_ROUTER_ADDRESS, TRUE);

      if (bflagall == TRUE) {
         OutputMessage(TEXT("\t Adapter Name ...... : %hs\r\n"), pAdapterInfo->AdapterName);
         OutputMessage(TEXT("\t Description ....... : %hs\r\n"), pAdapterInfo->Description);
         OutputMessage(TEXT("\t Adapter Index ..... : %lu\r\n"), pAdapterInfo->Index);
         if (pszNICAddr = FormatNICAddrString(pAdapterInfo->Address, pAdapterInfo->AddressLength)) {
             OutputMessage(TEXT("\t Address............ : %s\r\n"), pszNICAddr); 
             LocalFree(pszNICAddr);
         }


         OutputMessage(TEXT("\t DHCP Enabled....... : %s\r\n"), (pAdapterInfo->DhcpEnabled) ? TEXT("YES") : TEXT("NO"));

         if (pAdapterInfo->DhcpEnabled) {
             //
             // It does not make sense to display the following information if DHCP is not enabled.
             //
             PIP_ADDR_STRING pDhcpServer = &(pAdapterInfo->DhcpServer);
             do {
                OutputMessage(TEXT("\t DHCP Server........ : %hs\r\n"), pDhcpServer->IpAddress.String);
                pDhcpServer = pDhcpServer->Next;
             } while (pDhcpServer != NULL);
    
             PIP_ADDR_STRING pPrimaryWinsServer = &(pAdapterInfo->PrimaryWinsServer) ;
             do {
                OutputMessage(TEXT("\t Primary WinsServer  : %hs\r\n"), pPrimaryWinsServer->IpAddress.String);
                pPrimaryWinsServer=pPrimaryWinsServer->Next;
             } while (pPrimaryWinsServer != NULL);
    
             PIP_ADDR_STRING pSecondaryWinsServer =&(pAdapterInfo->SecondaryWinsServer) ;
             do {
                OutputMessage(TEXT("\t Secondary WinsServer: %hs\r\n"), pSecondaryWinsServer->IpAddress.String);
                pSecondaryWinsServer = pSecondaryWinsServer->Next;
             } while (pSecondaryWinsServer != NULL);
    
             PrintIpv6Addresses(pAdapterInfo, pAdapterAddresses, PRINT_DNSSERVER_ADDRESS, FALSE);
    
             FILETIME LeaseObtained, LeaseObtainedLocal;
             SYSTEMTIME SysLeaseObtained;
    
             if (pAdapterInfo->LeaseObtained != 0) {         	
             	TimeToFileTime(pAdapterInfo->LeaseObtained, &LeaseObtained);
             	FileTimeToLocalFileTime(&LeaseObtained, &LeaseObtainedLocal);    
             	BOOL bflag = FileTimeToSystemTime(&LeaseObtainedLocal, &SysLeaseObtained);
             	OutputMessage(TEXT("\t Lease obtained on   : %s, %s %lu ,%lu %lu : %lu : %lu  \r\n"), dayofweek[SysLeaseObtained.wDayOfWeek], month[SysLeaseObtained.wMonth], SysLeaseObtained.wDay, SysLeaseObtained.wYear, SysLeaseObtained.wHour, SysLeaseObtained.wMinute, SysLeaseObtained.wSecond);
             }
             else
             	OutputMessage(TEXT("\t Lease obtained on   : Not Available\r\n"));
             
             FILETIME LeaseExpires, LeaseExpiresLocal;
             SYSTEMTIME SysLeaseExpires;
    
             if (pAdapterInfo->LeaseExpires != 0) {
             	TimeToFileTime(pAdapterInfo->LeaseExpires, &LeaseExpires);
             	FileTimeToLocalFileTime(&LeaseExpires, &LeaseExpiresLocal);
             	FileTimeToSystemTime(&LeaseExpiresLocal, &SysLeaseExpires);
             	OutputMessage(TEXT("\t Lease expires on    : %s, %s %lu ,%lu %lu : %lu : %lu  \r\n"), dayofweek[SysLeaseExpires.wDayOfWeek], month[SysLeaseExpires.wMonth], SysLeaseExpires.wDay, SysLeaseExpires.wYear, SysLeaseExpires.wHour, SysLeaseExpires.wMinute, SysLeaseExpires.wSecond );
             }
             else
             	OutputMessage(TEXT("\t Lease expires on    : Not Available\r\n"));
    
             DWORD Index = (ULONG)(pAdapterInfo->Index);
             OutputMessage(TEXT("\t AutoConfig Enabled  : %s\r\n"), IsAutoConfigEnabled(Index) ? TEXT("YES") : TEXT("NO"));
         }  // Dhcp enabled
      }  // Matches if(blagall==TRUE) {                 

      OutputMessage(TEXT("\r\n"));
      pAdapterInfo = pAdapterInfo->Next;
   } //  End of while(pAdapterInfo!=NULL){

   //
   //   V6 only addresses..
   //

   DisplayV6OnlyAddresses(pAdapterAddresses);
   

   //---------------------End of GetAdaptersInfo-------------------

   // ---------------------GetNetwork Params --------------------------

   PFIXED_INFO pNetworkParams  = NULL;
   ULONG uSizeNetworkParams = 0;
   DWORD dwReturnvalueGetNetworkParams;

   dwReturnvalueGetNetworkParams = GetNetworkParams(pNetworkParams , &uSizeNetworkParams);
   if (dwReturnvalueGetNetworkParams == ERROR_BUFFER_OVERFLOW) {
      if (!(pNetworkParams = (PFIXED_INFO) malloc(uSizeNetworkParams))) {
         OutputMessage(TEXT("Insufficient Memory\r\n"));
         return(1);
      }

      dwReturnvalueGetNetworkParams = GetNetworkParams(pNetworkParams, &uSizeNetworkParams);
      if (dwReturnvalueGetNetworkParams != ERROR_SUCCESS) {
         DisplayErrorMessage(dwReturnvalueGetNetworkParams);
         return(1);
      }
   }
   else if (dwReturnvalueGetNetworkParams != ERROR_SUCCESS) {
      DisplayErrorMessage(dwReturnvalueGetNetworkParams);
      return(1);
   }

   if (bflagall == TRUE)
      OutputMessage(TEXT("\t Host name.......... : %hs\r\n"), pNetworkParams->HostName);
   if (bflagall == TRUE)
      OutputMessage(TEXT("\t Domain Name........ : %hs\r\n"), pNetworkParams->DomainName);

   PIP_ADDR_STRING pDnsServer = &(pNetworkParams->DnsServerList);

   // If there is no adapters - there will be no dns server - but there will be
   // a pointer - so we much check to see if the address string is empty
   if (pDnsServer->IpAddress.String[0] != 0)
   {
       bFirstAddress = TRUE;
       while (pDnsServer != NULL) 
       {  
           if (bFirstAddress)
               OutputMessage(TEXT("\t DNS Servers........ : %hs\r\n"), pDnsServer->IpAddress.String); 
           else
               OutputMessage(TEXT("\t                       %hs\r\n"), pDnsServer->IpAddress.String);

           bFirstAddress = FALSE;
           pDnsServer    = pDnsServer->Next; 
       }

   }

   if (bflagall == TRUE) {
      OutputMessage(TEXT("\t NODETYPE........... :  %u "), pNetworkParams->NodeType); //No details in the documentation regarding the correspondence between integer constants and nodetypes   
      OutputMessage(TEXT("\t Routing Enabled.... : %s\r\n"),  pNetworkParams->EnableRouting ? TEXT("YES") : TEXT(" NO"));
      OutputMessage(TEXT("\t Proxy Enabled...... :  %s\r\n"), pNetworkParams->EnableProxy ? TEXT("YES") : TEXT("NO"));
   }

   //----------------End of GetNetwork Params --------------------------
   OutputMessage(TEXT("\r\n"));

   if (pNetworkParams) 
      free(pNetworkParams);
   if (pOriginalPtr)  
      free(pOriginalPtr);
   return(0);

}

void RenewAddress(DWORD AdapterIndex) {
   ULONG sizeofbuffer=0;
   PIP_INTERFACE_INFO pbuffer=NULL;
   int returnvalue;
   BOOL bfound =FALSE;

   returnvalue= GetInterfaceInfo(pbuffer, &sizeofbuffer);
   if (returnvalue==ERROR_INSUFFICIENT_BUFFER) {
      pbuffer=(PIP_INTERFACE_INFO) malloc(sizeofbuffer);
      if (pbuffer != NULL) {
          returnvalue=GetInterfaceInfo(pbuffer, &sizeofbuffer);
      }
   }
   if (returnvalue!=NO_ERROR) {
      DisplayErrorMessage(returnvalue);
      exit(1);
   }

     OutputMessage(TEXT("\tAdapter Name: %s\r\n"), pbuffer->Adapter[0].Name);
     OutputMessage(TEXT("\tAdapter Index: %ld\r\n"), pbuffer->Adapter[0].Index);
     OutputMessage(TEXT("\tNum Adapters: %ld\r\n"), pbuffer->NumAdapters);


   for (int index=0;index<pbuffer->NumAdapters; index++) {
      if ((pbuffer->Adapter[index].Index==AdapterIndex)||(AdapterIndex==0)) {
         bfound =TRUE;
         //OutputMessage(TEXT(" Found the adapter"));
         returnvalue=IpRenewAddress(&(pbuffer->Adapter[index]));
         if (returnvalue==NO_ERROR)
            OutputMessage(TEXT("Successfully Renewed Adapter with Index Number %lu\r\n"), pbuffer->Adapter[index].Index);
         else {
            OutputMessage(TEXT("Failed to renew Adpter with Index Number %lu\r\n"), pbuffer->Adapter[index].Index);
            DisplayErrorMessage(returnvalue);
         } 
      }

   }
   if (!bfound)
      OutputMessage(TEXT("Adapter with given Index Number not found\r\n"));

   if (pbuffer) 
       free(pbuffer);
}


void ReleaseAddress( DWORD AdapterIndex) {

   BOOL bfound =FALSE;
   ULONG sizeofbuffer=0;
   PIP_INTERFACE_INFO prelease=NULL;
   int returnvalue;

   returnvalue= GetInterfaceInfo(prelease, &sizeofbuffer);
   if (returnvalue==ERROR_INSUFFICIENT_BUFFER) {
      prelease=(PIP_INTERFACE_INFO) malloc(sizeofbuffer);
      if (prelease != NULL) {
          returnvalue=GetInterfaceInfo(prelease, &sizeofbuffer);
      }
   }
   if (returnvalue!=NO_ERROR) {
      DisplayErrorMessage(returnvalue);
      exit(1);
   }

   for (int index=0;index<prelease->NumAdapters; index++) {
      if ((AdapterIndex==0) || (prelease->Adapter[index].Index==AdapterIndex)) {
         bfound=TRUE;
         // OutputMessage(TEXT(" Found the adapter"));
         returnvalue=IpReleaseAddress(&(prelease->Adapter[index]));
         if (returnvalue==NO_ERROR)
            OutputMessage(TEXT("Successfully Released adapter with index Number %lu\r\n"),prelease->Adapter[index].Index);
         else {
            OutputMessage(TEXT("Failed to release Adapter with index Number %lu\r\n"), prelease->Adapter[index].Index);
            DisplayErrorMessage(returnvalue);
         }
      }

   }


   if (!bfound)
      OutputMessage(TEXT("Adapter with the given index Number not found\r\n"));

   if (prelease) 
       free(prelease);
}


#define WSCNTL_FLUSH_NAME_RESOLVER_CACHE 6

extern "C"
DWORD
__cdecl
WSAControl(
    DWORD   Protocol,
    DWORD   Action,
    LPVOID  InputBuffer,
    LPDWORD InputBufferLength,
    LPVOID  OutputBuffer,
    LPDWORD OutputBufferLength
    );

void FlushDNSCache(void)
{
	OutputMessage(L"flushing DNS...\r\n");
    DWORD dwRes = WSAControl(-1, WSCNTL_FLUSH_NAME_RESOLVER_CACHE, NULL, NULL, NULL, NULL);
	OutputMessage(L"flushing DNS done: %i\r\n", dwRes);
}
