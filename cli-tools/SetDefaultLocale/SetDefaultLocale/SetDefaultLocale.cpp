// SetDefaultLocale.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

TCHAR* localeList[];

LANGID getLangID(){
	return GetUserDefaultLangID();
	//GetUserDefaultLCID();
	//GetUserDefaultUILanguage();
}

LANGID getSystemLangID(){
	return GetSystemDefaultUILanguage();
}
BOOL setSystemDefaultLCID(LCID lcID){
	//you can not change the system default locale!
	return SetSystemDefaultLCID(lcID);
}
BOOL setLangID(LANGID id){
	return SetUserDefaultUILanguage(id);
	//SetUserDefaultLangID();
}

LCID getLCID(){
	return GetUserDefaultLCID();
}

BOOL setLCID(LCID lcID){
	return SetUserDefaultLCID(lcID);
}

LANGID getUserLanguageID(){
	return GetUserDefaultLangID();
}

//argv[0] is the exe name!
int _tmain(int argc, _TCHAR* argv[])
{
	initFileNames();
	Add2Log(L"SetDefaultLocale v 0.1 started\n==============================\n");	          
	TCHAR str[MAX_PATH];

	if(argc==1){
		Add2Log(L"no args no fun.\ntry -help or a locale ID as hex (ie '0x407')\n---Program END\n");
		return -1;
	}

	if(argc==2){
		if (wcsicmp(argv[1], L"-help")==0){
			int i=0;
			do{
				Add2Log(localeList[i]); Add2Log(L"\n");
				i++;
			}while (localeList[i]!=NULL);
		}
		if (wcsicmp(argv[1], L"-status")==0){
			wsprintf(str, L"current user default LCID is: 0x%03x\n", getLCID());
			Add2Log(str);
			wsprintf(str, L"current user default langID is: 0x%03x\n", getLangID());
			Add2Log(str);
			wsprintf(str, L"current system langID is: 0x%03x\n", getSystemLangID());
			Add2Log(str);
		}
		if (wcsncmp(argv[1], L"0x", 2)==0){
			int lcID = wcstol(argv[1], (TCHAR **)NULL, 16);
			//set local ID
			if(setLCID(lcID))
				wsprintf(str, L"new user default LCID is: 0x%03x\n", getLCID());
			else
				wsprintf(str, L"setting LCID failed\n");
			Add2Log(str);

			if(setLangID(lcID))
				wsprintf(str, L"new user default langID is: 0x%03x\n", getLangID());
			else
				wsprintf(str, L"setting langID failed\n");
			Add2Log(str);

			if(setSystemDefaultLCID(lcID))
				wsprintf(str, L"system langID is: 0x%03x\n", getSystemLangID());
			else
				wsprintf(str, L"setting system langID failed\n");
			Add2Log(str);

		}
	}

	Add2Log(L"\n---Program END\n");
	return 0;
}

TCHAR* localeList[] = {
	L"supported arguments:",
	L"-help		print this help to log",
	L"-status	get current locale ID (LCID)",
	L"--------------------------------",
L"Locale identifier             Language                      Sublanguage - locale          Default code page             Language code",
L"0x0436                        Afrikaans                     South Africa                  1252                          AFK",
L"0x041c                        Albanian                      Albania                       1250                          SQI",
L"0x1401                        Arabic                        Algeria                       1256                          ARG",
L"0x3c01                        Arabic                        Bahrain                       1256                          ARH",
L"0x0c01                        Arabic                        Egypt                         1256                          ARE",
L"0x0801                        Arabic                        Iraq                          1256                          ARI",
L"0x2c01                        Arabic                        Jordan                        1256                          ARJ",
L"0x3401                        Arabic                        Kuwait                        1256                          ARK",
L"0x3001                        Arabic                        Lebanon                       1256                          ARB",
L"0x1001                        Arabic                        Libya                         1256                          ARL",
L"0x1801                        Arabic                        Morocco                       1256                          ARM",
L"0x2001                        Arabic                        Oman                          1256                          ARO",
L"0x4001                        Arabic                        Qatar                         1256                          ARQ",
L"0x0401                        Arabic                        Saudi Arabia                  1256                          ARA",
L"0x2801                        Arabic                        Syria                         1256                          ARS",
L"0x1c01                        Arabic                        Tunisia                       1256                          ART",
L"0x3801                        Arabic                        U.A.E.                        1256                          ARU",
L"0x2401                        Arabic                        Yemen                         1256                          ARY",
L"0x042b                        Armenian                      Armenia                       Unicode only                  HYE",
L"0x044d                        Assamese                      India                         Unicode only                  ASM",
L"0x082c                        Azeri                         Azerbaijan (Cyrillic)         1251                          AZE",
L"0x042c                        Azeri                         Azerbaijan (Latin)            1254                          AZE",
L"0x042d                        Basque                        Spain                         1252                          EUQ",
L"0x0423                        Belarusian                    Belarus                       1251                          BEL",
L"0x0445                        Bengali                       India                                                       BEN",
L"0x0402                        Bulgarian                     Bulgaria                      1251                          BGR",
L"0x0403                        Catalan                       Spain                         1252                          CAT",
L"0x0c04                        Chinese                       Hong Kong SAR                 950                           ZHH",
L"0x1404                        Chinese                       Macao SAR                     950                           ZHM",
L"0x0804                        Chinese                       PRC                           936                           CHS",
L"0x1004                        Chinese                       Singapore                     936                           ZHI",
L"0x0404                        Chinese                       Taiwan                        950                           CHT",
L"0x0827                        Classic Lithuanian            Lithuania                     1257                          LTC",
L"0x041a                        Croatian                      Croatia                       1250                          HRV",
L"0x0405                        Czech                         Czech Republic                1250                          CSY",
L"0x0406                        Danish                        Denmark                       1252                          DAN",
L"0x0465                        Divehi                        Maldives                      Unicode only                  DIV",
L"0x0813                        Dutch                         Belgium                       1252                          NLB",
L"0x0413                        Dutch                         Netherlands                   1252                          NLD",
L"0x0c09                        English                       Australia                     1252                          ENA",
L"0x2809                        English                       Belize                        1252                          ENL",
L"0x1009                        English                       Canada                        1252                          ENC",
L"0x2409                        English                       Caribbean                     1252                          ENB",
L"0x1809                        English                       Ireland                       1252                          ENI",
L"0x2009                        English                       Jamaica                       1252                          ENJ",
L"0x1409                        English                       New Zealand                   1252                          ENZ",
L"0x3409                        English                       Philippines                   1252                          ENP",
L"0x1c09                        English                       South Africa                  1252                          ENS",
L"0x2c09                        English                       Trinidad                      1252                          ENT",
L"0x0809                        English                       United Kingdom                1252                          ENG",
L"0x0409                        English                       United States                 1252                          USA",
L"0x3009                        English                       Zimbabwe                      1252                          ENW",
L"0x0425                        Estonian                      Estonia                       1257                          ETI",
L"0x0438                        Faeroese                      Faeroe Islands                1252                          FOS",
L"0x0429                        Farsi                         Iran                          1256                          FAR",
L"0x040b                        Finnish                       Finland                       1252                          FIN",
L"0x080c                        French                        Belgium                       1252                          FRB",
L"0x0c0c                        French                        Canada                        1252                          FRC",
L"0x040c                        French                        France                        1252                          FRA",
L"0x140c                        French                        Luxembourg                    1252                          FRL",
L"0x180c                        French                        Monaco                        1252                          FRM",
L"0x100c                        French                        Switzerland                   1252                          FRS",
L"0x042f                        Macedonian (FYROM)            Macedonian (FYROM)            1251                          MKI",
L"0x0456                        Galician                      Spain                         1252                          GLC",
L"0x0437                        Georgian                      Georgia                       Unicode only                  KAT",
L"0x0c07                        German                        Austria                       1252                          DEA",
L"0x0407                        German                        Germany                       1252                          DEU",
L"0x1407                        German                        Liechtenstein                 1252                          DEC",
L"0x1007                        German                        Luxembourg                    1252                          DEL",
L"0x0807                        German                        Switzerland                   1252                          DES",
L"0x0408                        Greek                         Greece                        1253                          ELL",
L"0x0447                        Gujarati                      India                         Unicode only                  GUJ",
L"0x040d                        Hebrew                        Israel                        1255                          HEB",
L"0x0439                        Hindi                         India                         Unicode only                  HIN",
L"0x040e                        Hungarian                     Hungary                       1250                          HUN",
L"0x040f                        Icelandic                     Iceland                       1252                          ISL",
L"0x0421                        Indonesian                    Indonesia (Bahasa)            1252                          IND",
L"0x0410                        Italian                       Italy                         1252                          ITA",
L"0x0810                        Italian                       Switzerland                   1252                          ITS",
L"0x0411                        Japanese                      Japan                         932                           JPN",
L"0x044b                        Kannada                       India (Kannada script)        Unicode only                  KAN",
L"0x043f                        Kazakh                        Kazakstan                     1251                          KKZ",
L"0x0457                        Konkani                       India                         Unicode only                  KNK",
L"0x0412                        Korean                        Korea                         949                           KOR",
L"0x0440                        Kyrgyz                        Kyrgyzstan                    1251                          KYR",
L"0x0426                        Latvian                       Latvia                        1257                          LVI",
L"0x0427                        Lithuanian                    Lithuania                     1257                          LTH",
L"0x083e                        Malay                         Brunei Darussalam             1252                          MSB",
L"0x043e                        Malay                         Malaysia                      1252                          MSL",
L"0x044c                        Malayalam                     India                         Unicode only                  MAL",
L"0x044e                        Marathi                       India                         Unicode only                  MAR",
L"0x0450                        Mongolian (Cyrillic)          Mongolia                      1251                          MON",
L"0x0414                        Norwegian                     Norway (Bokmål)              1252                          NOR",
L"0x0814                        Norwegian                     Norway (Nynorsk)              1252                          NON",
L"0x0448                        Oriya                         India                                                       ORI",
L"0x0415                        Polish                        Poland                        1250                          PLK",
L"0x0416                        Portuguese                    Brazil                        1252                          PTB",
L"0x0816                        Portuguese                    Portugal                      1252                          PTG",
L"0x0446                        Punjabi                       India (Gurmukhi script)       Unicode only                  PAN",
L"0x0418                        Romanian                      Romania                       1250                          ROM",
L"0x0419                        Russian                       Russia                        1251                          RUS",
L"0x044f                        Sanskrit                      India                         Unicode only                  SAN",
L"0x0c1a                        Serbian                       Serbia (Cyrillic)             1251                          SRB",
L"0x081a                        Serbian                       Serbia (Latin)                1250                          SRL",
L"0x041b                        Slovak                        Slovakia                      1250                          SKY",
L"0x0424                        Slovenian                     Slovenia                      1250                          SLV",
L"0x2c0a                        Spanish                       Argentina                     1252                          ESS",
L"0x400a                        Spanish                       Bolivia                       1252                          ESB",
L"0x340a                        Spanish                       Chile                         1252                          ESL",
L"0x240a                        Spanish                       Colombia                      1252                          ESO",
L"0x140a                        Spanish                       Costa Rica                    1252                          ESC",
L"0x1c0a                        Spanish                       Dominican Republic            1252                          ESD",
L"0x300a                        Spanish                       Ecuador                       1252                          ESF",
L"0x440a                        Spanish                       El Salvador                   1252                          ESE",
L"0x100a                        Spanish                       Guatemala                     1252                          ESG",
L"0x480a                        Spanish                       Honduras                      1252                          ESH",
L"0x080a                        Spanish                       Mexico                        1252                          ESM",
L"0x4c0a                        Spanish                       Nicaragua                     1252                          ESI",
L"0x180a                        Spanish                       Panama                        1252                          ESA",
L"0x3c0a                        Spanish                       Paraguay                      1252                          ESZ",
L"0x280a                        Spanish                       Peru                          1252                          ESR",
L"0x500a                        Spanish                       Puerto Rico                   1252                          ESU",
L"0x040a                        Spanish                       Spain (Traditional sort)      1252                          ESP",
L"0x0c0a                        Spanish                       Spain (International sort)    1252                          ESN",
L"0x380a                        Spanish                       Uruguay                       1252                          ESY",
L"0x200a                        Spanish                       Venezuela                     1252                          ESV",
L"0x0441                        Swahili                       Kenya                         1252                          SWK",
L"0x081d                        Swedish                       Finland                       1252                          SVF",
L"0x041d                        Swedish                       Sweden                        1252                          SVE",
L"0x045a                        Syriac                        Syria                         Unicode only                  SYR",
L"0x0449                        Tamil                         India                         Unicode only                  TAM",
L"0x0444                        Tatar                         Tatarstan                     1251                          TTT",
L"0x044a                        Telugu                        India (Telugu script)         Unicode only                  TEL",
L"0x041e                        Thai                          Thailand                      874                           THA",
L"0x041f                        Turkish                       Turkey                        1254                          TRK",
L"0x0422                        Ukrainian                     Ukraine                       1251                          UKR",
L"0x0420                        Urdu                          Pakistan                      1256                          URP",
L"0x0820                        Urdu                          India                         1256                          URI",
L"0x0843                        Uzbek                         Uzbekistan (Cyrillic)         1251                          UZB",
L"0x0443                        Uzbek                         Uzbekistan (Latin)            1254                          UZB",
L"0x042a                        Vietnamese                    Viet Nam                      1258                          VIT",
NULL};
