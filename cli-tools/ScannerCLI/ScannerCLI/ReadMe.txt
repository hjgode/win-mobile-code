========================================================================
    CONSOLE APPLICATION : ScannerCLI Project Overview
========================================================================

Just issue the scanner for a max time of supplied arg

default timeout is 1 second

"ScannerCLI.exe x" will issue a barcode scan with a timeout of x seconds. 

x must be > 0 and < 10.

"ScannerCLI.exe" will issue a barcode scan for max 1 second. The scan will
be stopped immediately if a barcode is read.

/////////////////////////////////////////////////////////////////////////////s