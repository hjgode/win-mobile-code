using System;

using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

//disable unused warnings
#pragma warning disable 0169

namespace CharMapCF
{
    class FontClass
    {
        delegate int EnumFontDelegate(IntPtr lpelfe, IntPtr lpntme, EnumFontsType FontType, int lParam);
        private List<string> fontNames;
        IntPtr fpEnumProc;
        EnumFontDelegate enumFontDelegate;

        [DllImport("coredll.dll")]
        private static extern int EnumFontFamilies(IntPtr hdc, string
        fontFamily, IntPtr lpEnumFontFamExProc, IntPtr lParam);

        [DllImport("coredll.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("coredll.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("coredll.dll")]
        private static extern IntPtr ReleaseDC(IntPtr hdc);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        class LOGFONT
        {
            public int lfHeight = 0;
            public int lfWidth = 0;
            public int lfEscapement = 0;
            public int lfOrientation = 0;
            public /*int*/ lfWeightType lfWeight = 0;
            public byte lfItalic = 0;
            public byte lfUnderline = 0;
            public byte lfStrikeOut = 0;
            public /*byte*/lfCharsetType lfCharSet = lfCharsetType.DEFAULT_CHARSET; // 1 = DEFAULT_CHARSET
            public byte lfOutPrecision = 0;
            public byte lfClipPrecision = 0;
            public byte lfQuality = 0;
            public byte lfPitchAndFamily = 0;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string
                lfFaceName = null;

        }
        enum lfPitchAndFamilyType{
            DEFAULT_PITCH = 0,
            FIXED_PITCH = 1,
            VARIABLE_PITCH = 2
        };

        enum lfCharsetType : byte{
            ANSI_CHARSET = 0,
            DEFAULT_CHARSET = 1,
            SYMBOL_CHARSET = 2,
            SHIFTJIS_CHARSET = 128,
            HANGEUL_CHARSET = 129,
            HANGUL_CHARSET = 129,
            GB2312_CHARSET = 134,
            CHINESEBIG5_CHARSET = 136,
            OEM_CHARSET = 255,

            JOHAB_CHARSET = 130,
            HEBREW_CHARSET = 177,
            ARABIC_CHARSET = 178,
            GREEK_CHARSET = 161,
            TURKISH_CHARSET = 162,
            VIETNAMESE_CHARSET = 163,
            THAI_CHARSET = 222,
            EASTEUROPE_CHARSET = 238,
            RUSSIAN_CHARSET = 204,
            MAC_CHARSET = 77,
            BALTIC_CHARSET = 186
        };

        [Flags]
        enum lfLangSetType:uint{
            FS_LATIN1 = 0x00000001,
            FS_ATIN2 = 0x00000002,
            FS_CYRIIC = 0x00000004,
            FS_GREEK = 0x00000008,
            FS_TURKISH = 0x00000010,
            FS_HEBREW = 0x00000020,
            FS_ARABIC = 0x00000040,
            FS_BATIC = 0x00000080,
            FS_VIETNAMESE = 0x00000100,
            FS_THAI = 0x00010000,
            FS_JISJAPAN = 0x00020000,
            FS_CHINESESIMP = 0x00040000,
            FS_WANSUNG = 0x00080000,
            FS_CHINESETRAD = 0x00100000,
            FS_JOHAB = 0x00200000,
            FS_SYMBO = 0x80000000
        };

        /// <summary>
        /// Specifies whether the font is italic, underscored, outlined, bold, and so forth. 
        /// The following list shows the bits corresponding to each font type.
        /// </summary>
        [Flags]
        enum ntmFlags{
            //Bit 	Font
	        Italic = 0,
	        Underscore = 0x02,
	        Negative = 0x04,
	        Outline = 0x08,
	        Strikeout = 0x10,
	        Bold = 0x20
	    };
        class NEWTEXTMETRIC
        {
            UInt32 tmHeight; //LONG
            UInt32 tmAscent; 
            UInt32 tmDescent; 
            UInt32 tmInternalLeading; 
            UInt32 tmExternalLeading; 
            UInt32 tmAveCharWidth; 
            UInt32 tmMaxCharWidth; 
            UInt32 tmWeight; 
            UInt32 tmOverhang; 
            UInt32 tmDigitizedAspectX; 
            UInt32 tmDigitizedAspectY;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            string tmFirstChar; //BCHAR
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            string tmLastChar;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            string tmDefaultChar;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            string tmBreakChar; 
            byte tmItalic; 
            byte tmUnderlined; 
            byte tmStruckOut; 
            byte tmPitchAndFamily; 
            byte tmCharSet;
            short ntmFlags;  //DWORD
            uint ntmSizeEM; 
            uint ntmCellHeight; 
            uint ntmAvgWidth; 
        } ;

        enum EnumFontsType
        {
            DEVICE_FONTTYPE = 0x0000,   //???
            RASTER_FONTTYPE = 0x0001,   //wingdi.h
            TRUETYPE_FONTTYPE = 0x0004  //wingdi.h
        };
        /// <summary>
        /// Specifies the weight of the font in the range 0 through 1000. For example, 400 is normal and 700 is bold. 
        /// If this value is zero, a default weight is used.
        /// </summary>
        public enum lfWeightType
        {
            //The following values are defined for convenience.
            //Value 	Weight,
            FW_DONTCARE = 0,
            FW_THIN = 100,
            FW_EXTRALIGHT = 200,
            FW_ULTRALIGHT = 200,
            FW_LIGHT = 300,
            FW_NORMAL = 400,
            FW_REGULAR = 400,
            FW_MEDIUM = 500,
            FW_SEMIBOLD = 600,
            FW_DEMIBOLD = 600,
            FW_BOLD = 700,
            FW_EXTRABOLD = 800,
            FW_ULTRABOLD = 800,
            FW_HEAVY = 900,
            FW_BLACK = 900
        };
        public FontClass()
        {
            fontNames = new List<string>();
            buildList();
        }
        /*
        int CALLBACK EnumFontFamProc(
          const LOGFONT FAR* lpelf, 
          const TEXTMETRIC FAR* lpntm, 
          DWORD FontType, 
          LPARAM lParam
        );
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpelfe">IntPtr LOGFONT structure that contains information about the logical attributes of the font</param>
        /// <param name="lpntme">structure that contains information about the physical attributes of the font, if the font is a TrueType font. If the font is not a TrueType font, this parameter points to a TEXTMETRIC structure</param>
        /// <param name="FontType">[in] DWORD that specifies the type of the font:
        /// DEVICE_FONTTYPE
        /// RASTER_FONTTYPE
        /// TRUETYPE_FONTTYPE</param>
        /// <param name="lParam">Pointer to the application-defined data passed by the EnumFontFamilies function</param>
        /// <returns>Nonzero continues enumeration. Zero stops enumeration</returns>
        private int EnumFontFamiliesExProc(IntPtr lpelfe, IntPtr lpntme, EnumFontsType FontType, int lParam)
        {
            LOGFONT logFont = (LOGFONT)Marshal.PtrToStructure(lpelfe, typeof(LOGFONT));

            if (logFont.lfWeight == lfWeightType.FW_REGULAR && logFont.lfItalic==0)
            {
                //we dont like duplicate names
                if(!fontNames.Contains(logFont.lfFaceName))
                    fontNames.Add(logFont.lfFaceName);

                System.Diagnostics.Debug.WriteLine(logFont.lfFaceName);
            }
            // Non-zero return continues enumerating
            return 1;
        }
        private void buildList(){
            // Need an HDC to pass to EnumFontFamilies
            IntPtr hwnd = GetDesktopWindow();
            IntPtr hdc = GetDC(hwnd);

            LOGFONT logFont = new LOGFONT();

            enumFontDelegate = new EnumFontDelegate(EnumFontFamiliesExProc);
            fpEnumProc = Marshal.GetFunctionPointerForDelegate(enumFontDelegate);

            EnumFontFamilies(hdc, null, fpEnumProc, IntPtr.Zero);

            // We got a list of the major families.  Copy the list,
            // then clear it so we can go back and grab all the individual fonts.
            List<string> fontFamilies = new List<string>();
            fontFamilies.AddRange(fontNames);
            fontNames.Clear();

            foreach(string fontFamily in fontFamilies)
            {
                EnumFontFamilies(hdc, fontFamily, fpEnumProc, IntPtr.Zero);
            }

            ReleaseDC(hdc);

            foreach(string s in fontNames)
            {
                //listBox1.Items.Add(s);
            }
        }
        public string[] getFontList()
        {
            buildList();
            string[] sList = new string[fontNames.Count];
            int i = 0;
            foreach(string s in fontNames)
            {
                sList[i] = s;
                i++;
                //listBox1.Items.Add(s);
            }
            return sList;
        }
    }
}
