========================================================================
       Windows CE APPLICATION : iKill2
========================================================================

iKill2 will kill a windows mobile process

_usage_

iKill2.exe [-f] application_exe_name
iKill2.exe [-f] "application exe name"

_examples_

iKill2.exe iexplore.exe
	first tries to quit target application and then a TerminateProcess

iKill2.exe -f iexplore.exe
	kills the target process using TerminateProcess

alternatively use quotes around application names including spaces

_return codes_

0 for success
negative values for failure

/////////////////////////////////////////////////////////////////////////////
