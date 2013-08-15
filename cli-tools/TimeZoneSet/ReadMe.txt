========================================================================
       Windows CE APPLICATION : TimeZoneSet
========================================================================

the app only supports one argument, a positive or negative number specifying
the GMT offset in inverted notation.
If you want to set GMT+1 you need to run

TimeZoneSet -1

You cannot specify a city or other informations like DST on/off. This would 
require another tool.

TimeZoneSet always dumps a list of known cities and timezones to 
\TZ-cities.TXT

/////////////////////////////////////////////////////////////////////////////
