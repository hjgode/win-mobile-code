# An app to show unicode char table #

Sometimes you may want to see which chars (better say glyphs) are supported by a font installed on your mobile device. Unfortunately there is no charmap application pre-installed on windows mobile OS and I did not find any current app for this need.

So I started this small project to enable you to check the glyphs for installed fonts. You can switch the char set table index (for example 0x04 is for Cyrillic) and the font.

CharMapCF uses UCS-2, UTF-16 encoding and is able to switch between all 256 char tables of the unicode Basic Multilanguage Plane (BMP).

Using this small tool you may find many code points and charset table index with undefined glyphs. Dont worry, this is normal. The unicode consortium has not defined
all code points and char sets with glyphs, there i still room for additional glyphs.

CharmapCF will show all glyphs of 256 code points of one char set index at once within a table. You can easily find the unicode UCS-2 code point for all displayed glyphs as there is a header row and column with the codepoint number as hex.