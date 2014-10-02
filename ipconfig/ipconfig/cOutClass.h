

class COutput
{
public:
	COutput(HWND hEdit) { m_hEdit = hEdit; };
	COutput() { m_hEdit = NULL; };

    COutput& operator<<(LPCTSTR);
	COutput& operator<<(WORD);
	COutput& operator<<(CHAR);
	COutput& operator<<(DWORD);
	COutput& operator<<(FLOAT);
	COutput& operator<<(int);
	COutput& operator<<(char* lpcharOutput);
	COutput& operator<<(wchar_t* lpwcharOutput);
	
	void print(const wchar_t *fmt, ...);
	
	void SetOutputWindow(HWND hEdit) { m_hEdit = hEdit; };
	void COutput::CLS();
	void refresh();

private:
	HWND m_hEdit;
};

extern const TCHAR endl[];
extern const TCHAR tab[];

extern COutput cout;		
extern HWND	hWndEdit;		// Read only edit box for output display
extern HWND	hWnd;			// Handle to application window
extern HINSTANCE hInst;		// The current instance
extern TCHAR szTitle[];		// The title bar text
