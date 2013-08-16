========================================================================
    CONSOLE APPLICATION : iBacklightCLI Project Overview
========================================================================

iBacklightCLI

arguments			function

on					switch backlight on using current brightness
off					switch backlight off
max					switch backlight brightness to max and on
min					switch backlight brightness to min and on

state				return 0 for is OFF, 1 for ON, -1 for error
level				return current brightness level or -1 for error

1 to max level		set new brightness level, returns new level or -1 for error (ie val exceeds max level)

/////////////////////////////////////////////////////////////////////////////s